using AssetsManagerApi.Application.Models.Dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AssetsManagerApi.Application.Utils;

public static class CSharpFileTransformer
{
    public static (FolderDto UpdatedFolder, List<string> RemovedNamespaces) UpdateNamespaces(FolderDto rootFolder)
    {
        var removedNamespaces = new List<string>();
        var updatedFolder = RewriteFolderWithNamespace(rootFolder, rootFolder.Name, removedNamespaces);
        return (updatedFolder, removedNamespaces);
    }

    private static FolderDto RewriteFolderWithNamespace(FolderDto folder, string currentNamespace, List<string> removedNamespaces)
    {
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
                var (updatedText, oldNamespace) = RewriteNamespace(codeFile.Text, currentNamespace);

                if (oldNamespace != null && oldNamespace != currentNamespace)
                {
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

        var rewriter = new NamespaceRewriter(newNamespace);
        var newRoot = rewriter.Visit(root);

        return (newRoot.NormalizeWhitespace().ToFullString(), oldNamespace);
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
}
