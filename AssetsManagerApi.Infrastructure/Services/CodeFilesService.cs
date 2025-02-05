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

public class CodeFilesService : ICodeFilesService
{
    private readonly ICodeFilesRepository _codeFilesRepository;

    private readonly IFoldersRepository _foldersRepository;

    private readonly IMapper _mapper;

    public CodeFilesService(ICodeFilesRepository codeFilesRepository, IFoldersRepository foldersRepository, IMapper mapper)
    {
        _foldersRepository = foldersRepository;
        _codeFilesRepository = codeFilesRepository;
        _mapper = mapper;
    }

    public async Task<CodeFileDto> CreateCodeFileAsync(CodeFileCreateDto createDto, CancellationToken cancellationToken)
    {
        if (createDto.ParentId != null)
        {
            var folder = await _foldersRepository.GetOneAsync(createDto.ParentId, cancellationToken);

            if (folder == null)
            {
                throw new EntityNotFoundException("Parent folder not found");
            }
        }

        var entity = new CodeFile
        {
            Name = createDto.Name,
            Type = FileType.CodeFile,
            Text = createDto.Text,
            Language = LanguagesExtensions.StringToLanguage(createDto.Language),
            ParentId = createDto.ParentId,
            CreatedById = GlobalUser.Id,
            CreatedDateUtc = DateTime.UtcNow,
        };

        var createdEntity = await _codeFilesRepository.AddAsync(entity, cancellationToken);

        return _mapper.Map<CodeFileDto>(createdEntity);
    }


    public async Task<CodeFileDto> DeleteCodeFileAsync(string codeFileId, CancellationToken cancellationToken)
    {
        var codeFile = await _codeFilesRepository.GetOneAsync(codeFileId, cancellationToken);

        if (codeFile == null)
        {
            throw new EntityNotFoundException("Code file not found");
        }

        var entity = await _codeFilesRepository.DeleteAsync(codeFile, cancellationToken);

        return _mapper.Map<CodeFileDto>(entity);
    }

    public async Task<CodeFileDto> UpdateCodeFileAsync(CodeFileUpdateDto dto, CancellationToken cancellationToken)
    {
        var codeFile = await _codeFilesRepository.GetOneAsync(dto.Id, cancellationToken);

        if (codeFile == null)
        {
            throw new EntityNotFoundException("Code file not found");
        }

        codeFile.Name = dto.Name;
        codeFile.Text = dto.Text;
        codeFile.Language = LanguagesExtensions.StringToLanguage(dto.Language);
        codeFile.ParentId = dto.ParentId;
        codeFile.LastModifiedDateUtc = DateTime.UtcNow;
        codeFile.LastModifiedById = GlobalUser.Id;

        var entity = await _codeFilesRepository.UpdateAsync(codeFile, cancellationToken);

        return _mapper.Map<CodeFileDto>(entity);
    }
}
