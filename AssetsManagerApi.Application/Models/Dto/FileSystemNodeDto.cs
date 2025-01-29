using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;
using System.Text.Json.Serialization;

namespace AssetsManagerApi.Application.Models.Dto;

[JsonDerivedType(typeof(FolderDto), "folder")]
[JsonDerivedType(typeof(CodeFileDto), "codeFile")]
public class FileSystemNodeDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public FileType Type { get; set; }
}
