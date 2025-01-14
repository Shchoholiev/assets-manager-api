using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.Dto;

public class FileSystemNodeDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public FolderDto? ParentFolder { get; set; }

    public FileType Type { get; set; }
}
