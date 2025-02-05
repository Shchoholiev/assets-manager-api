using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.UpdateDto;

namespace AssetsManagerApi.Application.IServices;

public interface ICodeFilesService
{
    Task<CodeFileDto> DeleteCodeFileAsync(string codeFileId, CancellationToken cancellationToken);

    Task<CodeFileDto> CreateCodeFileAsync(CodeFileCreateDto createDto, CancellationToken cancellationToken);

    Task<CodeFileDto> UpdateCodeFileAsync(CodeFileUpdateDto dto, CancellationToken cancellationToken);
}
