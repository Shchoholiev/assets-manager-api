using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AssetsManagerApi.Infrastructure.Services;

public class OpenAIService(
    ILogger<OpenAIService> logger,
    HttpClient httpClient
) : IGenerativeAiService
{
    private readonly ILogger _logger = logger;

    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<CodeAssetDto>> SelectRelevantCodeAssets(
        string projectDescription,
        IEnumerable<CodeAssetDto> assets,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Selecting relevant code assets for a project.");

        List<object> messages = [
            new
            {
                role = "developer",
                content = "You are a helpful assistant that selects relevant code assets based on project description."
            }
        ];
        messages.AddRange(GetRelevantCodeAssetsSelectionExamples());

        messages.Add(new
        {
            role = "user",
            content =
                $"""
                Project Description:
                {projectDescription}

                Available Code Assets:
                {PreprocessCodeAssets(assets)}


                Return a list of relevant code assets as a JSON array based on the project description.
                """
        });

        var request = new
        {
            model = "gpt-4o-mini",
            messages = messages,
            response_format = new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = "array_of_code_asset_ids",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            ids = new
                            {
                                type = "array",
                                description = "An array of code asset ids represented as strings.",
                                items = new
                                {
                                    type = "string"
                                }
                            }
                        },
                        required = new[] { "ids" },
                        additionalProperties = false
                    },
                    strict = true
                }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("/v1/chat/completions", request, cancellationToken);
        _logger.LogInformation("Open AI Response status code: {statusCode}", response.StatusCode);

        var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

        var responseObject = JObject.Parse(jsonString);
        var ids = responseObject["choices"]
            ?.Select(choice => choice["message"]?["content"]?.ToString())
            .Where(content => !string.IsNullOrEmpty(content))
            .SelectMany(content =>
            {
                JObject innerObject = JObject.Parse(content);
                return innerObject["ids"]?.Select(id => id.ToString()) ?? Enumerable.Empty<string>();
            })
            .ToList();

        var selectedAssets = assets.Where(x => ids?.Contains(x.Id.ToString()) ?? false).ToList();

        _logger.LogInformation("Selected {count} code assets", selectedAssets.Count);
        
        return selectedAssets;
    }

    private static string PreprocessCodeAssets(IEnumerable<CodeAssetDto> codeAssets)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var strippedAssets = codeAssets.Select(x => new { x.Id, x.Name, x.Description });
        return JsonSerializer.Serialize(strippedAssets, options);
    }

    private static List<object> GetRelevantCodeAssetsSelectionExamples()
    {
        List<object> firstExample = [
            new
            {
                role = "user",
                content =
                    """
                    Project Description:
                    I am developing a mortgage calculation microservice. It should accurately calculate mortgage details and generate statements.

                    Available Code Assets:
                    [
                        {
                            "id": "c5212788-0454-4269-a1da-23440509389c",
                            "name": "ATM Integration Service",
                            "description": "Connects ATM networks with the core banking system."
                        },
                        {
                            "id": "3f6a4167-916f-4bc8-9002-9cf18c664d67",
                            "name": "Transaction Processing Engine",
                            "description": "Processes and records financial transactions reliably."
                        },
                        {
                            "id": "b73c177f-c73d-4d90-bcae-95e9a6f2c0d2",
                            "name": "Electronic Statement Generator",
                            "description": "Creates electronic statements for customer accounts."
                        },
                        {
                            "id": "2deca278-2f36-4c1d-84dd-9e07e48f75af",
                            "name": "Data Caching Service",
                            "description": "Implements caching to speed up data retrieval processes."
                        },
                        {
                            "id": "baa059d9-6c33-48e7-9a29-a101c210d165",
                            "name": "Mortgage Calculation Service",
                            "description": "Calculates mortgage details based on user inputs."
                        }
                    ]


                    Return a list of relevant code assets as a JSON array based on the project description.
                    """
            },
            new
            {
                role = "assistant",
                content =
                    """
                    {
                        "ids": [
                            "baa059d9-6c33-48e7-9a29-a101c210d165",
                            "b73c177f-c73d-4d90-bcae-95e9a6f2c0d2"
                        ]
                    }
                    """
            }
        ];

        List<object> secondExample = [
            new
            {
                role = "user",
                content =
                    """
                    Project Description:
                    I'm enhancing our ATM network by integrating a microservice that synchronizes ATM transactions in real time.

                    Available Code Assets:
                    [
                        {
                            "id": "baa059d9-6c33-48e7-9a29-a101c210d165",
                            "name": "Mortgage Calculation Service",
                            "description": "Calculates mortgage details based on user inputs."
                        },
                        {
                            "id": "c5212788-0454-4269-a1da-23440509389c",
                            "name": "ATM Integration Service",
                            "description": "Connects ATM networks with the core banking system."
                        },
                        {
                            "id": "3f6a4167-916f-4bc8-9002-9cf18c664d67",
                            "name": "Transaction Processing Engine",
                            "description": "Processes and records financial transactions reliably."
                        },
                        {
                            "id": "7e1b5e83-11c1-47f8-a4be-9e21e245228d",
                            "name": "Real-Time Analytics Dashboard",
                            "description": "Displays live analytics data for monitoring performance."
                        },
                        {
                            "id": "2deca278-2f36-4c1d-84dd-9e07e48f75af",
                            "name": "Data Caching Service",
                            "description": "Implements caching to speed up data retrieval processes."
                        }
                    ]


                    Return a list of relevant code assets as a JSON array based on the project description.
                    """
            },
            new
            {
                role = "assistant",
                content =
                    """
                    {
                        "ids": [
                            "c5212788-0454-4269-a1da-23440509389c",
                            "3f6a4167-916f-4bc8-9002-9cf18c664d67"
                        ]
                    }
                    """
            }
        ];

        return [.. firstExample, .. secondExample];
    }
}
