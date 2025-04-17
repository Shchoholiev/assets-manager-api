using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AssetsManagerApi.Application.Utils;

public static class ProgramCsGenerator
{
    private class TypeRegistration
    {
        public string TypeName { get; set; }
        public string Namespace { get; set; }
        public bool IsInterface { get; set; }
        public bool IsDbContext { get; set; }
    }

    public static CodeFileDto GenerateProgramCs(FolderDto folder, string rootNamespace)
    {
        var registrations = new List<TypeRegistration>();
        var namespaces = new HashSet<string>();

        void TraverseFolder(FolderDto currentFolder, string currentNamespace)
        {
            namespaces.Add(currentNamespace);
            if (currentFolder.Items == null)
                return;
            foreach (var item in currentFolder.Items)
            {
                if (item is FolderDto subFolder)
                {
                    var subNamespace = currentNamespace + "." + subFolder.Name;
                    TraverseFolder(subFolder, subNamespace);
                }
                else if (item is CodeFileDto codeFile)
                {
                    var typeName = Path.GetFileNameWithoutExtension(codeFile.Name).Trim();
                    var isInterface = typeName.StartsWith("I") && typeName.Length > 1 && char.IsUpper(typeName[1]);
                    var isDbContext = typeName.EndsWith("Context", StringComparison.OrdinalIgnoreCase);
                    registrations.Add(new TypeRegistration
                    {
                        TypeName = typeName,
                        Namespace = currentNamespace,
                        IsInterface = isInterface,
                        IsDbContext = isDbContext
                    });
                }
            }
        }

        TraverseFolder(folder, rootNamespace);

        var registrationStatements = new List<string>();
        var implementations = registrations.Where(r => !r.IsInterface).ToList();
        // TODO: needs improvement to handle registrations better
        // foreach (var impl in implementations)
        // {
        //     var interfaceCandidate = registrations.FirstOrDefault(r =>
        //         r.IsInterface &&
        //         r.TypeName == "I" + impl.TypeName &&
        //         r.Namespace == impl.Namespace);
        //     if (interfaceCandidate != null)
        //     {
        //         registrationStatements.Add($"builder.Services.AddScoped<{interfaceCandidate.TypeName}, {impl.TypeName}>();");
        //     }
        //     else if (impl.IsDbContext)
        //     {
        //         registrationStatements.Add($"builder.Services.AddDbContext<{impl.TypeName}>();");
        //     }
        //     else
        //     {
        //         registrationStatements.Add($"builder.Services.AddScoped<{impl.TypeName}>();");
        //     }
        // }

        var usingDirectives = new List<string>
        {
            "using Microsoft.AspNetCore.Builder;",
            "using Microsoft.Extensions.DependencyInjection;",
            "using Microsoft.Extensions.Hosting;"
        };

        foreach (var ns in namespaces)
        {
            if (ns != rootNamespace)
                usingDirectives.Add($"using {ns};");
        }

        var compilationUnit = SyntaxFactory.CompilationUnit();

        var usingSyntaxes = usingDirectives
            .Select(u => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u.Replace("using ", "").Replace(";", "").Trim())));
        compilationUnit = compilationUnit.AddUsings(usingSyntaxes.ToArray());

        var statements = new List<StatementSyntax>
        {
            SyntaxFactory.ParseStatement("var builder = WebApplication.CreateBuilder(args);")
        };

        foreach (var reg in registrationStatements)
        {
            statements.Add(SyntaxFactory.ParseStatement(reg));
        }

        statements.Add(SyntaxFactory.ParseStatement("builder.Services.AddControllers();"));
        statements.Add(SyntaxFactory.ParseStatement("var app = builder.Build();"));
        statements.Add(SyntaxFactory.ParseStatement("if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }"));
        statements.Add(SyntaxFactory.ParseStatement("app.UseHttpsRedirection();"));
        statements.Add(SyntaxFactory.ParseStatement("app.UseAuthorization();"));
        statements.Add(SyntaxFactory.ParseStatement("app.MapControllers();"));
        statements.Add(SyntaxFactory.ParseStatement("app.Run();"));

        var globalStatements = statements.Select(s => SyntaxFactory.GlobalStatement(s)).ToArray();
        compilationUnit = compilationUnit.AddMembers(globalStatements);

        var code = compilationUnit.NormalizeWhitespace().ToFullString();

        var programFile = new CodeFileDto
        {
            Name = "Program.cs",
            Type = FileType.CodeFile,
            Text = code,
            Language = "Csharp"
        };

        return programFile;
    }
}
