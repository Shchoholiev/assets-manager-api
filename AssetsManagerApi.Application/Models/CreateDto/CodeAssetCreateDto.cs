using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.CreateDto;
public class CodeAssetCreateDto
{
    public string Name { get; set; }

    public AssetTypes AssetType { get; set; }

    public string Language { get; set; }

    public string RootFolderName { get; set; }

    public string PrimaryCodeFileName { get; set; }
}

