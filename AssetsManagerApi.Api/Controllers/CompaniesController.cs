using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

/// <summary>
/// Controller for managing companies.
/// </summary>
[Route("companies")]
public class CompaniesController(ICompaniesService companiesService) : ApiController
{
    private readonly ICompaniesService _companiesService = companiesService;

    /// <summary>
    /// Retrieves the user's company.
    /// </summary>
    /// <returns>The user's company details.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpGet]
    public async Task<ActionResult<CompanyDto>> GetUsersCompanyAsync(CancellationToken cancellationToken)
    {
        return await _companiesService.GetUsersCompanyAsync(cancellationToken);
    }
}
