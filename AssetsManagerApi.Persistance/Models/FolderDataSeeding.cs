using AssetsManagerApi.Domain.Entities;

namespace AssetsManagerApi.Persistance.Models;

public class FolderDataSeeding : Folder
{
    public List<FileSystemNode> Items { get; set; }
}
