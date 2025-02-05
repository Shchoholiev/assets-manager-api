namespace AssetsManagerApi.Application.Models.UpdateDto;
public class FolderUpdateDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string? ParentId { get; set; }
}
