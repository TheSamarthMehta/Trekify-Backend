# Trekify Backend - .NET API

A .NET 8 Web API backend for the Trekify application, converted from Node.js/Express with MongoDB to .NET with SQL Server.

## Features

- **Authentication**: JWT-based authentication with register, login, and token validation
- **Trek Data Management**: Excel file parsing for trek information
- **SQL Server Integration**: Entity Framework Core with SQL Server database
- **Static File Serving**: Image serving from local Images folder
- **Error Handling**: Global error handling middleware
- **CORS Support**: Cross-origin resource sharing enabled

## API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - User login
- `GET /api/auth/me` - Validate token and get user info

### Data (`/api/data`)
- `GET /api/data/` - Get all trek data
- `GET /api/data/load-excel` - Load trek data from Excel with headers
- `GET /api/data/states` - Get unique states list
- `GET /api/data/trek-types` - Get unique trek types list

### Static Files
- `GET /images/*` - Serve images from Images folder

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio or VS Code

## Setup Instructions

### 1. Clone and Navigate
```bash
cd Trekify_Backend/Trekify.API
```

### 2. Install Dependencies
```bash
dotnet restore
```

### 3. Configure Database
Update `appsettings.json` with your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TrekifyDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Run Database Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## Project Structure

```
Trekify.API/
├── Controllers/
│   ├── AuthController.cs      # Authentication endpoints
│   └── DataController.cs      # Trek data endpoints
├── Data/
│   └── ApplicationDbContext.cs # Entity Framework DbContext
├── DTOs/
│   ├── ApiResponseDto.cs      # Standard API response format
│   ├── AuthResponseDto.cs     # Authentication response models
│   ├── LoginRequestDto.cs     # Login request model
│   └── RegisterRequestDto.cs  # Registration request model
├── Middleware/
│   ├── ErrorHandlingMiddleware.cs  # Global error handling
│   └── StaticFileMiddleware.cs     # Static file serving
├── Models/
│   ├── Trek.cs               # Trek entity model
│   └── User.cs               # User entity model
├── Services/
│   ├── AuthService.cs        # Authentication business logic
│   ├── ExcelService.cs       # Excel file processing
│   ├── IAuthService.cs       # Authentication interface
│   └── IExcelService.cs      # Excel service interface
├── appsettings.json          # Development configuration
├── appsettings.Production.json # Production configuration
├── Program.cs                # Application entry point
└── Trekify.API.csproj       # Project dependencies
```

## Key Features Migrated from Node.js

### Authentication
- BCrypt password hashing (12 rounds)
- Static JWT token validation (matches Node.js behavior)
- Same API endpoints and response format

### Trek Data Processing
- Excel file parsing using EPPlus
- Header-based column detection
- Same data transformation logic
- Identical API response structure

### Static Files
- Image serving from Images folder
- Content-type detection
- Same URL structure (`/images/*`)

### Error Handling
- Global exception handling
- 404 route handling
- Same error response format

## Configuration

### Environment Variables
Set these in `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server connection string"
  },
  "JwtSettings": {
    "Secret": "Your JWT secret key",
    "Issuer": "TrekifyAPI",
    "Audience": "TrekifyClient",
    "ExpirationHours": 24
  }
}
```

### SQL Server Setup
1. Install SQL Server (LocalDB for development)
2. Update connection string in `appsettings.json`
3. Run migrations to create database schema

## Migration from Node.js

This .NET version maintains 100% API compatibility with the original Node.js version:

| Node.js Package | .NET Equivalent |
|----------------|----------------|
| express | ASP.NET Core Web API |
| mongoose | Entity Framework Core |
| bcryptjs | BCrypt.Net-Next |
| jsonwebtoken | System.IdentityModel.Tokens.Jwt |
| xlsx | EPPlus |
| cors | Built-in CORS middleware |
| multer | Built-in file handling |

## Deployment

### Development
```bash
dotnet run
```

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### Docker Support
Create `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

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
ENTRYPOINT ["dotnet", "Trekify.API.dll"]
```

## Testing

Test the API using the included Swagger UI at `/swagger` or use tools like:
- Postman
- curl
- REST Client extensions

### Sample API Calls

```bash
# Register user
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password123","name":"Test User"}'

# Get trek data
curl -X GET https://localhost:5001/api/data/

# Get states
curl -X GET https://localhost:5001/api/data/states
```

## Dependencies

- **Microsoft.EntityFrameworkCore.SqlServer** - SQL Server database provider
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication
- **BCrypt.Net-Next** - Password hashing
- **EPPlus** - Excel file processing
- **System.IdentityModel.Tokens.Jwt** - JWT token generation

## Author

Converted from Node.js by: GitHub Copilot
Original Node.js version by: TheSamarthMehta

## License

ISC