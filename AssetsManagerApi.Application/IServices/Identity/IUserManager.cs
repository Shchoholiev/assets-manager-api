

using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Identity;

namespace AssetsManagerApi.Application.IServices.Identity;

public interface IUserManager
{
    Task<TokensModel> RegisterAsync(Register register, CancellationToken cancellationToken);

    Task<TokensModel> LoginAsync(Login login, CancellationToken cancellationToken);

    Task<TokensModel> RefreshAccessTokenAsync(TokensModel tokensModel, CancellationToken cancellationToken);

    Task<UserDto> AddToRoleAsync(string roleName, string userId, CancellationToken cancellationToken);

    Task<UserDto> RemoveFromRoleAsync(string roleName, string userId, CancellationToken cancellationToken);
}
