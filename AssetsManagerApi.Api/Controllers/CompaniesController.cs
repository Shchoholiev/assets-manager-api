using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.CreateDto;
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
    
    /// <summary>
    /// Creates a new company and assigns the current user as its administrator.
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompanyAsync([FromBody] CompanyCreateDto createDto, CancellationToken cancellationToken)
    {
        var dto = await _companiesService.CreateCompanyAsync(createDto, cancellationToken);
        // Return Created (201) without relying on route generation for Location header
        return Created(string.Empty, dto);
    }
}
