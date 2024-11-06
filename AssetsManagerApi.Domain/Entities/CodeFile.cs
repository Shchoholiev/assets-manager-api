using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Domain.Entities;

public class CodeFile : FileSystemNode
{
    public string Text { get; set; }

    public Languages Language { get; set; }
}
