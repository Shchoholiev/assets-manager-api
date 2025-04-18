using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        // messages.AddRange(GetRelevantCodeAssetsSelectionExamples());
        messages.AddRange(GetRelevantCodeAssetsSelectionCoTExamples());

        messages.Add(new
        {
            role = "user",
            content =
                $"""
                Project Description:
                {projectDescription}

                Available Code Assets:
                {PreprocessCodeAssets(assets)}

                Return a list of all relevant code assets as a JSON object with key "ids" and array of string inside <json></json> tag based on the project description. 
                Explain your reasoning step by step before returning the final answer.
                """
        });

        var request = new
        {
            model = "gpt-4o-mini",
            messages = messages,
            // response_format = new
            // {
            //     type = "json_schema",
            //     json_schema = new
            //     {
            //         name = "array_of_code_asset_ids",
            //         schema = new
            //         {
            //             type = "object",
            //             properties = new
            //             {
            //                 ids = new
            //                 {
            //                     type = "array",
            //                     description = "An array of code asset ids represented as strings.",
            //                     items = new
            //                     {
            //                         type = "string"
            //                     }
            //                 }
            //             },
            //             required = new[] { "ids" },
            //             additionalProperties = false
            //         },
            //         strict = true
            //     }
            // }
        };

        var response = await _httpClient.PostAsJsonAsync("/v1/chat/completions", request, cancellationToken);
        _logger.LogInformation("Open AI Response status code: {statusCode}", response.StatusCode);

        var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

        // _logger.LogInformation("Open AI Response: {response}", jsonString);

        var match = Regex.Match(jsonString, @"<json>(.*?)</json>", RegexOptions.Singleline);
        var json = match.Groups[1].Value;

        // _logger.LogInformation("Extracted JSON: {json}", json);

        var unescapedJson = Regex.Unescape(json).Trim();
        var responseObject = JObject.Parse(unescapedJson);
        var ids = responseObject["ids"]?.ToObject<string[]>();

        // For JSON response
        // var responseObject = JObject.Parse(jsonString);
        // var ids = responseObject["choices"]
        //     ?.Select(choice => choice["message"]?["content"]?.ToString())
        //     .Where(content => !string.IsNullOrEmpty(content))
        //     .SelectMany(content =>
        //     {
        //         JObject innerObject = JObject.Parse(content);
        //         return innerObject["ids"]?.Select(id => id.ToString()) ?? Enumerable.Empty<string>();
        //     })
        //     .ToList();

        var selectedAssets = assets.Where(x => ids?.Contains(x.Id.ToString()) ?? false).ToList();

        _logger.LogInformation("Selected {count} code assets", selectedAssets.Count);
        
        return selectedAssets;
    }

    public async Task<string> ReviewCodeOnDrySolidPrinciplesAsync(string code, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reviewing code for DRY and SOLID principles.");

        var messages = new List<object>
        {
            new 
            { 
                role = "developer", 
                content = "You are a seasoned code reviewer specializing in DRY (Don't Repeat Yourself) and SOLID principles. Analyze the code and provide a detailed explanation on how it adheres to these principles and include suggestions for improvement if needed." 
            }
        };

        messages.Add(new 
        { 
            role = "user", 
            content = 
                $"""
                <code>
                {code}
                </code>

                <response template>
                DRY
                1. what to change in code to better align with DRY
                2. each item should be a single paragraph

                SOLID
                1. what to change in code to better align with SOLID
                2. ...
                </response template>

                Task:
                1. Analyze code provided in <code></code> according to DRY / SOLID principles
                2. Return result as defined in <response template></response template>
                3. Keep response concise
                4. Response must be a plain text. Do not highlight text or add any styles.
                """ 
        });

        var request = new 
        {
            model = "gpt-4.1-nano",
            messages = messages,
        };

        var response = await _httpClient.PostAsJsonAsync("/v1/chat/completions", request, cancellationToken);
        _logger.LogInformation("Open AI Response status code: {statusCode}", response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        // _logger.LogInformation("Open AI Response content: {responseContent}", responseContent);

        var jsonResponse = JObject.Parse(responseContent);
        var reviewText = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();
        
        return reviewText;
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

    private static List<object> GetRelevantCodeAssetsSelectionCoTExamples()
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


                    Return a list of all relevant code assets as a JSON object with key "ids" and array of string inside <json></json> tag based on the project description. 
                    Explain your reasoning step by step before returning the final answer.
                    """
            },
            new
            {
                role = "assistant",
                content =
                    """
                    Let's analyze the project requirements step by step:

                    1. **Understanding the Goal**: The project is focused on **mortgage calculations** and generating **statements**.
                    2. **Identifying Key Functions**:
                    - The microservice needs to **calculate mortgage details** accurately.
                    - It should also be able to **generate mortgage statements**.
                    3. **Matching Available Code Assets**:
                    - **"Mortgage Calculation Service"** (ID: `"baa059d9-6c33-48e7-9a29-a101c210d165"`)  
                        - This service is specifically designed to **calculate mortgage details based on user inputs**, making it essential for the project.
                    - **"Electronic Statement Generator"** (ID: `"b73c177f-c73d-4d90-bcae-95e9a6f2c0d2"`)  
                        - Since the microservice also needs to **generate mortgage statements**, this service is directly relevant.

                    **Final Decision**: Based on this reasoning, the two most relevant assets are:
                    
                    <json>
                    {
                        "ids": [
                            "baa059d9-6c33-48e7-9a29-a101c210d165",
                            "b73c177f-c73d-4d90-bcae-95e9a6f2c0d2"
                        ]
                    }
                    </json>
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

                    Return a list of all relevant code assets as a JSON object with key "ids" and array of string inside <json></json> tag based on the project description. 
                    Explain your reasoning step by step before returning the final answer.
                    """
            },
            new
            {
                role = "assistant",
                content =
                    """
                    Let's analyze the project requirements step by step:

                    1. **Understanding the Goal**: The project is about enhancing the ATM network with a microservice that synchronizes transactions in real time.
                    2. **Identifying Key Functions**:
                    - The microservice must **integrate with ATMs** to handle transactions.
                    - It must **process financial transactions** reliably.
                    - It may require **real-time synchronization** with the core banking system.
                    3. **Matching Available Code Assets**:
                    - **"ATM Integration Service"** (ID: `"c5212788-0454-4269-a1da-23440509389c"`)  
                        - This service directly **connects ATM networks with the core banking system**, making it a crucial component for integration.
                    - **"Transaction Processing Engine"** (ID: `"3f6a4167-916f-4bc8-9002-9cf18c664d67"`)  
                        - Since transaction reliability is a major factor, this engine is essential for **processing and recording financial transactions securely**.

                    **Final Decision**: Based on this reasoning, the two most relevant assets are:
                    
                    <json>
                    {
                        "ids": [
                            "c5212788-0454-4269-a1da-23440509389c",
                            "3f6a4167-916f-4bc8-9002-9cf18c664d67"
                        ]
                    }
                    </json>
                    """
            }
        ];


        return [.. firstExample, .. secondExample];
    }
}
