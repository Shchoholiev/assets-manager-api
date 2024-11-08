using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Paging;
using AutoMapper;

namespace AssetsManagerApi.Infrastructure.Services;

public class TagsService : ITagsService
{
    private readonly ITagsRepository _tagsRepository;

    private readonly IMapper _mapper;

    public TagsService(ITagsRepository tagsRepository, IMapper mapper)
    {
        _mapper = mapper;
        _tagsRepository = tagsRepository;
    }

    public async Task<PagedList<TagDto>> GetPopularTagsPage(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._tagsRepository.GetTagsOrderedByUsageAsync(pageNumber, pageSize, cancellationToken);
        var dtos = _mapper.Map<List<TagDto>>(entities);
        var totalCount = await this._tagsRepository.GetCountAsync(cancellationToken);
        return new PagedList<TagDto>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<PagedList<TagDto>> GetTagsPage(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._tagsRepository.GetPageAsync(pageNumber, pageSize, cancellationToken);
        var dtos = _mapper.Map<List<TagDto>>(entities);
        var totalCount = await this._tagsRepository.GetCountAsync(cancellationToken);
        return new PagedList<TagDto>(dtos, pageNumber, pageSize, totalCount);
    }
}
