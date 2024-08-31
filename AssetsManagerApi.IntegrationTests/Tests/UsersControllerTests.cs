using System.Net;
using System.Net.Http.Json;
using AssetsManagerApi.Api.Models;
using AssetsManagerApi.Application.Models.Identity;

namespace AssetsManagerApi.IntegrationTests.Tests;

public class UsersControllerTests(TestingFactory<Program> factory) 
    : TestsBase(factory, "users")
{
    [Fact]
    public async Task RegisterAsync_ValidInput_ReturnsTokens()
    {
        // Arrange
        var register = new Register
        {
            Email = "serhii.shchoholiev@nure.ua",
            Password = "Yuiop12345",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/register", register);
        var tokens = await response.Content.ReadFromJsonAsync<TokensModel>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tokens);
        Assert.NotNull(tokens.AccessToken);
        Assert.NotNull(tokens.RefreshToken);
    }

    [Fact]
    public async Task RegisterAsync_ExistingEmail_Returns409Conflict()
    {
        // Arrange
        var register = new Register
        {
            Email = "test@gmail.com",
            Password = "Yuiop12345",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/register", register);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task RegisterAsync_InvalidEmail_Returns422UnprocessableEntity()
    {
        // Arrange
        var register = new Register
        {
            Email = "gmail.com",
            Password = "Yuiop12345",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/register", register);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task LoginAsync_ValidInput_ReturnsTokens()
    {
        // Arrange
        var login = new Login
        {
            Email = "test@gmail.com",
            Password = "Yuiop12345"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var tokens = await response.Content.ReadFromJsonAsync<TokensModel>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tokens);
        Assert.NotNull(tokens.AccessToken);
        Assert.NotNull(tokens.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_IncorrectPassword_Returns400BadRequest()
    {
        // Arrange
        var login = new Login
        {
            Email = "test@gmail.com",
            Password = "incorrect"
        };

            // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_Returns422UnprocessableEntity()
    {
        // Arrange
        var login = new Login
        {
            Email = "mail.com",
            Password = "Yuiop12345"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task LoginAsync_NonExistingEmail_Returns404NotFound()
    {
        // Arrange
        var login = new Login
        {
            Email = "notFound@gmail.com",
            Password = "Yuiop12345"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    #region VerifyEmail

    [Fact]
    public async Task VerifyEmailAsync_ValidToken_ReturnsOk()
    {
        // Arrange
        var validToken = "valid-token";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/verify?token={validToken}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task VerifyEmailAsync_InvalidToken_Returns404NotFound()
    {
        // Arrange
        var invalidToken = "invalid-token";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/verify?token={invalidToken}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task VerifyEmailAsync_ExpiredToken_Returns410Gone()
    {
        // Arrange
        var expiredToken = "expired-token"; 

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/verify?token={expiredToken}");

        // Assert
        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
    }

    [Fact]
    public async Task VerifyEmailAsync_NoTokenProvided_ReturnsBadRequest()
    {
        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/verify");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    [Fact]
    public async Task RequestPasswordReset_ValidEmail_ReturnsOk()
    {
        // Arrange
        var requestModel = new ResetPasswordRequestModel { Email = "anotherreset@example.com" };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/password-reset", requestModel);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RequestPasswordReset_InvalidEmail_ReturnsNotFound()
    {
        // Arrange
        var requestModel = new ResetPasswordRequestModel { Email = "nonexistentuser@example.com" };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/password-reset", requestModel);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RequestPasswordReset_NoEmailProvided_ReturnsBadRequest()
    {
        // Arrange
        var requestModel = new ResetPasswordRequestModel();

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/password-reset", requestModel);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Test for Reset Password

    [Fact]
    public async Task ResetPassword_ValidToken_ReturnsOk()
    {
        // Arrange
        var resetModel = new ResetPasswordModel
        {
            Token = "valid-reset-token",
            NewPassword = "NewSecurePassword123!"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/password-reset", resetModel);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ReturnsNotFound()
    {
        // Arrange
        var resetModel = new ResetPasswordModel
        {
            Token = "invalid-reset-token",
            NewPassword = "NewSecurePassword123!"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/password-reset", resetModel);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_ExpiredToken_ReturnsGone()
    {
        // Arrange
        var resetModel = new ResetPasswordModel
        {
            Token = "expired-reset-token",
            NewPassword = "NewSecurePassword123!"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/password-reset", resetModel);

        // Assert
        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_NoTokenProvided_ReturnsBadRequest()
    {
        // Arrange
        var resetModel = new ResetPasswordModel
        {
            Token = "",
            NewPassword = "NewSecurePassword123!"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/password-reset", resetModel);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}