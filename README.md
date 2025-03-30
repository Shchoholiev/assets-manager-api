# assets-manager-api

## Run tests
Default: `dotnet test`  

With logs: `dotnet test --logger "console;verbosity=detailed"`

Single file with logs: `dotnet test --filter "FullyQualifiedName~StartProjectsControllerTests" --logger "console;verbosity=detailed"`

Single test with logs: `dotnet test --filter FullyQualifiedName=AssetsManagerApi.IntegrationTests.Tests.StartProjectsControllerTests.CreateStartProject_ValidInput_ReturnsAssets --logger "console;verbosity=detailed"`