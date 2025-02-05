using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.CreateDto;

public class StartProjectCreateDto
{
    public string Prompt { get; set; }

    public Languages Language { get; set; } = Languages.csharp;
}
