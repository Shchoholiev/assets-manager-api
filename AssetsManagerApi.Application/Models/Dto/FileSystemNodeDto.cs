using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.Dto;

public class FileSystemNodeDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string ParentId { get; set; }

    public FileType Type { get; set; }

    public FileTagDto Tag { get; set; }
}
