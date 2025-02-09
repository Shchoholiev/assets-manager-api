using System.Net;
using System.Net.Http.Json;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.UpdateDto;

namespace AssetsManagerApi.IntegrationTests.Tests;

public class StartProjectsControllerTests(TestingFactory<Program> factory) 
    : TestsBase(factory, "start-projects")
{
    [Fact]
    public async Task CreateStartProject_ValidInput_ReturnsAssets()
    {
        // Arrange
        var request = new StartProjectCreateDto
        {
            Prompt = "I'm building a virtual assistant feature to handle customer inquiries with " + 
                     "an AI-driven solution, integrating chat support for smoother interaction."
        };
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", request);
        var startProject = await response.Content.ReadFromJsonAsync<StartProjectDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(startProject);
        Assert.NotNull(startProject.CodeAssets);
        
        // Assert that there are exactly 2 assets
        Assert.Equal(2, startProject.CodeAssets.Count);

        // Assert that the assets have the expected IDs
        Assert.Contains(startProject.CodeAssets, asset => asset.Id.ToString() == "4b32587b-5e64-435c-9ba8-69740845bd5a");
        Assert.Contains(startProject.CodeAssets, asset => asset.Id.ToString() == "c4c8ec92-71cb-46d8-9b65-97c21aa6681f");
    }

    [Fact]
    public async Task CreateStartProject_UserWithoutCompany_Returns403Forbidden()
    {
        // Arrange
        var request = new StartProjectCreateDto
        {
            Prompt = "I'm building a virtual assistant feature to handle customer inquiries with " + 
                     "an AI-driven solution, integrating chat support for smoother interaction."
        };
        await LoginAsync("no-company@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateStartProject_MissingPrompt_Returns400BadRequest()
    {
        // Arrange
        var request = new StartProjectCreateDto();
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateStartProject_NotAuthorized_Returns401Unauthorized()
    {
        // Arrange
        var request = new StartProjectCreateDto();

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #region CreateCodeFile

    [Fact]
    public async Task CreateCodeFile_ValidInput_Returns201WithCodeFile()
    {
        // Arrange
        var codeFile = new CodeFileCreateDto
        {
            Text = "Console.WriteLine(\"Hello, World!\");",
            Language = "Csharp",
            Name = "HelloWorld.cs",
            ParentId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688aac5"
        };
        var startProjectId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{startProjectId}/code-files", codeFile);
        var stringContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response content: {stringContent}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdCodeFile = await response.Content.ReadFromJsonAsync<CodeFileDto>();
        Assert.NotNull(createdCodeFile);
        Assert.Equal(codeFile.Name, createdCodeFile.Name);
        Assert.Equal(codeFile.Language, createdCodeFile.Language);
        Assert.Equal(codeFile.Text, createdCodeFile.Text);
    }

    #endregion


    #region UpdateCodeFile

    [Fact]
    public async Task UpdateCodeFile_ValidInput_Returns201WithCodeFile()
    {
        // Arrange
        var codeFileId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f618aac0";
        var codeFile = new CodeFileUpdateDto
        {
            Id = codeFileId,
            Text = "Console.WriteLine(\"Hello, World!\");",
            Language = "Csharp",
            Name = "HelloWorld.cs",
            ParentId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688aac5"
        };
        var startProjectId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{startProjectId}/code-files/{codeFileId}", codeFile);
        var stringContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response content: {stringContent}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedCodeFile = await response.Content.ReadFromJsonAsync<CodeFileDto>();
        Assert.NotNull(updatedCodeFile);
        Assert.Equal(codeFile.Name, updatedCodeFile.Name);
        Assert.Equal(codeFile.Language, updatedCodeFile.Language);
        Assert.Equal(codeFile.Text, updatedCodeFile.Text);
    }

    #endregion
}