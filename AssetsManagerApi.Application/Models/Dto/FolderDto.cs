namespace AssetsManagerApi.Application.Models.Dto;

public class FolderDto : FileSystemNodeDto
{
    public List<FileSystemNodeDto>? Items = new();
}
