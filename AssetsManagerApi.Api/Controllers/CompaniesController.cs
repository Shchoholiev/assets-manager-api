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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var dto = await _companiesService.CreateCompanyAsync(createDto, cancellationToken);
        // Return Created (201) without relying on route generation for Location header
        return Created(string.Empty, dto);
    }
    
    /// <summary>
    /// Adds a user to the current user's company by email.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("users")]
    public async Task<ActionResult<UserDto>> AddUserToCompanyAsync([FromBody] Models.AddUserRequestModel request, CancellationToken cancellationToken)
    {
        var user = await _companiesService.AddUserToCompanyAsync(request.Email, cancellationToken);
        return Ok(user);
    }

    /// <summary>
    /// Removes a user from the current user's company.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("users/{userId}")]
    public async Task<ActionResult<UserDto>> RemoveUserFromCompanyAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _companiesService.RemoveUserFromCompanyAsync(userId, cancellationToken);
        return Ok(user);
    }

    /// <summary>
    /// Assigns a role to the specified user within the current user's company. Only 'Admin' supported for now.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("users/{userId}/roles")]
    public async Task<ActionResult<UserDto>> AssignCompanyUserRoleAsync(string userId, [FromBody] Models.RoleRequestModel request, CancellationToken cancellationToken)
    {
        if (!string.Equals(request.RoleName, "Admin", StringComparison.OrdinalIgnoreCase))
            return BadRequest($"Role '{request.RoleName}' not supported.");

        var user = await _companiesService.AssignCompanyUserRoleAsync(userId, request.RoleName, cancellationToken);
        return Ok(user);
    }
}
