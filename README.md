# assets-manager-api  
A comprehensive API backend for managing code assets, projects, companies, users, and their associated data within an organization. API allows to create a start project from a project desctiption by finding relevant code assets using OpenAI's models. 

## Table of Contents  
- [Features](#features)  
- [Stack](#stack)  
- [Installation](#installation)  
  - [Prerequisites](#prerequisites)  
  - [Setup Instructions](#setup-instructions)  
- [Configuration](#configuration)  

## Features  
- CRUD operations for code assets, folders, and code files with filtering and pagination support.  
- Manage companies including creation, retrieving user's company info, and user management within companies.  
- User authentication and authorization with roles such as Admin and Enterprise.  
- Token-based authentication with refresh tokens support.  
- Email verification, password reset, and secure account management.  
- Manage tags with popular tags listing and search capabilities.  
- Start Projects creation based on textual project prompts with AI-assisted selection of relevant code assets.  
- Compile .NET project code remotely using a compilation API integration.  

## Stack  
- C# and .NET 8.0 Web API framework  
- Cosmos DB as primary database service  
- Gmail SMTP for sending emails  
- OpenAI API: gpt-4o-mini, gpt-4.1-nano

## Installation  

### Prerequisites  
- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [Cosmos DB instance](https://azure.microsoft.com/en-us/services/cosmos-db/)  
- SMTP service credentials (Gmail SMTP recommended)  

### Setup Instructions  
```bash
# Clone repository
git clone https://github.com/Shchoholiev/assets-manager-api.git
cd assets-manager-api

# Restore dependencies and build solution
dotnet restore
dotnet build

# Run the API (development environment)
dotnet run --project AssetsManagerApi.Api

# The API will be available at http://localhost:5052 (default)
# Optionally, use Swagger UI at /swagger to explore endpoints.
```

## Configuration  

The application supports multiple environment configurations with `appsettings.json` files:

- `appsettings.Development.json` for local development  
- `appsettings.Production.json` for production environment  
- `appsettings.Test.json` for testing environment  

### Key Configuration Sections  

- **CosmosDb**  
  - `DatabaseName` : Name of the Cosmos DB database instance.  

- **EmailSettings**  
  - `VerificationUrl` : URL for email verification links.  
  - `PasswordResetUrl` : URL for reset password links.  

- **OpenAi**  
  - `BaseUrl` : Base URL for OpenAI API integration (if used for any AI-powered features).  

- **JsonWebTokenKeys** (for JWT Auth)  
  - `ValidIssuer`  
  - `ValidAudience`  
  - Validation flags for issuer, audience, lifetime, signing keys.  

- **Smtp** (for email)  
  - `Host` : SMTP host (e.g., smtp.gmail.com)  
  - `Port` : SMTP port (e.g., 587)  
  - `Username` : Username for the SMTP server  

- **CompilationApis**  
  - `DotNetCompilationUrl` : URL endpoint for remote .NET project compilation service.  

### Environment Variables  
You may override configuration via environment variables or JSON config files, e.g.:  
- `ASPNETCORE_ENVIRONMENT` to specify the environment (Development, Production, Test)  
- SMTP credentials and JWT secrets should be secured and injected appropriately  

## Run tests
Default: `dotnet test`  

With logs: `dotnet test --logger "console;verbosity=detailed"`

Single file with logs: `dotnet test --filter "FullyQualifiedName~StartProjectsControllerTests" --logger "console;verbosity=detailed"`

Single test with logs: `dotnet test --filter FullyQualifiedName=AssetsManagerApi.IntegrationTests.Tests.StartProjectsControllerTests.CreateStartProject_ValidInput_ReturnsAssets --logger "console;verbosity=detailed"`
