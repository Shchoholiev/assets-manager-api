using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Models.UpdateDto;
using AssetsManagerApi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

/// <summary>
/// Controller for managing start projects.
/// </summary>
[Route("start-projects")]
public class StartProjectsController(
    IStartProjectsService startProjectsService
) : ApiController
{
    private readonly IStartProjectsService _startProjectsService = startProjectsService;
    
    /// <summary>
    /// Initializes a new start project and finds assets based on the provided project description.
    /// </summary>
    /// <param name="startProject">Prompt with project description</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A newly created start project with associated assets.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("")]
    [Produces("application/json")]
    public async Task<ActionResult<StartProjectDto>> CreateStartProjectAsync([FromBody] StartProjectCreateDto startProject, CancellationToken cancellationToken)
    {
        return await _startProjectsService.CreateStartProjectAsync(startProject, cancellationToken);
    }

    /// <summary>
    /// Creates a new code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileDto">The details of the code file to create.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created code file.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{startProjectId}/code-files")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeFileDto>> CreateCodeFileAsync(
        string startProjectId, // for future use
        [FromBody] CodeFileCreateDto codeFileDto,
        CancellationToken cancellationToken)
    {
        var createdCodeFile = await _startProjectsService.CreateCodeFileAsync(startProjectId, codeFileDto, cancellationToken);
        return Created("", createdCodeFile);
    }

    /// <summary>
    /// Updates an existing code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileId">The identifier of the code file to update.</param>
    /// <param name="codeFileDto">The updated code file details.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated code file.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPut("{startProjectId}/code-files/{codeFileId}")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeFileDto>> UpdateCodeFileAsync(
        string startProjectId, // for future use
        string codeFileId,
        [FromBody] CodeFileUpdateDto codeFileDto,
        CancellationToken cancellationToken)
    {
        var updatedCodeFile = await _startProjectsService.UpdateCodeFileAsync(startProjectId, codeFileId, codeFileDto, cancellationToken);
        return Ok(updatedCodeFile);
    }

    /// <summary>
    /// Deletes an existing code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileId">The identifier of the code file to delete.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if the deletion is successful.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpDelete("{startProjectId}/code-files/{codeFileId}")]
    [Produces("application/json")]
    public async Task<ActionResult> DeleteCodeFileAsync(
        string startProjectId, // for future use
        string codeFileId,
        CancellationToken cancellationToken)
    {
        await _startProjectsService.DeleteCodeFileAsync(startProjectId, codeFileId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Creates a new folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderDto">The details of the folder to create.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created folder.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{startProjectId}/folders")]
    [Produces("application/json")]
    public async Task<ActionResult<FolderDto>> CreateFolderAsync(
        string startProjectId, // for future use
        [FromBody] FolderCreateDto folderDto,
        CancellationToken cancellationToken)
    {
        var folder = await _startProjectsService.CreateFolderAsync(startProjectId, folderDto, cancellationToken);
        return Created("", folder);
    }

    /// <summary>
    /// Updates an existing folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderId">The identifier of the folder to update.</param>
    /// <param name="folderDto">The updated folder details.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated folder.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPut("{startProjectId}/folders/{folderId}")]
    [Produces("application/json")]
    public async Task<ActionResult<FolderDto>> UpdateFolderAsync(
        string startProjectId, // for future use
        string folderId,
        [FromBody] FolderUpdateDto folderDto,
        CancellationToken cancellationToken)
    {
        var folder = await _startProjectsService.UpdateFolderAsync(startProjectId, folderId, folderDto, cancellationToken);
        return Ok(folder);
    }

    /// <summary>
    /// Deletes an existing folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderId">The identifier of the folder to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>No content if the deletion is successful.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpDelete("{startProjectId}/folders/{folderId}")]
    [Produces("application/json")]
    public async Task<ActionResult> DeleteFolderAsync(
        string startProjectId, // for future use
        string folderId,
        CancellationToken cancellationToken)
    {
        await _startProjectsService.DeleteFolderAsync(startProjectId, folderId, cancellationToken);
        return NoContent();
    }


    /// <summary>
    /// Combines the code assets of a start project into a single project.
    /// </summary>
    /// <param name="id">The ID of the start project to combine.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Code Asset similar to Code Asset endpoints</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{id}/combine")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeAssetDto>> CombineStartProjectAsync(string id, CancellationToken cancellationToken)
    {
        return await _startProjectsService.CombineStartProjectAsync(id, cancellationToken);
    }

    [Authorize(Roles = "Enterprise")]
    [HttpGet("{id}/combined-asset")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeAssetDto>> GetCombinedAssetAsync(string id, CancellationToken cancellationToken)
    {
        return Ok(GetDummyCombinedCodeAsset());
    }

    /// <summary>
    /// Compiles a start project and returns the compilation result.
    /// </summary>
    /// <param name="id">The ID of the start project to compile.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The result of the compilation, including any errors if they occurred.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{id}/compile")]
    [Produces("application/json")]
    public async Task<ActionResult<CompilationResult>> CompileStartProjectAsync(string id, CancellationToken cancellationToken)
    {
        var dummy = new CompilationResult
        {
            Error = null
        };
        return Ok(dummy);
    }

    /// <summary>
    /// Downloads the start project as a zip file.
    /// </summary>
    /// <param name="id">The ID of the start project to download.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A zip file containing the project assets.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpGet("{id}/download")]
    [Produces("application/zip")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<FileContentResult> DownloadStartProjectAsync(string id, CancellationToken cancellationToken)
    {
        var zipFileBytes = CreateDummyZipFile();
        var fileName = "start-project.zip";

        return File(zipFileBytes, "application/zip", fileName);
    }

    private byte[] CreateDummyZipFile()
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            var demoFile = archive.CreateEntry("dummy.txt");
            using var entryStream = demoFile.Open();
            using var streamWriter = new StreamWriter(entryStream);
            streamWriter.Write("This is a dummy file for testing.");
        }

        return memoryStream.ToArray();
    }

    private CodeAssetDto GetDummyCombinedCodeAsset() 
    {
        var combinedAsset = new CodeAssetDto
        {
            Id = "d2fbe8a3-74cb-4d34-86b8-5c5f8764e2a3",
            Name = "AI Customer Support Suite",
            Description = "A virtual assistant that handles customer inquiries with an AI-driven solution, integrating chat support for smoother interaction.",
            Tags =
            [
                new TagDto { Id = "40404040-4040-4040-4040-404040404040", Name = "Integration" },
                new TagDto { Id = "20202020-2020-2020-2020-202020202020", Name = "Analytics" }
            ],
            AssetType = AssetTypes.Corporate,
            Language = "CSharp",
            UserName = "Start Project Generator",
            RootFolder = new FolderDto
            {
                Id = "91f48b8c-86b7-49b5-9054-85c362adfd7d",
                Name = "AICustomerSupportSuite",
                Type = FileType.Folder,
                Items =
                [
                    new FolderDto
                    {
                        Id = "controllers-folder",
                        Name = "Controllers",
                        Type = FileType.Folder,
                        Items = new List<FileSystemNodeDto>
                        {
                            new CodeFileDto
                            {
                                Id = "virtual-assistant-controller",
                                Name = "VirtualAssistantController.cs",
                                Type = FileType.CodeFile,
                                Text = "using System.Threading.Tasks;\nusing Microsoft.AspNetCore.Mvc;\nusing VirtualAssistantService.Models;\nusing VirtualAssistantService.Services;\n\nnamespace VirtualAssistantService.Controllers\n{\n    [ApiController]\n    [Route(\"api/[controller]\")]\n    public class VirtualAssistantController : ControllerBase\n    {\n        private readonly IVirtualAssistantService _assistantService;\n\n        public VirtualAssistantController(IVirtualAssistantService assistantService)\n        {\n            _assistantService = assistantService;\n        }\n\n        [HttpPost(\"inquiry\")]\n        public async Task<ActionResult<AssistantResponse>> PostInquiry([FromBody] Inquiry inquiry)\n        {\n            var response = await _assistantService.HandleInquiryAsync(inquiry);\n            if (response == null)\n            {\n                return BadRequest(\"Unable to process inquiry\");\n            }\n            return Ok(response);\n        }\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "chat-support-controller",
                                Name = "ChatSupportController.cs",
                                Type = FileType.CodeFile,
                                Text = "using Microsoft.AspNetCore.Mvc;\nusing System.Threading.Tasks;\nusing ChatSupportIntegration.Services;\nusing ChatSupportIntegration.Models;\n\nnamespace ChatSupportIntegration.Controllers\n{\n    [ApiController]\n    [Route(\"api/[controller]\")]\n    public class ChatSupportController : ControllerBase\n    {\n        private readonly IChatService _chatService;\n\n        public ChatSupportController(IChatService chatService)\n        {\n            _chatService = chatService;\n        }\n\n        [HttpGet(\"{sessionId}\")]\n        public async Task<ActionResult<ChatSession>> GetChatSession(string sessionId)\n        {\n            var session = await _chatService.GetChatSessionAsync(sessionId);\n            if (session == null) \n                return NotFound();\n            \n            return Ok(session);\n        }\n\n        [HttpPost]\n        public async Task<ActionResult<ChatMessage>> PostChatMessage([FromBody] ChatMessage message)\n        {\n            var processedMessage = await _chatService.ProcessChatMessageAsync(message);\n            return Ok(processedMessage);\n        }\n    }\n}\n",
                                Language = "CSharp"
                            }
                        },
                    },
                    new FolderDto
                    {
                        Id = "services-folder",
                        Name = "Services",
                        Type = FileType.Folder,
                        Items = new List<FileSystemNodeDto>
                        {
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "ChatService.cs",
                                Type = FileType.CodeFile,
                                Text = "using System;\nusing System.Threading.Tasks;\nusing ChatSupportIntegration.Models;\nusing ChatSupportIntegration.Infrastructure;\n\nnamespace ChatSupportIntegration.Services\n{\n    public interface IChatService\n    {\n        Task<ChatSession> GetChatSessionAsync(string sessionId);\n        Task<ChatMessage> ProcessChatMessageAsync(ChatMessage message);\n    }\n\n    public class ChatService : IChatService\n    {\n        private readonly IIntegrationService _integrationService;\n        private readonly IChatLogger _chatLogger;\n\n        public ChatService(IIntegrationService integrationService, IChatLogger chatLogger)\n        {\n            _integrationService = integrationService;\n            _chatLogger = chatLogger;\n        }\n\n        // In a production environment, persistent storage like a database would be used.\n        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, ChatSession> Sessions =\n            new System.Collections.Concurrent.ConcurrentDictionary<string, ChatSession>();\n\n        public async Task<ChatSession> GetChatSessionAsync(string sessionId)\n        {\n            Sessions.TryGetValue(sessionId, out ChatSession session);\n            return await Task.FromResult(session);\n        }\n\n        public async Task<ChatMessage> ProcessChatMessageAsync(ChatMessage message)\n        {\n            if (string.IsNullOrEmpty(message.SessionId))\n            {\n                // Create a new session if none exists.\n                message.SessionId = Guid.NewGuid().ToString();\n                var newSession = new ChatSession { SessionId = message.SessionId };\n                Sessions.TryAdd(newSession.SessionId, newSession);\n            }\n\n            Sessions.AddOrUpdate(message.SessionId,\n                new ChatSession { SessionId = message.SessionId, Messages = new System.Collections.Generic.List<ChatMessage> { message } },\n                (key, existingSession) =>\n                {\n                    existingSession.Messages.Add(message);\n                    return existingSession;\n                });\n\n            _chatLogger.LogInfo($\"Processing message for session {message.SessionId}\");\n            \n            // Integrate with the external live chat provider\n            var response = await _integrationService.SendMessageToLiveChatAsync(message);\n            \n            if (response != null)\n            {\n                Sessions.AddOrUpdate(message.SessionId,\n                    new ChatSession { SessionId = message.SessionId, Messages = new System.Collections.Generic.List<ChatMessage> { response } },\n                    (key, existingSession) =>\n                    {\n                        existingSession.Messages.Add(response);\n                        return existingSession;\n                    });\n            }\n\n            return response ?? message;\n        }\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "IntegrationService.cs",
                                Type = FileType.CodeFile,
                                Text = "using System.Net.Http;\nusing System.Text.Json;\nusing System.Threading.Tasks;\nusing ChatSupportIntegration.Models;\nusing ChatSupportIntegration.Infrastructure;\n\nnamespace ChatSupportIntegration.Services\n{\n    public interface IIntegrationService\n    {\n        Task<ChatMessage> SendMessageToLiveChatAsync(ChatMessage message);\n    }\n\n    public class IntegrationService : IIntegrationService\n    {\n        private readonly HttpClient _httpClient;\n        private readonly IIntegrationAdapter _integrationAdapter;\n\n        public IntegrationService(IIntegrationAdapter integrationAdapter, IHttpClientFactory httpClientFactory)\n        {\n            _integrationAdapter = integrationAdapter;\n            _httpClient = httpClientFactory.CreateClient();\n        }\n\n        public async Task<ChatMessage> SendMessageToLiveChatAsync(ChatMessage message)\n        {\n            // Delegate the external communication to the adapter\n            return await _integrationAdapter.SendChatMessageAsync(message);\n        }\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "IVirtualAssistantService.cs",
                                Type = FileType.CodeFile,
                                Text = "using System.Threading.Tasks;\nusing VirtualAssistantService.Models;\n\nnamespace VirtualAssistantService.Services\n{\n    public interface IVirtualAssistantService\n    {\n        Task<AssistantResponse> HandleInquiryAsync(Inquiry inquiry);\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "VirtualAssistantService.cs",
                                Type = FileType.CodeFile,
                                Text = "using System;\nusing System.Threading.Tasks;\nusing VirtualAssistantService.Infrastructure;\nusing VirtualAssistantService.Models;\n\nnamespace VirtualAssistantService.Services\n{\n    public class VirtualAssistantService : IVirtualAssistantService\n    {\n        private readonly IAiEngineAdapter _aiEngineAdapter;\n\n        public VirtualAssistantService(IAiEngineAdapter aiEngineAdapter)\n        {\n            _aiEngineAdapter = aiEngineAdapter;\n        }\n\n        public async Task<AssistantResponse> HandleInquiryAsync(Inquiry inquiry)\n        {\n            if (string.IsNullOrWhiteSpace(inquiry.Question))\n            {\n                throw new ArgumentException(\"Inquiry question cannot be empty.\");\n            }\n            string aiResponse = await _aiEngineAdapter.GetAssistantResponseAsync(inquiry.Question, inquiry.CustomerId);\n            return new AssistantResponse\n            {\n                InquiryId = inquiry.InquiryId,\n                ResponseMessage = aiResponse,\n                ProcessedAt = DateTime.UtcNow\n            };\n        }\n    }\n}\n",
                                Language = "CSharp"
                            },
                        }
                    },
                    new FolderDto
                    {
                        Id = "dummy",
                        Name = "Models",
                        Type = FileType.Folder,
                        Items = new List<FileSystemNodeDto>
                        {
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "ChatMessage.cs",
                                Type = FileType.CodeFile,
                                Text = "using System;\n\nnamespace ChatSupportIntegration.Models\n{\n    public class ChatMessage\n    {\n        public string MessageId { get; set; } = Guid.NewGuid().ToString();\n        public string SessionId { get; set; }\n        public string UserId { get; set; }\n        public string Content { get; set; }\n        public DateTime Timestamp { get; set; } = DateTime.UtcNow;\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "ChatSession.cs",
                                Type = FileType.CodeFile,
                                Text = "using System.Collections.Generic;\n\nnamespace ChatSupportIntegration.Models\n{\n    public class ChatSession\n    {\n        public string SessionId { get; set; }\n        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "Inquiry.cs",
                                Type = FileType.CodeFile,
                                Text = "using System;\n\nnamespace VirtualAssistantService.Models\n{\n    public class Inquiry\n    {\n        public string InquiryId { get; set; } = Guid.NewGuid().ToString();\n        public string CustomerId { get; set; }\n        public string Question { get; set; }\n        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "AssistantResponse.cs",
                                Type = FileType.CodeFile,
                                Text = "using System;\n\nnamespace VirtualAssistantService.Models\n{\n    public class AssistantResponse\n    {\n        public string InquiryId { get; set; }\n        public string ResponseMessage { get; set; }\n        public DateTime ProcessedAt { get; set; }\n    }\n}\n",
                                Language = "CSharp"
                            },
                        }
                    },
                    new FolderDto
                    {
                        Id = "dummy",
                        Name = "Infrastructure",
                        Type = FileType.Folder,
                        Items = new List<FileSystemNodeDto>
                        {
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "ChatLogger.cs",
                                Type = FileType.CodeFile,
                                Text = "using System;\nusing Microsoft.Extensions.Logging;\n\nnamespace ChatSupportIntegration.Infrastructure\n{\n    public interface IChatLogger\n    {\n        void LogInfo(string message);\n        void LogError(string message, Exception ex);\n    }\n\n    public class ChatLogger : IChatLogger\n    {\n        private readonly ILogger<ChatLogger> _logger;\n\n        public ChatLogger(ILogger<ChatLogger> logger)\n        {\n            _logger = logger;\n        }\n\n        public void LogInfo(string message)\n        {\n            _logger.LogInformation(message);\n        }\n\n        public void LogError(string message, Exception ex)\n        {\n            _logger.LogError(ex, message);\n        }\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "IntegrationAdapter.cs",
                                Type = FileType.CodeFile,
                                Text = "using System.Net.Http;\nusing System.Text;\nusing System.Text.Json;\nusing System.Threading.Tasks;\nusing ChatSupportIntegration.Models;\n\nnamespace ChatSupportIntegration.Infrastructure\n{\n    public interface IIntegrationAdapter\n    {\n        Task<ChatMessage> SendChatMessageAsync(ChatMessage message);\n    }\n\n    public class IntegrationAdapter : IIntegrationAdapter\n    {\n        private readonly HttpClient _httpClient;\n\n        public IntegrationAdapter(IHttpClientFactory httpClientFactory)\n        {\n            _httpClient = httpClientFactory.CreateClient();\n        }\n\n        public async Task<ChatMessage> SendChatMessageAsync(ChatMessage message)\n        {\n            // Replace with actual live chat provider endpoint in production\n            string externalEndpoint = \"https://api.livechatprovider.com/send\";\n            \n            var jsonContent = JsonSerializer.Serialize(message);\n            var content = new StringContent(jsonContent, Encoding.UTF8, \"application/json\");\n            \n            var response = await _httpClient.PostAsync(externalEndpoint, content);\n            response.EnsureSuccessStatusCode();\n\n            var responseContent = await response.Content.ReadAsStringAsync();\n            var chatResponse = JsonSerializer.Deserialize<ChatMessage>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });\n            return chatResponse;\n        }\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "IAiEngineAdapter.cs",
                                Type = FileType.CodeFile,
                                Text = "using System.Threading.Tasks;\n\nnamespace VirtualAssistantService.Infrastructure\n{\n    public interface IAiEngineAdapter\n    {\n        Task<string> GetAssistantResponseAsync(string question, string customerId);\n    }\n}\n",
                                Language = "CSharp"
                            },
                            new CodeFileDto
                            {
                                Id = "dummy",
                                Name = "ChatMessage.cs",
                                Type = FileType.CodeFile,
                                Text = "using System;\nusing System.Net.Http;\nusing System.Text;\nusing System.Text.Json;\nusing System.Threading.Tasks;\n\nnamespace VirtualAssistantService.Infrastructure\n{\n    public class AiEngineAdapter : IAiEngineAdapter\n    {\n        private readonly HttpClient _httpClient;\n\n        public AiEngineAdapter(IHttpClientFactory httpClientFactory)\n        {\n            _httpClient = httpClientFactory.CreateClient();\n        }\n\n        public async Task<string> GetAssistantResponseAsync(string question, string customerId)\n        {\n            var requestPayload = new\n            {\n                question = question,\n                customerId = customerId,\n                timestamp = DateTime.UtcNow\n            };\n\n            var json = JsonSerializer.Serialize(requestPayload);\n            var content = new StringContent(json, Encoding.UTF8, \"application/json\");\n            \n            // Replace the URL with the actual endpoint of the AI engine service\n            var response = await _httpClient.PostAsync(\"https://api.aiengineprovider.com/process\", content);\n            response.EnsureSuccessStatusCode();\n\n            var responseContent = await response.Content.ReadAsStringAsync();\n            using (var document = JsonDocument.Parse(responseContent))\n            {\n                if (document.RootElement.TryGetProperty(\"answer\", out var answerElement))\n                {\n                    return answerElement.GetString();\n                }\n            }\n            throw new Exception(\"Invalid response from AI Engine service.\");\n        }\n    }\n}\n",
                                Language = "CSharp"
                            },
                        }
                    },
                    new CodeFileDto
                    {
                        Id = "program-file",
                        Name = "Program.cs",
                        Type = FileType.CodeFile,
                        Text = "using Microsoft.AspNetCore.Hosting;\nusing Microsoft.Extensions.Hosting;\n\nnamespace AICustomerSupportSuite\n{\n    public class Program\n    {\n        public static void Main(string[] args)\n        {\n            CreateHostBuilder(args).Build().Run();\n        }\n\n        public static IHostBuilder CreateHostBuilder(string[] args) =>\n            Host.CreateDefaultBuilder(args)\n                .ConfigureWebHostDefaults(webBuilder =>\n                {\n                    webBuilder.UseStartup<Startup>();\n                });\n    }\n}",
                        Language = "CSharp"
                    },
                    new CodeFileDto
                    {
                        Id = "project-file",
                        Name = "AICustomerSupportSuite.csproj",
                        Type = FileType.CodeFile,
                        Text = """
                            <Project Sdk="Microsoft.NET.Sdk">
                                <ItemGroup>
                                    <PackageReference Include="Microsoft.Azure.Cosmos" Version="3.42.0" />
                                    <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
                                    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
                                    <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
                                    <PackageReference Include="MongoDB.Driver" Version="2.23.1" />
                                </ItemGroup>
                                <PropertyGroup>
                                    <TargetFramework>net8.0</TargetFramework>
                                    <ImplicitUsings>enable</ImplicitUsings>
                                    <Nullable>enable</Nullable>
                                </PropertyGroup>
                            </Project>
                            """,
                        Language = "CSharp"
                    }
                ]
            },
            PrimaryCodeFile = new CodeFileDto
            {
                Id = "program-file",
                Name = "Program.cs",
                Type = FileType.CodeFile,
                Text = "using Microsoft.AspNetCore.Hosting;\nusing Microsoft.Extensions.Hosting;\n\nnamespace AICustomerSupportSuite\n{\n    public class Program\n    {\n        public static void Main(string[] args)\n        {\n            CreateHostBuilder(args).Build().Run();\n        }\n\n        public static IHostBuilder CreateHostBuilder(string[] args) =>\n            Host.CreateDefaultBuilder(args)\n                .ConfigureWebHostDefaults(webBuilder =>\n                {\n                    webBuilder.UseStartup<Startup>();\n                });\n    }\n}",
                Language = "CSharp"
            }
        };
        return combinedAsset;
    }
}
