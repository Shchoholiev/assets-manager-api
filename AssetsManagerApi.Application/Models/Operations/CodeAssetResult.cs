using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.Operations;

public class CodeAssetResult
{
    public string Id { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

    public List<TagDto> Tags { get; set; }

    public AssetTypes AssetType { get; set; }

    public string Language { get; set; }

    public string RootFolderId { get; set; }

    public string? CompanyId { get; set; }

    public CodeFileDto PrimaryCodeFile { get; set; }
}
