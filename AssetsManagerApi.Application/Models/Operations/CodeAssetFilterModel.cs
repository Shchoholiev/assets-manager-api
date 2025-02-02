using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.Operations;

public class CodeAssetFilterModel
{
    public string? SearchString { get; set; }

    public List<string>? TagIds { get; set; }

    public AssetTypes AssetType { get; set; }

    public string? Language { get; set; }
}
