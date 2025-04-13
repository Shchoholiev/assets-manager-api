using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Utils;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.UnitTests;

[TestFixture]
public class CSharpFileTransformerTests
{
    [Test]
    public void UpdateNamespaces_RewritesNamespaceInCodeFiles()
    {
        var folder = new FolderDto
        {
            Name = "Root",
            Type = FileType.Folder,
            Items =
            [
                new FolderDto
                {
                    Name = "Controllers",
                    Type = FileType.Folder,
                    Items =
                    [
                        new CodeFileDto
                        {
                            Name = "MyController.cs",
                            Type = FileType.CodeFile,
                            Text = "namespace OldNamespace.Controllers\n{\n    public class MyController { }\n}",
                            Language = "Csharp"
                        }
                    ]
                }
            ]
        };

        var (updated, removedNamespaces) = CSharpFileTransformer.UpdateNamespaces(folder);

        var codeFile = ((FolderDto)updated.Items[0]).Items[0] as CodeFileDto;

        Assert.That(codeFile.Text, Does.Contain("namespace Root.Controllers"));
        Assert.That(codeFile.Text, Does.Not.Contain("OldNamespace"));
        Assert.That(removedNamespaces, Does.Contain("OldNamespace.Controllers"));
    }

    [Test]
    public void RemoveInvalidUsings_RemovesUsingsMatchingRemovedNamespaces()
    {
        var folder = new FolderDto
        {
            Name = "Root",
            Type = FileType.Folder,
            Items =
            [
                new CodeFileDto
                {
                    Name = "Service.cs",
                    Type = FileType.CodeFile,
                    Language = "Csharp",
                    Text = """
                    using System;
                    using OldNamespace.Services;
                    using OldNamespace.Models;

                    namespace Root
                    {
                        public class Service { }
                    }
                    """
                }
            ]
        };

        var removedNamespaces = new List<string> { "OldNamespace.Services" };

        var result = CSharpFileTransformer.RemoveInvalidUsings(folder, removedNamespaces);
        var file = result.Items[0] as CodeFileDto;

        Assert.That(file.Text, Does.Contain("using System;"));
        Assert.That(file.Text, Does.Not.Contain("OldNamespace.Services"));
        Assert.That(file.Text, Does.Contain("OldNamespace.Models"));
    }

    [Test]
    public void BuildClassToNamespaceDictionary_WithNestedFoldersAndFiles_ReturnsCorrectMapping()
    {
        var folder = new FolderDto
        {
            Name = "Root",
            Type = FileType.Folder,
            Items = new List<FileSystemNodeDto>
        {
            new FolderDto
            {
                Name = "SubFolder",
                Type = FileType.Folder,
                Items = new List<FileSystemNodeDto>
                {
                    new CodeFileDto
                    {
                        Name = "TestFile.cs",
                        Type = FileType.CodeFile,
                        Text = "public class TestApi { }",
                        Language = "Csharp"
                    }
                }
            },
            new CodeFileDto
            {
                Name = "Main.cs",
                Type = FileType.CodeFile,
                Text = "public class MainApi { }",
                Language = "Csharp"
            }
        }
        };

        var dictionary = CSharpFileTransformer.BuildClassToNamespaceDictionary(folder, "MyApp");
        
        Assert.That(dictionary.ContainsKey("TestApi"), Is.True);
        Assert.That(dictionary["TestApi"], Is.EqualTo("MyApp.Root.SubFolder"));

        Assert.That(dictionary.ContainsKey("MainApi"), Is.True);
        Assert.That(dictionary["MainApi"], Is.EqualTo("MyApp.Root"));
    }

    [Test]
    public void BuildClassToNamespaceDictionary_WithMultipleTypesInSingleFile_ReturnsMappingForAllTypes()
    {
        var folder = new FolderDto
        {
            Name = "Root",
            Type = FileType.Folder,
            Items = new List<FileSystemNodeDto>
        {
            new CodeFileDto
            {
                Name = "MultiTypeFile.cs",
                Type = FileType.CodeFile,
                Text = @"
                    public class MultiApi { }
                    public struct HelperStruct { }
                    public interface ITestInterface { }
                    public enum SomeEnum { A, B }
                ",
                Language = "Csharp"
            }
        }
        };

        Dictionary<string, string> dictionary = CSharpFileTransformer.BuildClassToNamespaceDictionary(folder, "MyApp");

        Assert.That(dictionary.ContainsKey("MultiApi"), Is.True);
        Assert.That(dictionary["MultiApi"], Is.EqualTo("MyApp.Root"));

        Assert.That(dictionary.ContainsKey("HelperStruct"), Is.True);
        Assert.That(dictionary["HelperStruct"], Is.EqualTo("MyApp.Root"));

        Assert.That(dictionary.ContainsKey("ITestInterface"), Is.True);
        Assert.That(dictionary["ITestInterface"], Is.EqualTo("MyApp.Root"));

        Assert.That(dictionary.ContainsKey("SomeEnum"), Is.True);
        Assert.That(dictionary["SomeEnum"], Is.EqualTo("MyApp.Root"));
    }

    [Test]
    public void BuildClassToNamespaceDictionary_WithNoItems_ReturnsEmptyDictionary()
    {
        var folder = new FolderDto
        {
            Name = "EmptyRoot",
            Type = FileType.Folder,
            Items = null
        };

        Dictionary<string, string> dictionary = CSharpFileTransformer.BuildClassToNamespaceDictionary(folder, "MyApp");

        Assert.That(dictionary.Count, Is.EqualTo(0));
    }
}