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
    
    private readonly ICodeFilesService _codeFilesService;

    private readonly IMapper _mapper;

    public FoldersService(
        IFoldersRepository foldersRepository, 
        ICodeFilesRepository codeFilesRepository,
        ICodeFilesService codeFilesService,
        IMapper mapper)
    {
        _foldersRepository = foldersRepository;
        _codeFilesRepository = codeFilesRepository;
        _codeFilesService = codeFilesService;
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
            CreatedById = GlobalUser.Id,
            CreatedDateUtc = DateTime.UtcNow,
        };

        await _foldersRepository.AddAsync(folderEntity, cancellationToken);

        return _mapper.Map<FolderDto>(folderEntity);
    }

    public async Task<FolderDto> DeleteFolderAsync(string folderId, CancellationToken cancellationToken)
    {
        var folder = await _foldersRepository.GetOneAsync(folderId, cancellationToken);

        if (folder == null)
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

        /// <summary>
    /// Recursively saves a FolderDto along with its nested folders and code files.
    /// If the folder or any code file already has an Id, it is assumed to be saved, and creation is skipped.
    /// </summary>
    /// <param name="folderDto">The folder hierarchy to save.</param>
    /// <param name="parentId">The parent folder's Id, or null for root.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created (or existing) folder as stored in the database.</returns>
    public async Task<FolderDto> SaveFolderHierarchyAsync(FolderDto folderDto, string? parentId, CancellationToken cancellationToken)
    {
        FolderDto createdFolder;
        if (!string.IsNullOrEmpty(folderDto.Id))
        {
            createdFolder = folderDto;
        }
        else
        {
            var folderCreateDto = new FolderCreateDto
            {
                Name = folderDto.Name,
                ParentId = parentId
            };

            createdFolder = await CreateFolderAsync(folderCreateDto, cancellationToken);
            folderDto.Id = createdFolder.Id;
        }

        if (folderDto.Items != null && folderDto.Items.Count > 0)
        {
            foreach (var item in folderDto.Items)
            {
                if (item.Type == FileType.Folder)
                {
                    if (item is FolderDto childFolder)
                    {
                        await SaveFolderHierarchyAsync(childFolder, createdFolder.Id, cancellationToken);
                    }
                }
                else if (item.Type == FileType.CodeFile)
                {
                    if (item is CodeFileDto codeFile && string.IsNullOrEmpty(codeFile.Id))
                    {
                        var codeFileCreateDto = new CodeFileCreateDto
                        {
                            Name = codeFile.Name,
                            Text = codeFile.Text,
                            Language = codeFile.Language,
                            ParentId = createdFolder.Id
                        };

                        var createdCodeFile = await _codeFilesService.CreateCodeFileAsync(codeFileCreateDto, cancellationToken);
                        item.Id = createdCodeFile.Id;
                    }
                }
            }
        }

        return createdFolder;
    }
}
