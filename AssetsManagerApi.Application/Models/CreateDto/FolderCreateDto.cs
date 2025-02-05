namespace AssetsManagerApi.Application.Models.CreateDto;

public class FolderCreateDto
{
    public string Name { get; set; }

    public string? ParentId { get; set; }
}
