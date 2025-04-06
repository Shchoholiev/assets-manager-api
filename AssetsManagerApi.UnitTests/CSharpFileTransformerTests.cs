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
                            Language = "C#"
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
                    Language = "C#",
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
}