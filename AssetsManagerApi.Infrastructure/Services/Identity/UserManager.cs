using AssetsManagerApi.Application.Exceptions;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.IServices.Identity;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Identity;
using AssetsManagerApi.Domain.Entities.Identity;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace AssetsManagerApi.Infrastructure.Services.Identity;

public class UserManager(
    IUsersRepository usersRepository,
    IPasswordHasher passwordHasher,
    ITokensService tokensService,
    IRolesRepository rolesRepository,
    IRefreshTokensRepository refreshTokensRepository,
    IEmailsService emailsService,
    IMapper mapper,
    ILogger<UserManager> logger,
    IConfiguration configuration) : IUserManager
{
    private readonly IUsersRepository _usersRepository = usersRepository;

    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    private readonly ITokensService _tokensService = tokensService;

    private readonly IRolesRepository _rolesRepository = rolesRepository;
    
    private readonly IRefreshTokensRepository _refreshTokensRepository = refreshTokensRepository;

    private readonly IEmailsService _emailsService = emailsService;

    private readonly IMapper _mapper = mapper;

    private readonly ILogger _logger = logger;

    private readonly string _verificationUrl = configuration.GetValue<string>("EmailSettings:VerificationUrl")!;

    private readonly string _passwordResetUrl = configuration.GetValue<string>("EmailSettings:PasswordResetUrl")!;

    public async Task<TokensModel> RegisterAsync(Register register, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Registering user with email: {register.Email}.");

        var userDto = new UserDto 
        { 
            Email = register.Email
        };
        await ValidateUserAsync(userDto, new User(), cancellationToken);

        var role = await this._rolesRepository.GetOneAsync(r => r.Name == "User", cancellationToken);
        var user = new User
        {
            Name = register.Name,
            Email = register.Email,
            Roles = new List<Role> { role },
            PasswordHash = this._passwordHasher.Hash(register.Password),
            CreatedDateUtc = DateTime.UtcNow,
            CreatedById = string.Empty // Default value for all new users
        };

        await this._usersRepository.AddAsync(user, cancellationToken);

        this._logger.LogInformation($"Created user with id: {user.Id}.");

        var refreshToken = await AddRefreshToken(user.Id, cancellationToken);
        var tokens = this.GetUserTokens(user, refreshToken);

        await SendEmailVerificationAsync(user.Id, cancellationToken);

        this._logger.LogInformation($"Registered guest with guest id: {user.Id}.");

        return tokens;
    }

    public async Task<TokensModel> LoginAsync(Login login, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Logging in user with email: {login.Email} and phone: {login.Phone}.");

        // TODO: move to a separate method
        if (!string.IsNullOrEmpty(login.Email))
        {
            ValidateEmail(login.Email);
        }

        var user = await this._usersRepository.GetOneAsync(u => u.Email == login.Email, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException($"User with email: {login.Email} is not found.");
        }

        if (!this._passwordHasher.Check(login.Password, user.PasswordHash))
        {
            throw new InvalidDataException("Invalid password!");
        }

        var refreshToken = await AddRefreshToken(user.Id, cancellationToken);

        var tokens = this.GetUserTokens(user, refreshToken);

        this._logger.LogInformation($"Logged in user with email: {login.Email} and phone: {login.Phone}.");

        return tokens;
    }

    public async Task<TokensModel> RefreshAccessTokenAsync(TokensModel tokensModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Refreshing access token.");

        var principal = _tokensService.GetPrincipalFromExpiredToken(tokensModel.AccessToken);
        var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new EntityNotFoundException("User");
        }

        var userTask = this._usersRepository.GetOneAsync(userId, cancellationToken);

        var refreshTokenModel = await this._refreshTokensRepository
            .GetOneAsync(r => 
                r.Token == tokensModel.RefreshToken 
                && r.CreatedById == userId
                && r.IsDeleted == false, cancellationToken);
        if (refreshTokenModel == null || refreshTokenModel.ExpiryDateUTC < DateTime.UtcNow)
        {
            throw new SecurityTokenExpiredException("Refresh Token expired.");
        }

        // Update Refresh token if it expires in less than 7 days to keep user constantly logged in if he uses the app
        if (refreshTokenModel.ExpiryDateUTC.AddDays(-7) < DateTime.UtcNow)
        {
            await _refreshTokensRepository.DeleteAsync(refreshTokenModel, cancellationToken);
            
            refreshTokenModel = await AddRefreshToken(userId, cancellationToken);
        }

        var user = await userTask;
        var tokens = this.GetUserTokens(user, refreshTokenModel);

        this._logger.LogInformation($"Refreshed access token.");

        return tokens;
    }

    public async Task<UserDto> AddToRoleAsync(string roleName, string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding Role: {roleName} to User with Id: {userId}.");

        var role = await this._rolesRepository.GetOneAsync(r => r.Name == roleName, cancellationToken);
        if (role == null)
        {
            throw new EntityNotFoundException("User");
        }

        var user = await this._usersRepository.GetOneAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException("User");
        }

        user.Roles.Add(role);
        var updatedUser = await this._usersRepository.UpdateUserAsync(user, cancellationToken);
        var userDto = this._mapper.Map<UserDto>(updatedUser);

        this._logger.LogInformation($"Added Role: {roleName} to User with Id: {userId}.");

        return userDto;
    }

    public async Task<UserDto> RemoveFromRoleAsync(string roleName, string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Removing Role: {roleName} from User with Id: {userId}.");

        var role = await this._rolesRepository.GetOneAsync(r => r.Name == roleName, cancellationToken);
        if (role == null)
        {
            throw new EntityNotFoundException("User");
        }

        var user = await this._usersRepository.GetOneAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException("User");
        }

        var deletedRole = user.Roles.Find(x => x.Name == role.Name);
        user.Roles.Remove(deletedRole);

        var updatedUser = await this._usersRepository.UpdateUserAsync(user, cancellationToken);
        var userDto = this._mapper.Map<UserDto>(updatedUser);

        this._logger.LogInformation($"Removed Role: {roleName} from User with Id: {userId}.");

        return userDto;
    }

    public async Task SendEmailVerificationAsync(string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Sending verification email to user with ID {userId}.");

        var user = await _usersRepository.GetOneAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException($"User with Id: {userId} is not found.");
        }

        var token = Guid.NewGuid().ToString();
        user.EmailVerificationToken = token;
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _usersRepository.UpdateUserAsync(user, cancellationToken);

        var verificationLink = $"{_verificationUrl}?token={token}";
        var subject = "Please Confirm Your Email Address";
        
        var body = $@"
            <html>
                <body>
                    <h2>Welcome to Assets Manager, {user.Name}!</h2>
                    <p>Thank you for registering with Assets Manager. To complete your registration, please verify your email address by clicking the link below:</p>
                    <p><a href='{verificationLink}' style='color: #1a73e8;'>Verify Your Email Address</a></p>
                    <p>If you did not sign up for this account, please ignore this email.</p>
                    <p>Best regards,<br/>The Assets Manager Team</p>
                </body>
            </html>";

        await _emailsService.SendEmailAsync(user.Email, subject, body);

        _logger.LogInformation($"Verification email sent to {user.Email}.");
    }

    public async Task VerifyEmailAsync(string token, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Verifying email with token {token}.");

        var user = await _usersRepository.GetOneAsync(
            u => u.EmailVerificationToken == token, cancellationToken);
        if (user == null)
        {
            _logger.LogError($"User with email verification token: {token} not found.");
            throw new EntityNotFoundException($"User with email verification token: {token} is not found.");
        }

        if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
        {
            _logger.LogError($"Verification token for user {user.Email} has expired.");
            throw new TokenExpiredException("The email verification token has expired.");
        }

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null; 
        user.EmailVerificationTokenExpiry = null;

        await _usersRepository.UpdateUserAsync(user, cancellationToken);

        _logger.LogInformation($"Email verified successfully for user {user.Email}.");
    }


    public async Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Requesting password reset for user with email {email}.");

        var user = await _usersRepository.GetOneAsync(u => u.Email == email, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException($"User with email {email} is not found.");
        }

        var token = Guid.NewGuid().ToString();
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _usersRepository.UpdateUserAsync(user, cancellationToken);

        var resetLink = $"{_passwordResetUrl}?token={token}";
        var subject = "Password Reset Request";
        var body = $@"
            <html>
                <body>
                    <h2>Password Reset for Assets Manager</h2>
                    <p>We received a request to reset the password for your account. If you made this request, please click the link below to reset your password:</p>
                    <p><a href='{resetLink}' style='color: #1a73e8;'>Reset Your Password</a></p>
                    <p>If you did not request a password reset, please ignore this email or contact support if you have any concerns.</p>
                    <p>Best regards,<br/>The Assets Manager Team</p>
                </body>
            </html>";

        await _emailsService.SendEmailAsync(user.Email, subject, body);

        _logger.LogInformation($"Password reset email sent to {user.Email}.");
    }

    public Task ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<RefreshToken> AddRefreshToken(string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding new refresh token for user with Id : {userId}.");

        var refreshToken = new RefreshToken
        {
            Token = _tokensService.GenerateRefreshToken(),
            ExpiryDateUTC = DateTime.UtcNow.AddDays(30),
            CreatedById = userId,
            CreatedDateUtc = DateTime.UtcNow
        };

        await this._refreshTokensRepository.AddAsync(refreshToken, cancellationToken);

        this._logger.LogInformation($"Added new refresh token.");

        return refreshToken;
    }

    private TokensModel GetUserTokens(User user, RefreshToken refreshToken)
    {
        var claims = this.GetClaims(user);
        var accessToken = this._tokensService.GenerateAccessToken(claims);

        this._logger.LogInformation($"Returned new access and refresh tokens.");

        return new TokensModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
        };
    }

    private IEnumerable<Claim> GetClaims(User user)
    {
        var claims = new List<Claim>()
        {
            new (ClaimTypes.Name, user.Name ?? string.Empty),
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Email, user.Email ?? string.Empty),
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new (ClaimTypes.Role, role.Name));
        }

        this._logger.LogInformation($"Returned claims for User with Id: {user.Id}.");

        return claims;
    }

    private async Task ValidateUserAsync(UserDto userDto, User user, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(userDto.Email)) 
        {
            ValidateEmail(userDto.Email);
            if (userDto.Email != user.Email 
                && await this._usersRepository.ExistsAsync(x => x.Email == userDto.Email, cancellationToken))
            {
                throw new EntityAlreadyExistsException("User", "Email", userDto.Email);
            }
        }
    }

    private void ValidateEmail(string email)
    {
        string regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        if (!Regex.IsMatch(email, regex))
        {
            throw new InvalidEmailException(email);
        }
    }
}