namespace AssetsManagerApi.Domain.Entities;

public class Folder : FileSystemNode
{
    public List<FileSystemNode>? Items { get; set; }
}
