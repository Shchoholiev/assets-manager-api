using System.Net;
using System.Net.Http.Json;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;

namespace AssetsManagerApi.IntegrationTests.Tests;

public class StartProjectsControllerTests(TestingFactory<Program> factory) 
    : TestsBase(factory, "start-projects")
{
    // TODO: Update Assert as this is used for auth testing
    [Fact]
    public async Task CreateStartProjectAsync_ValidInput_ReturnsAssets()
    {
        // Arrange
        var request = new StartProjectCreateDto
        {
            Prompt = "I am creating a .NET project with JWT authentication",
        };
        await LoginAsync("test@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", request);
        var startProject = await response.Content.ReadFromJsonAsync<StartProjectDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(startProject);
        Assert.NotNull(startProject.CodeAssets);
    }
}