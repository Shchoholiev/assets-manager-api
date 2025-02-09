using AssetsManagerApi.Domain.Entities;

namespace AssetsManagerApi.IntegrationTests.Models;

public class FolderDataSeeding : Folder
{
    public List<FileSystemNode> Items { get; set; }
}
