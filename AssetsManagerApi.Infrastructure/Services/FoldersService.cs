using AssetsManagerApi.Application.Exceptions;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Global;
using AssetsManagerApi.Application.Models.UpdateDto;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;
using AutoMapper;

namespace AssetsManagerApi.Infrastructure.Services;
public class FoldersService : IFoldersService
{
    private readonly IFoldersRepository _foldersRepository;

    private readonly ICodeFilesRepository _codeFilesRepository;

    private readonly IMapper _mapper;

    public FoldersService(IFoldersRepository foldersRepository, ICodeFilesRepository codeFilesRepository, IMapper mapper)
    {
        _foldersRepository = foldersRepository;
        _codeFilesRepository = codeFilesRepository;
        _mapper = mapper;
    }

    public async Task<FolderDto> CreateFolderAsync(FolderCreateDto createDto, CancellationToken cancellationToken)
    {
        if (createDto.ParentId != null)
        {
            var folder = await _foldersRepository.GetOneAsync(createDto.ParentId, cancellationToken);

            if (folder == null)
            {
                throw new EntityNotFoundException("Parent folder not found");
            }
        }

        var folderEntity = new Folder()
        {
            Name = createDto.Name,
            Type = FileType.Folder,
            ParentId = createDto.ParentId,
        };

        await _foldersRepository.AddAsync(folderEntity, cancellationToken);

        return _mapper.Map<FolderDto>(folderEntity);
    }

    public async Task<FolderDto> DeleteFolderAsync(string folderId, CancellationToken cancellationToken)
    {
        var folder = await _foldersRepository.GetOneAsync(folderId, cancellationToken);

        if (folderId == null)
        {
            throw new EntityNotFoundException("Folder not found");
        }

        await DeleteFolderRecursiveAsync(folderId, cancellationToken);

        return _mapper.Map<FolderDto>(folder);
    }

    public async Task<FolderDto> UpdateFolderAsync(FolderUpdateDto dto, CancellationToken cancellationToken)
    {
        var folder = await _foldersRepository.GetOneAsync(dto.Id, cancellationToken);

        if (folder == null)
        {
            throw new EntityNotFoundException("Fodler not found");
        }

        folder.Name = dto.Name;
        folder.ParentId = dto.ParentId;
        folder.LastModifiedDateUtc = DateTime.UtcNow;
        folder.LastModifiedById = GlobalUser.Id;

        var entity = await _foldersRepository.UpdateAsync(folder, cancellationToken);

        return _mapper.Map<FolderDto>(entity);
    }

    private async Task DeleteFolderRecursiveAsync(string folderId, CancellationToken cancellationToken)
    {
        var files = await _codeFilesRepository.GetAllAsync(f => f.ParentId == folderId, cancellationToken);
        foreach (var file in files)
        {
            await _codeFilesRepository.DeleteAsync(file, cancellationToken);
        }

        var folders = await _foldersRepository.GetAllAsync(f => f.ParentId == folderId, cancellationToken);
        foreach (var folder in folders)
        {
            await DeleteFolderRecursiveAsync(folder.Id, cancellationToken);
        }

        var folderToDelete = await _foldersRepository.GetOneAsync(folderId, cancellationToken);
        if (folderToDelete != null)
        {
            await _foldersRepository.DeleteAsync(folderToDelete, cancellationToken);
        }
    }
}
