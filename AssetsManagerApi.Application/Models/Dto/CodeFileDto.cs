using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.Dto;

public class CodeFileDto : FileSystemNodeDto
{
    public string Text { get; set; }

    public string Language { get; set; }
}
