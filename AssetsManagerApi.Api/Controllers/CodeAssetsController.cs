using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace AssetsManagerApi.Api.Controllers;

[Route("codeAssets")]
public class CodeAssetsController(ICodeAssetsService codeAssetsService) : ApiController
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    [HttpGet]
    public async Task<ActionResult<PagedList<CodeAssetDto>>> GetCodeAssetsPage([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var codeAssets = await this._codeAssetsService.GetCodeAssetsPage(pageNumber, pageSize, cancellationToken);
        return Ok(codeAssets);
    }

    [HttpGet("{codeAssetId}")]
    public ActionResult<CodeAssetDto> GetCodeAssetByIdPage(string codeAssetId)
    {
        var dummy = new CodeAssetDto
        {
            Id = "ecb7e985-5d24-463b-ab62-6368395584e2",
            Name = "JavaScript Project",
            Description = "A JavaScript project with multiple utility functions.",
            RootFolder = new FolderDto
            {
                Id = "b2c3d4e5-f678-9012-abcd-ef2345678901",
                Name = "JavaScriptProjectFolder",
                Type = FileType.Folder,
                Items = new List<FileSystemNodeDto>()
                {
                    new FolderDto
                    {
                        Id = "c3d4e5f6-7890-1234-abcd-ef3456789012",
                        Name = "Utils",
                        Type = FileType.Folder,
                        Items = new List<FileSystemNodeDto>()
                        {
                            new CodeFileDto
                            {
                                Id = "d4e5f678-9012-3456-abcd-ef4567890123",
                                Name = "mathUtils.js",
                                Type = FileType.CodeFile,
                                Text = @"
export function add(a, b) {
    return a + b;
}

export function subtract(a, b) {
    return a - b;
}",
                                Language = "Javascript",
                            },
                            new CodeFileDto
                            {
                                Id = "e5f67890-1234-5678-abcd-ef5678901234",
                                Name = "stringUtils.js",
                                Type = FileType.CodeFile,
                                Text = @"
export function capitalize(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

export function lowercase(str) {
    return str.toLowerCase();
}",
                                Language = "Javascript",
                            },
                            new CodeFileDto
                            {
                                Id = "f6f67890-2234-5678-abcd-ef6678901234",
                                Name = "arrayUtils.js",
                                Type = FileType.CodeFile,
                                Text = @"
export function firstElement(arr) {
    return arr.length > 0 ? arr[0] : null;
}

export function lastElement(arr) {
    return arr.length > 0 ? arr[arr.length - 1] : null;
}",
                                Language = "Javascript",
                            }
                        }
                    },
                    new FolderDto
                    {
                        Id = "f6789012-3456-7890-abcd-ef6789012345",
                        Name = "Components",
                        Type = FileType.Folder,
                        Items = new List<FileSystemNodeDto>()
                        {
                            new CodeFileDto
                            {
                                Id = "67890123-4567-8901-abcd-ef7890123456",
                                Name = "header.js",
                                Type = FileType.CodeFile,
                                Text = @"
export function Header() {
    return `<header>Welcome</header>`;
}",
                                Language = "Javascript",
                            },
                            new CodeFileDto
                            {
                                Id = "78901234-5678-9012-abcd-ef8901234567",
                                Name = "footer.js",
                                Type = FileType.CodeFile,
                                Text = @"
export function Footer() {
    return `<footer>Goodbye</footer>`;
}",
                                Language = "Javascript",
                            },
                            new FolderDto
                            {
                                Id = "8a901234-5678-9012-abcd-ef9901234567",
                                Name = "Buttons",
                                Type = FileType.Folder,
                                Items = new List<FileSystemNodeDto>()
                                {
                                    new CodeFileDto
                                    {
                                        Id = "9b901234-5678-9012-abcd-efaa01234567",
                                        Name = "primaryButton.js",
                                        Type = FileType.CodeFile,
                                        Text = @"
export function PrimaryButton() {
    return `<button class='primary'>Click me</button>`;
}",
                                        Language = "Javascript",
                                    },
                                    new CodeFileDto
                                    {
                                        Id = "ac901234-5678-9012-abcd-efbb01234567",
                                        Name = "secondaryButton.js",
                                        Type = FileType.CodeFile,
                                        Text = @"
export function SecondaryButton() {
    return `<button class='secondary'>Click me</button>`;
}",
                                        Language = "Javascript",
                                    }
                                }
                            }
                        }
                    },
                    new FolderDto
                    {
                        Id = "431e3321-a436-4f1f-9118-916dca3841c3",
                        Name = "Others",
                        Type = FileType.Folder,
                        Items = new List<FileSystemNodeDto>()
                        {
                            new CodeFileDto
                            {
                                Id = "bc901234-5678-9012-abcd-efcc01234567",
                                Name = "config.js",
                                Type = FileType.CodeFile,
                                Text = @"
export const CONFIG = {
    apiUrl: 'https://api.example.com',
    timeout: 5000
};",
                                Language = "Javascript",
                            },
                            new CodeFileDto
                            {
                                Id = "cd901234-5678-9012-abcd-efdd01234567",
                                Name = "logger.js",
                                Type = FileType.CodeFile,
                                Text = @"
export function logInfo(message) {
    console.log(`INFO: ${message}`);
}

export function logError(message) {
    console.error(`ERROR: ${message}`);
}",
                                Language = "Javascript",
                            }
                        }
                    },
                    new CodeFileDto
                    {
                        Id = "89012345-6789-0123-abcd-ef9012345678",
                        Name = "app.js",
                        Type = FileType.CodeFile,
                        Text = @"
function greet(name) {
    console.log(`Hello, ${name}!`);
}

function multiply(a, b) {
    return a * b;
}

greet('Developer');
const result = multiply(6, 7);
console.log(`The product of 6 and 7 is ${result}`);
",
                        Language = "Javascript",
                    }
                }
            },

            PrimaryCodeFile = new CodeFileDto
            {
                Id = "90123456-7890-1234-abcd-ef0123456789",
                Name = "app.js",
                Type = FileType.CodeFile,
                Text = @"
function greet(name) {
    console.log(`Hello, ${name}!`);
}

function multiply(a, b) {
    return a * b;
}

greet('Developer');
const result = multiply(6, 7);
console.log(`The product of 6 and 7 is ${result}`);
",
                Language = "Javascript",
            },
            AssetType = AssetTypes.Corporate,
            Language = "Javascript",
            Tags = new List<TagDto>()
            {
                new TagDto
                {
                    Id = "84bcd7cd-b020-40d5-b339-7b05db3db4ec",
                    Name = "JavaScript",
                },
                new TagDto
                {
                    Id = "322d9313-1712-47ac-826c-10a54921f84e",
                    Name = "Functions",
                },
                new TagDto
                {
                    Id = "cdb6210e-996c-4691-b9ea-3d4331fb20bc",
                    Name = "Utility",
                },
                new TagDto
                {
                    Id = "427aa013-1bba-4a1f-b285-6b9953db7523",
                    Name = "WebDevelopment",
                },
            },
        };

        return Ok(dummy);
    }

    [HttpGet("test")]
    public ActionResult<PagedList<CodeAssetDto>> Test([FromQuery] Expression<Func<CodeAssetDto, bool>> predicate, [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var predicatee = predicate; 
        return Ok();
    }
}