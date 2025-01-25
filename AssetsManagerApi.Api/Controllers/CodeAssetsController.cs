using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

[Route("codeAssets")]
public class CodeAssetsController(ICodeAssetsService codeAssetsService) : ApiController
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    [HttpGet]
    public async Task<ActionResult<PagedList<CodeAssetResult>>> GetCodeAssetsPage([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var codeAssets = await this._codeAssetsService.GetCodeAssetsPage(pageNumber, pageSize, cancellationToken);
        return Ok(codeAssets);
    }

    [HttpGet("{codeAssetId}")]
    public ActionResult<CodeAssetResult> GetCodeAssetByIdPage(string codeAssetId)
    {
        var dummy = new CodeAssetResult
        {
            Id = "ecb7e985-5d24-463b-ab62-6368395584e2",
            Name = "JavaScript Project",
            Description = "A JavaScript project with multiple utility functions.",
            RootFolder =
            new FolderDto
            {
                Id = "20e56a85-3b18-4645-9193-9f71a565dbc2",
                Name = "JavaScriptProjectFolder",
                ParentFolder = null,
                Type = FileType.Folder,
            },
            PrimaryCodeFile =
            new CodeFileDto
            {
                Id = "325e4359-f428-4f21-8dc6-f63924531cf5",
                Name = "app.js",
                ParentFolder =
                new FolderDto
                {
                    Id = "20e56a85-3b18-4645-9193-9f71a565dbc2",
                    Name = "JavaScriptProjectFolder",
                    ParentFolder = null,
                    Type = FileType.Folder,
                },
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
                Language = Languages.javascript,
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

            Folders = new List<FolderDto>()
            {
                new FolderDto
                {
                    Id = "20e56a85-3b18-4645-9193-9f71a565dbc2",
                    Name = "JavaScriptProjectFolder",
                    ParentFolder = null,
                    Type = FileType.Folder,
                },

                new FolderDto
                {
                    Id = "41ce016c-2f03-4125-855e-0acdb8a51d4b",
                    Name = "Utils",
                    ParentFolder =
                    new FolderDto
                    {
                        Id = "20e56a85-3b18-4645-9193-9f71a565dbc2",
                        Name = "JavaScriptProjectFolder",
                        ParentFolder = null,
                        Type = FileType.Folder,
                    },
                    Type = FileType.Folder,
                },
            },

            Files = new List<CodeFileDto>()
            {
                new CodeFileDto
                {
                    Id = "f17532e3-db92-479e-a867-ec729a697eeb",
                    Name = "app.js",
                    ParentFolder =
                    new FolderDto
                    {
                        Id = "20e56a85-3b18-4645-9193-9f71a565dbc2",
                        Name = "JavaScriptProjectFolder",
                        ParentFolder = null,
                        Type = FileType.Folder,
                    },
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
                    Language = Languages.javascript,
                },

                new CodeFileDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "mathUtils.js",
                    ParentFolder =
                    new FolderDto
                    {
                        Id = "41ce016c-2f03-4125-855e-0acdb8a51d4b",
                        Name = "Utils",
                        ParentFolder =
                        new FolderDto
                        {
                            Id = "20e56a85-3b18-4645-9193-9f71a565dbc2",
                            Name = "JavaScriptProjectFolder",
                            ParentFolder = null,
                            Type = FileType.Folder,
                        },
                        Type = FileType.Folder,
                    },
                    Type = FileType.CodeFile,
                    Text = @"
export function add(a, b) {
    return a + b;
}

export function subtract(a, b) {
    return a - b;
}",
                    Language = Languages.javascript,
                }
            },
            User = 
            new UserDto
            {
                Id = "652c3b89ae02a3135d6409fc",
                Email = "test@gmail.com",
                Name = "Test user"
            }
        };
        return Ok(dummy);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedList<CodeAssetResult>>> SearchCodeAssetsPage([FromQuery] string input, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var codeAssets = await this._codeAssetsService.SearchCodeAssetsPage(input, pageNumber, pageSize, cancellationToken);
        return Ok(codeAssets);
    }
}
