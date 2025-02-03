using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Models.Dto;

public class CodeAssetDto
{
    public string Id { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

    public List<TagDto> Tags { get; set; }

    public AssetTypes AssetType { get; set; }

    public string Language { get; set; }

    public FolderDto RootFolder { get; set; }

    public string? CompanyId { get; set; }

    public CodeFileDto PrimaryCodeFile { get; set; }

    public string? UserName { get; set; }
}