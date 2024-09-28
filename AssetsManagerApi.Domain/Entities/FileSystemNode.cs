using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Domain.Entities;

public class FileSystemNode : EntityBase
{
    public string Name { get; set; }

    public string ParentId { get; set; }

    public FileType Type { get; set; }

    public FileTag Tag { get; set; }
}

