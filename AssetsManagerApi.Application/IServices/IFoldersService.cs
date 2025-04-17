using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.UpdateDto;

namespace AssetsManagerApi.Application.IServices;

public interface IFoldersService
{
    Task<FolderDto> DeleteFolderAsync(string folderId, CancellationToken cancellationToken);

    Task<FolderDto> CreateFolderAsync(FolderCreateDto createDto, CancellationToken cancellationToken);

    Task<FolderDto> UpdateFolderAsync(FolderUpdateDto dto, CancellationToken cancellationToken);

    Task<FolderDto> SaveFolderHierarchyAsync(FolderDto folderDto, string? parentId, CancellationToken cancellationToken);
}
