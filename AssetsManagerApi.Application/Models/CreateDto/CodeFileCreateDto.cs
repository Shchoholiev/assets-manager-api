namespace AssetsManagerApi.Application.Models.CreateDto;

public class CodeFileCreateDto
{
    public string Text { get; set; }

    public string Language { get; set; }

    public string Name { get; set; }

    public string ParentId { get; set; }
}
