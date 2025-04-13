using AssetsManagerApi.Application.Models.Dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace AssetsManagerApi.Application.Utils;

public static class CSharpFileTransformer
{
    private static readonly ILogger logger = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    }).CreateLogger(nameof(CSharpFileTransformer));

    public static (FolderDto UpdatedFolder, List<string> RemovedNamespaces) UpdateNamespaces(FolderDto rootFolder)
    {
        logger.LogInformation("Starting UpdateNamespaces for root folder {FolderName}", rootFolder.Name);
        var removedNamespaces = new List<string>();
        var updatedFolder = RewriteFolderWithNamespace(rootFolder, rootFolder.Name, removedNamespaces);
        logger.LogInformation("Finished UpdateNamespaces for root folder {FolderName}", rootFolder.Name);
        return (updatedFolder, removedNamespaces);
    }

    private static FolderDto RewriteFolderWithNamespace(FolderDto folder, string currentNamespace, List<string> removedNamespaces)
    {
        logger.LogInformation("Rewriting folder {FolderName} with namespace {Namespace}", folder.Name, currentNamespace);
        var updatedFolder = new FolderDto
        {
            Name = folder.Name,
            Type = folder.Type,
            Items = new List<FileSystemNodeDto>()
        };

        foreach (var item in folder.Items ?? [])
        {
            if (item is CodeFileDto codeFile)
            {
                logger.LogInformation("Rewriting namespace for file {FileName} in namespace {Namespace}", codeFile.Name, currentNamespace);
                var (updatedText, oldNamespace) = RewriteNamespace(codeFile.Text, currentNamespace);

                if (oldNamespace != null && oldNamespace != currentNamespace)
                {
                    logger.LogInformation("Namespace changed for file {FileName}: old namespace {OldNamespace} removed", codeFile.Name, oldNamespace);
                    removedNamespaces.Add(oldNamespace);
                }

                updatedFolder.Items.Add(new CodeFileDto
                {
                    Name = codeFile.Name,
                    Type = codeFile.Type,
                    Language = codeFile.Language,
                    Text = updatedText
                });
            }
            else if (item is FolderDto subFolder)
            {
                var subNamespace = $"{currentNamespace}.{subFolder.Name}";
                var updatedSubFolder = RewriteFolderWithNamespace(subFolder, subNamespace, removedNamespaces);
                updatedFolder.Items.Add(updatedSubFolder);
            }
        }

        return updatedFolder;
    }

    private static (string UpdatedText, string? OldNamespace) RewriteNamespace(string originalText, string newNamespace)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(originalText);
        var root = syntaxTree.GetRoot();
        var nsNode = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        var oldNamespace = nsNode?.Name.ToString();

        logger.LogInformation("Rewriting namespace. Old: {OldNamespace}, New: {NewNamespace}", oldNamespace ?? "null", newNamespace);

        var rewriter = new NamespaceRewriter(newNamespace);
        var newRoot = rewriter.Visit(root);
        var updatedText = newRoot.NormalizeWhitespace().ToFullString();
        logger.LogInformation("Completed rewriting namespace. New text length: {Length}", updatedText.Length);
        return (updatedText, oldNamespace);
    }

    private class NamespaceRewriter(string newNamespace) : CSharpSyntaxRewriter
    {
        private readonly string _newNamespace = newNamespace;

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            var updatedNamespace = SyntaxFactory.ParseName(_newNamespace)
                .WithTriviaFrom(node.Name);
            return node.WithName(updatedNamespace);
        }

        public override SyntaxNode VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
        {
            var updatedNamespace = SyntaxFactory.ParseName(_newNamespace)
                .WithTriviaFrom(node.Name);
            return node.WithName(updatedNamespace);
        }
    }

    public static FolderDto RemoveInvalidUsings(FolderDto folder, List<string> removedNamespaces)
    {
        logger.LogInformation("Removing invalid usings for folder {FolderName}", folder.Name);
        var result = RemoveUsingsRecursive(folder, removedNamespaces);
        logger.LogInformation("Completed removing invalid usings for folder {FolderName}", folder.Name);
        return result;
    }

    /// <summary>
    /// Recursively builds a dictionary mapping actual type names extracted from C# source code
    /// in CodeFileDto instances to a fully-qualified namespace derived from the folder hierarchy.
    /// Only types declared within the file content (classes, structs, interfaces, enums) are included.
    /// </summary>
    /// <param name="rootFolder">The root folder representing a hierarchical structure of folders and code files.</param>
    /// <param name="baseNamespace">A base namespace used as the root for the resulting namespace paths. Default is "MyApp".</param>
    /// <returns>
    /// A dictionary where each key is a type name (extracted from CodeFileDto content) and the value is the namespace
    /// determined by the code file's folder location.
    /// </returns>
    public static Dictionary<string, string> BuildClassToNamespaceDictionary(FolderDto rootFolder, string baseNamespace = "MyApp")
    {
        logger.LogInformation("Building class-to-namespace dictionary starting from root folder {FolderName}", rootFolder.Name);

        var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        void ProcessFolder(FolderDto folder, string currentNamespace)
        {
            foreach (var item in folder.Items ?? [])
            {
                if (item is FolderDto subFolder)
                {
                    ProcessFolder(subFolder, $"{currentNamespace}.{subFolder.Name}");
                }
                else if (item is CodeFileDto codeFile)
                {
                    if (codeFile.Language?.Equals("Csharp", StringComparison.OrdinalIgnoreCase) != true)
                    {
                        logger.LogInformation("Skipping file {FileName} with unsupported language: {Language}", codeFile.Name, codeFile.Language);
                        continue;
                    }

                    logger.LogInformation("Processing file {FileName} in namespace {Namespace}", codeFile.Name, currentNamespace);

                    var syntaxTree = CSharpSyntaxTree.ParseText(codeFile.Text);
                    var rootNode = syntaxTree.GetCompilationUnitRoot();

                    var classDeclarations = rootNode.DescendantNodes().OfType<ClassDeclarationSyntax>();
                    var structDeclarations = rootNode.DescendantNodes().OfType<StructDeclarationSyntax>();
                    var interfaceDeclarations = rootNode.DescendantNodes().OfType<InterfaceDeclarationSyntax>();
                    var enumDeclarations = rootNode.DescendantNodes().OfType<EnumDeclarationSyntax>();

                    foreach (var decl in classDeclarations)
                    {
                        var typeName = decl.Identifier.Text;
                        logger.LogInformation("Found class {TypeName} in file {FileName}", typeName, codeFile.Name);
                        mapping[typeName] = currentNamespace;
                    }
                    foreach (var decl in structDeclarations)
                    {
                        var typeName = decl.Identifier.Text;
                        logger.LogInformation("Found struct {TypeName} in file {FileName}", typeName, codeFile.Name);
                        mapping[typeName] = currentNamespace;
                    }
                    foreach (var decl in interfaceDeclarations)
                    {
                        var typeName = decl.Identifier.Text;
                        logger.LogInformation("Found interface {TypeName} in file {FileName}", typeName, codeFile.Name);
                        mapping[typeName] = currentNamespace;
                    }
                    foreach (var decl in enumDeclarations)
                    {
                        var typeName = decl.Identifier.Text;
                        logger.LogInformation("Found enum {TypeName} in file {FileName}", typeName, codeFile.Name);
                        mapping[typeName] = currentNamespace;
                    }
                }
            }
        }

        ProcessFolder(rootFolder, $"{baseNamespace}.{rootFolder.Name}");

        logger.LogInformation("Completed building class-to-namespace dictionary with {Count} entries", mapping.Count);

        return mapping;
    }

    private static FolderDto RemoveUsingsRecursive(FolderDto folder, List<string> removedNamespaces)
    {
        logger.LogInformation("Removing usings in folder {FolderName}", folder.Name);

        var updatedFolder = new FolderDto
        {
            Name = folder.Name,
            Type = folder.Type,
            Items = new List<FileSystemNodeDto>()
        };

        foreach (var item in folder.Items ?? [])
        {
            if (item is CodeFileDto file)
            {
                logger.LogInformation("Removing usings from file {FileName}", file.Name);

                var newText = RemoveUsingsFromCode(file.Text, removedNamespaces);
                updatedFolder.Items.Add(new CodeFileDto
                {
                    Name = file.Name,
                    Type = file.Type,
                    Language = file.Language,
                    Text = newText
                });
            }
            else if (item is FolderDto subfolder)
            {
                var updatedSubfolder = RemoveUsingsRecursive(subfolder, removedNamespaces);
                updatedFolder.Items.Add(updatedSubfolder);
            }
        }

        return updatedFolder;
    }

    private static string RemoveUsingsFromCode(string codeText, List<string> removedNamespaces)
    {
        logger.LogInformation("Removing invalid usings from code text of length {Length}", codeText.Length);

        var tree = CSharpSyntaxTree.ParseText(codeText);
        var root = tree.GetCompilationUnitRoot();

        var newUsings = root.Usings
            .Where(u => u.Name != null && !removedNamespaces.Any(oldNs => u.Name.ToString() == oldNs))
            .ToList();

        var newRoot = root.WithUsings(SyntaxFactory.List(newUsings));
        var updatedText = newRoot.NormalizeWhitespace().ToFullString();

        logger.LogInformation("Completed removal of invalid usings; updated code length is {Length}", updatedText.Length);

        return updatedText;
    }

    /// <summary>
    /// Recursively processes the given folder structure and, for every CodeFileDto encountered,
    /// adds missing using directives based on the provided type-to-namespace mapping.
    /// This method directly updates the properties of the objects in the provided folder instead of creating new copies.
    /// </summary>
    /// <param name="folder">The root folder with a hierarchical structure containing code files.</param>
    /// <param name="typeToNamespace">A dictionary mapping type names to their corresponding namespaces.</param>
    /// <returns>The modified FolderDto with code files having the missing using directives added.</returns>
    public static FolderDto AddMissingUsingsToFolder(FolderDto folder, Dictionary<string, string> typeToNamespace)
    {
        if (folder.Items == null)
            return folder;

        foreach (var item in folder.Items)
        {
            if (item is CodeFileDto codeFile)
            {
                codeFile.Text = AddMissingUsings(codeFile.Text, typeToNamespace);
            }
            else if (item is FolderDto subFolder)
            {
                AddMissingUsingsToFolder(subFolder, typeToNamespace);
            }
        }

        return folder;
    }

    /// <summary>
    /// Adds missing using directives to the given C# code text based on the specified type-to-namespace mapping.
    /// It parses the code, finds identifier usages as a heuristic for type references, and then adds any using
    /// directives for namespaces that are not already present.
    /// </summary>
    /// <param name="codeText">The original C# source code.</param>
    /// <param name="typeToNamespace">A dictionary mapping type names to namespaces.</param>
    /// <returns>The updated C# source code with missing using directives added.</returns>
    public static string AddMissingUsings(string codeText, Dictionary<string, string> typeToNamespace)
    {
        var tree = CSharpSyntaxTree.ParseText(codeText);
        var root = tree.GetCompilationUnitRoot();

        var existingUsings = root.Usings
            .Select(u => u.Name.ToString())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var usedTypeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var identifier in root.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            usedTypeNames.Add(identifier.Identifier.Text);
        }

        var requiredNamespaces = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var typeName in usedTypeNames)
        {
            if (typeToNamespace.TryGetValue(typeName, out var ns))
            {
                requiredNamespaces.Add(ns);
            }
        }

        var missingNamespaces = requiredNamespaces.Where(ns => !existingUsings.Contains(ns)).ToList();
        if (missingNamespaces.Count == 0)
        {
            return codeText;
        }

        var newUsingDirectives = missingNamespaces
             .Select(ns => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns))
                 .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed))
             .ToList();

        var allUsings = root.Usings.AddRange(newUsingDirectives);
        var sortedUsings = allUsings.OrderBy(u => u.Name.ToString()).ToArray();
        var newRoot = root.WithUsings(SyntaxFactory.List(sortedUsings));

        return newRoot.NormalizeWhitespace().ToFullString();
    }
}
