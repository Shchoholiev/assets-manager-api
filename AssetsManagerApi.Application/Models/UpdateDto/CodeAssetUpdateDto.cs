using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.UpdateDto;
public class CodeAssetUpdateDto
{
    public string Id { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

    public List<string> TagsIds { get; set; }

    public AssetTypes AssetType { get; set; }

    public string Language { get; set; }

    public string RootFolderId { get; set; }

    public string PrimaryCodeFileId { get; set; }
}
