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

    /// <summary>
    /// Generates an email verification token and sends a verification email to the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the email was sent successfully; otherwise, false.</returns>
    Task<bool> SendEmailVerificationAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifies the user's email using the provided token.
    /// </summary>
    /// <param name="token">The email verification token.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the email verification was successful; otherwise, false.</returns>
    Task<bool> VerifyEmailAsync(string token, CancellationToken cancellationToken);
}