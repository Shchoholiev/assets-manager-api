using AssetsManagerApi.Application.IServices.Identity;
using AssetsManagerApi.Application.Models.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

[Route("tokens")]
public class TokensController(IUserManager userManager) : ApiController
{
    private readonly IUserManager _userManager = userManager;

    [HttpPost("refresh")]
    public async Task<ActionResult<TokensModel>> RefreshAccessTokenAsync([FromBody] TokensModel tokensModel, CancellationToken cancellationToken)
    {
        var refreshedTokens = await _userManager.RefreshAccessTokenAsync(tokensModel, cancellationToken);
        return Ok(refreshedTokens);
    }
}
