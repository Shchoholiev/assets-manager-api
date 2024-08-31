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
    Task SendEmailVerificationAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifies the user's email using the provided token.
    /// </summary>
    /// <param name="token">The email verification token.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task VerifyEmailAsync(string token, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a password reset email with a token to the user's email address.
    /// </summary>
    /// <param name="email">The email address of the user requesting a password reset.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Resets the user's password using the provided token and new password.
    /// </summary>
    /// <param name="token">The password reset token.</param>
    /// <param name="newPassword">The new password to set for the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken);
}