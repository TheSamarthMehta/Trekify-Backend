# .NET Trekify API Deployment

## SQL Server Database Setup

### 1. Create Database Migration
```bash
cd Trekify.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 2. Connection String Configuration

**Development (appsettings.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TrekifyDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

**Production (appsettings.Production.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=TrekifyDB;User Id=username;Password=password;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Environment Variables (Alternative)
```bash
# Windows
set ASPNETCORE_ConnectionStrings__DefaultConnection="Server=server;Database=TrekifyDB;User Id=user;Password=pass;TrustServerCertificate=true"

# Linux/macOS
export ASPNETCORE_ConnectionStrings__DefaultConnection="Server=server;Database=TrekifyDB;User Id=user;Password=pass;TrustServerCertificate=true"
```

## Render.com Deployment (Equivalent to Node.js)

### render.yaml
```yaml
services:
  - type: web
    name: trekify-dotnet-api
    runtime: docker
    plan: free
    region: ohio
    branch: main
    dockerfilePath: ./Trekify.API/Dockerfile
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://0.0.0.0:$PORT
      - key: ASPNETCORE_ConnectionStrings__DefaultConnection
        fromDatabase:
          name: trekify-db
          property: connectionString
    buildCommand: dotnet publish -c Release -o ./publish

databases:
  - name: trekify-db
    databaseName: trekifydb
    user: trekify_user
    plan: free
    region: ohio
```

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Trekify.API.csproj", "."]
RUN dotnet restore "./Trekify.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Trekify.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trekify.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ../data ./data
COPY ../Images ./Images
ENTRYPOINT ["dotnet", "Trekify.API.dll"]
```

## Azure App Service Deployment

### 1. Publish Profile
```bash
dotnet publish -c Release -p:PublishProfile=Azure
```

### 2. Azure CLI Deployment
```bash
# Login to Azure
az login

# Create resource group
az group create --name trekify-rg --location "East US"

# Create App Service plan
az appservice plan create --name trekify-plan --resource-group trekify-rg --sku B1 --is-linux

# Create web app
az webapp create --resource-group trekify-rg --plan trekify-plan --name trekify-api --runtime "DOTNET|8.0"

# Configure connection string
az webapp config connection-string set --resource-group trekify-rg --name trekify-api --connection-string-type SQLServer --settings DefaultConnection="Server=tcp:your-server.database.windows.net,1433;Initial Catalog=TrekifyDB;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Deploy
az webapp deployment source config-zip --resource-group trekify-rg --name trekify-api --src ./publish.zip
```

## Development Setup

### 1. Install .NET 8 SDK
```bash
# Windows (using winget)
winget install Microsoft.DotNet.SDK.8

# macOS (using Homebrew)
brew install --cask dotnet

# Linux (Ubuntu)
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### 2. Install SQL Server
```bash
# Windows - SQL Server LocalDB (Development)
# Install with Visual Studio or SQL Server Express

# Linux - SQL Server 2022 (Ubuntu)
curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | sudo gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg
sudo add-apt-repository "deb [arch=amd64,armhf,arm64 signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/ubuntu/22.04/mssql-server-2022 $(lsb_release -cs) main"
sudo apt-get update
sudo apt-get install -y mssql-server
sudo /opt/mssql/bin/mssql-conf setup

# macOS - SQL Server Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" -p 1433:1433 --name sqlserver --hostname sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Run Application
```bash
cd Trekify.API
dotnet restore
dotnet ef database update
dotnet run
```

## Performance Considerations

### 1. Connection Pooling
Already configured in Entity Framework Core by default.

### 2. Async/Await
All database operations use async/await pattern.

### 3. Memory Management
- EPPlus Excel processing disposes resources properly
- Entity Framework change tracking optimized

### 4. Caching (Optional Enhancement)
```csharp
// Add to Program.cs
builder.Services.AddMemoryCache();

// Use in services
private readonly IMemoryCache _cache;
// Cache trek data for 30 minutes
_cache.Set("treks", treks, TimeSpan.FromMinutes(30));
```

## Monitoring and Logging

### Application Insights (Azure)
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-key-here"
  }
}
```

### Structured Logging
```csharp
// Already configured in Program.cs
builder.Logging.AddConsole();
builder.Logging.AddDebug();
```

## Security Considerations

### 1. HTTPS Only (Production)
```csharp
// Add to Program.cs for production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}
```

### 2. API Rate Limiting
```csharp
// Add package: AspNetCoreRateLimit
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
```

### 3. Input Validation
Model validation is already implemented using Data Annotations.

## Migration Checklist

- [x] **Database**: MongoDB → SQL Server with Entity Framework
- [x] **Authentication**: bcryptjs → BCrypt.Net-Next
- [x] **JWT**: jsonwebtoken → System.IdentityModel.Tokens.Jwt
- [x] **Excel**: xlsx → EPPlus
- [x] **CORS**: express cors → ASP.NET Core CORS
- [x] **Static Files**: express.static → Custom middleware
- [x] **Error Handling**: express error middleware → ASP.NET Core middleware
- [x] **API Endpoints**: All endpoints maintain same structure and responses
- [x] **Environment Config**: dotenv → appsettings.json
- [x] **Deployment**: Node.js deployment → .NET deployment options

## Cost Comparison

| Service | Node.js Cost | .NET Cost | Notes |
|---------|-------------|-----------|-------|
| Render Free | $0/month | $0/month | Same free tier |
| Azure App Service | ~$13/month | ~$13/month | B1 Basic plan |
| Database | MongoDB Atlas Free | SQL Azure Free | 32MB limit both |
| Render Paid | $7/month | $7/month | Same pricing |

The .NET version maintains the same deployment cost structure as the Node.js version.