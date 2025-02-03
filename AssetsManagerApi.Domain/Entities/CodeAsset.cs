using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Domain.Entities;
public class CodeAsset : EntityBase
{
    public string Description { get; set; }

    public string Name { get; set; }

    public List<Tag> Tags { get; set; }

    public AssetTypes AssetType { get; set; }

    public Languages Language { get; set; }

    public string RootFolderId { get; set; }

    public string? CompanyId { get; set; }

    public string PrimaryCodeFileId { get; set; }
}
