namespace AssetsManagerApi.Application.Models.Dto;

public class StartProjectDto
{
    public string Id { get; set; }

    public List<CodeAssetDto> CodeAssets { get; set; }
}
