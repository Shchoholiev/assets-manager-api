using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.UpdateDto;

public class CodeFileUpdateDto
{
    public string Id { get; set; }

    public string Text { get; set; }

    public string Language { get; set; }

    public string Name { get; set; }

    public string? ParentId { get; set; }
}
