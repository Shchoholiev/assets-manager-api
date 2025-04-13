using System.Text.Json;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Utils;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.UnitTests;

[TestFixture]
public class FolderMergerTests
{
    [Test]
    public void FolderMergerTests_FoldersWithUniqueNames_ReturnsFolderWithTwoFolders()
    {
        var folder1 = new FolderDto
        {
            Name = "Root1",
            Type = FileType.Folder,
            Items =
            [
                new FolderDto
                {
                    Name = "FolderA",
                    Type = FileType.Folder,
                    Items = [ new CodeFileDto { Name = "File1.cs", Type = FileType.CodeFile, Text = "code 1", Language = "C#" } ]
                }
            ]
        };

        var folder2 = new FolderDto
        {
            Name = "Root2",
            Type = FileType.Folder,
            Items =
            [
                new FolderDto
                {
                    Name = "FolderB",
                    Type = FileType.Folder,
                    Items = [ new CodeFileDto { Name = "File2.cs", Type = FileType.CodeFile, Text = "code 2", Language = "C#" } ]
                }
            ]
        };

        var result = FolderMerger.MergeFolders([folder1, folder2]);

        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.Items.Any(f => f is FolderDto fd && fd.Name == "FolderA"));
        Assert.That(result.Items.Any(f => f is FolderDto fd && fd.Name == "FolderB"));
    }

    [Test]
    public void Merge_Folders_With_Same_Name_Should_Merge_Contents()
    {
        var folder1 = new FolderDto
        {
            Name = "Root1",
            Type = FileType.Folder,
            Items =
            [
                new FolderDto
                {
                    Name = "Common",
                    Type = FileType.Folder,
                    Items =
                    [
                        new CodeFileDto
                        {
                            Name = "File1.cs",
                            Type = FileType.CodeFile,
                            Text = "code 1",
                            Language = "C#"
                        }
                    ]
                }
            ]
        };

        var folder2 = new FolderDto
        {
            Name = "Root2",
            Type = FileType.Folder,
            Items =
            [
                new FolderDto
                {
                    Name = "Common",
                    Type = FileType.Folder,
                    Items =
                    [
                        new CodeFileDto
                        {
                            Name = "File2.cs",
                            Type = FileType.CodeFile,
                            Text = "code 2",
                            Language = "C#"
                        }
                    ]
                }
            ]
        };

        var result = FolderMerger.MergeFolders([folder1, folder2]);

        Assert.That(result.Items.Count, Is.EqualTo(1));
        var commonFolder = result.Items.First() as FolderDto;
        Assert.That(commonFolder.Name, Is.EqualTo("Common"));
        Assert.That(commonFolder.Items.Count, Is.EqualTo(2));
    }
}
