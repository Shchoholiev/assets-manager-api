using System.Net.Http.Headers;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Compilation;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace AssetsManagerApi.Infrastructure.Services;

public class CompilationService(
    ILogger<CompilationService> logger,
    HttpClient httpClient,
    IConfiguration configuration
) : ICompilationService
{
    private readonly ILogger _logger = logger;

    private readonly HttpClient _httpClient = httpClient;

    private readonly IConfiguration _configuration = configuration;
    
    public async Task<CompilationResponse> CompileDotNetProjectAsync(byte[] projectZip, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting compilation of .NET project.");

        using var zipStream = new MemoryStream(projectZip);
        using var zipContentStream = new StreamContent(zipStream);
        zipContentStream.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

        var compilerUrl = _configuration.GetValue<string>("CompilationApis:DotNetCompilationUrl")!;
        var compilerApiKey = _configuration.GetValue<string>("CompilationApis:DotNetCompilationApiKey")!;
        
        _logger.LogInformation("Sending POST request to compilation service.");
        var response = await _httpClient.PostAsync(
            $"{compilerUrl}?code={compilerApiKey}", 
            zipContentStream, 
            cancellationToken);

        _logger.LogInformation("Received response from compilation service. Status Code: {StatusCode}", response.StatusCode);

        var compilationResult = await response.Content.ReadFromJsonAsync<CompilationResponse>(cancellationToken);

        var compilationResultString = await response.Content.ReadAsStringAsync(cancellationToken);


        _logger.LogInformation(compilationResultString);

        _logger.LogInformation("Compilation completed.");

        return compilationResult ?? throw new InvalidOperationException("Compilation response was null.");
    }
}
