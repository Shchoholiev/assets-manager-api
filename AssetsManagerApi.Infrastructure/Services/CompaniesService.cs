using AssetsManagerApi.Application.Exceptions;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Global;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;

namespace AssetsManagerApi.Infrastructure.Services;

public class CompaniesService : ICompaniesService
{
    private readonly IMapper _mapper;

    private readonly ICompaniesRepository _companiesRepository;

    public CompaniesService(ICompaniesRepository companiesRepository, IMapper mapper)
    {
        _mapper = mapper;
        _companiesRepository = companiesRepository;
    }

    public async Task<CompanyDto> GetUsersCompanyAsync(CancellationToken cancellationToken)
    {
        if (GlobalUser.CompanyId == null)
        {
            throw new EntityNotFoundException("User does not have company");
        }

        var entity = await this._companiesRepository.GetOneAsync(GlobalUser.CompanyId, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException("User's company not found");
        }

        var dto = _mapper.Map<CompanyDto>(entity);

        return dto;
    }
}
