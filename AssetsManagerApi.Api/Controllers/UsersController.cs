using AssetsManagerApi.Application.IServices.Identity;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

[Route("users")]
public class UsersController(
    IUserManager userManager) : ApiController
{
    private readonly IUserManager _userManager = userManager;

    [HttpPost("register")]
    public async Task<ActionResult<TokensModel>> RegisterAsync([FromBody] Register register, CancellationToken cancellationToken)
    {
        var tokens = await _userManager.RegisterAsync(register, cancellationToken);
        return Ok(tokens);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokensModel>> LoginAsync([FromBody] Login login, CancellationToken cancellationToken)
    {
        var tokens = await _userManager.LoginAsync(login, cancellationToken);
        return Ok(tokens);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{userId}/roles/{roleName}")]
    public async Task<ActionResult<UserDto>> AddToRoleAsync(string roleName, string userId, CancellationToken cancellationToken)
    {
        var userDto = await _userManager.AddToRoleAsync(roleName, userId, cancellationToken);
        return Ok(userDto);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}/roles/{roleName}")]
    public async Task<ActionResult<UserDto>> RemoveFromRoleAsync(string roleName, string userId, CancellationToken cancellationToken)
    {
        var userDto = await _userManager.RemoveFromRoleAsync(roleName, userId, cancellationToken);
        return Ok(userDto);
    }
}
