using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Utils;
using AssetsManagerApi.Domain.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AssetsManagerApi.UnitTests;

[TestFixture]
public class ProgramCsGeneratorTests
{
    [Test]
    public void GenerateProgramCs_WithEmptyFolder_ReturnsBasicTemplate()
    {
        var folder = new FolderDto
        {
            Name = "EmptyFolder",
            Type = FileType.Folder,
            Items = new List<FileSystemNodeDto>()
        };

        var codeFile = ProgramCsGenerator.GenerateProgramCs(folder, "MyApp");
        var generatedCode = codeFile.Text;

        Assert.IsFalse(string.IsNullOrEmpty(generatedCode));
        StringAssert.Contains("using Microsoft.AspNetCore.Builder;", generatedCode);
        StringAssert.Contains("var builder = WebApplication.CreateBuilder(args);", generatedCode);
        StringAssert.Contains("app.Run();", generatedCode);
        Assert.AreEqual("Program.cs", codeFile.Name);
        Assert.AreEqual("Csharp", codeFile.Language);
    }

    // [Test]
    // public void GenerateProgramCs_WithServiceAndInterface_FoundInFolder_RegistersScopedService()
    // {
    //     var folder = new FolderDto
    //     {
    //         Name = "MyApp",
    //         Type = FileType.Folder,
    //         Items = new List<FileSystemNodeDto>
    //         {
    //             new FolderDto
    //             {
    //                 Name = "Services",
    //                 Type = FileType.Folder,
    //                 Items = new List<FileSystemNodeDto>
    //                 {
    //                     new CodeFileDto
    //                     {
    //                         Name = "IMyService.cs",
    //                         Type = FileType.CodeFile,
    //                         Text = "public interface IMyService { }",
    //                         Language = "Csharp"
    //                     },
    //                     new CodeFileDto
    //                     {
    //                         Name = "MyService.cs",
    //                         Type = FileType.CodeFile,
    //                         Text = "public class MyService : IMyService { }",
    //                         Language = "Csharp"
    //                     }
    //                 }
    //             }
    //         }
    //     };

    //     var codeFile = ProgramCsGenerator.GenerateProgramCs(folder, "MyApp");
    //     var generatedCode = codeFile.Text;

    //     StringAssert.Contains("builder.Services.AddScoped<IMyService, MyService>()", generatedCode);
    // }

    // [Test]
    // public void GenerateProgramCs_WithDbContext_FoundInFolder_RegistersDbContext()
    // {
    //     var folder = new FolderDto
    //     {
    //         Name = "MyApp",
    //         Type = FileType.Folder,
    //         Items = new List<FileSystemNodeDto>
    //         {
    //             new FolderDto
    //             {
    //                 Name = "DataAccess",
    //                 Type = FileType.Folder,
    //                 Items = new List<FileSystemNodeDto>
    //                 {
    //                     new CodeFileDto
    //                     {
    //                         Name = "MyDbContext.cs",
    //                         Type = FileType.CodeFile,
    //                         Text = "public class MyDbContext { }",
    //                         Language = "Csharp"
    //                     }
    //                 }
    //             }
    //         }
    //     };

    //     var codeFile = ProgramCsGenerator.GenerateProgramCs(folder, "MyApp");
    //     var generatedCode = codeFile.Text;

    //     StringAssert.Contains("builder.Services.AddDbContext<MyDbContext>()", generatedCode);
    // }

    [Test]
    public void GenerateProgramCs_ProducesValidSyntaxTree()
    {
        var folder = new FolderDto
        {
            Name = "MyApp",
            Type = FileType.Folder,
            Items = new List<FileSystemNodeDto>
            {
                new FolderDto
                {
                    Name = "Services",
                    Type = FileType.Folder,
                    Items = new List<FileSystemNodeDto>
                    {
                        new CodeFileDto
                        {
                            Name = "MyService.cs",
                            Type = FileType.CodeFile,
                            Text = "public class MyService { }",
                            Language = "Csharp"
                        }
                    }
                }
            }
        };

        var codeFile = ProgramCsGenerator.GenerateProgramCs(folder, "MyApp");
        var generatedCode = codeFile.Text;

        var syntaxTree = CSharpSyntaxTree.ParseText(generatedCode);
        var diagnostics = syntaxTree.GetDiagnostics();
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);
        Assert.IsFalse(errors.Any(), $"Generated code has syntax errors: {string.Join(Environment.NewLine, errors)}");
    }

    [Test]
    public void GenerateProgramCs_AddsCorrectUsings()
    {
        var folder = new FolderDto
        {
            Name = "MyApp",
            Type = FileType.Folder,
            Items = new List<FileSystemNodeDto>
            {
                new FolderDto
                {
                    Name = "Services",
                    Type = FileType.Folder,
                    Items = new List<FileSystemNodeDto>
                    {
                        new CodeFileDto
                        {
                            Name = "MyService.cs",
                            Type = FileType.CodeFile,
                            Text = "public class MyService { }",
                            Language = "Csharp"
                        }
                    }
                }
            }
        };

        var codeFile = ProgramCsGenerator.GenerateProgramCs(folder, "MyApp");
        var generatedCode = codeFile.Text;

        StringAssert.Contains("using Microsoft.AspNetCore.Builder;", generatedCode);
        StringAssert.Contains("using Microsoft.Extensions.DependencyInjection;", generatedCode);
        StringAssert.Contains("using Microsoft.Extensions.Hosting;", generatedCode);
        StringAssert.Contains("using MyApp.Services;", generatedCode);
    }
}
