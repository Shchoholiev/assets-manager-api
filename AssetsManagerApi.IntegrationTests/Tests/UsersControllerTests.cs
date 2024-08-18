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
            Email = "register@gmail.com",
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
}