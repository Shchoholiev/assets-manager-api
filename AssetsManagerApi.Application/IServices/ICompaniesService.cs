using AssetsManagerApi.Application.Models.Dto;

namespace AssetsManagerApi.Application.IServices;

public interface ICompaniesService
{
    Task<CompanyDto> GetUsersCompanyAsync(CancellationToken cancellationToken);
}

