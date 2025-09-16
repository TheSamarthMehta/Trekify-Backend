# Trekify Backend API (.NET)

A .NET 8 Web API backend for the Trekify application, providing RESTful APIs for user authentication, trek data management, and user preferences with SQL Server database integration.

## 🚀 Features

- **User Authentication**: Registration, login, and profile management with JWT
- **Trek Data Management**: Comprehensive trek information with filtering and search capabilities
- **SQL Server Integration**: Robust relational database with Entity Framework Core
- **Excel Data Processing**: Support for Excel data import and processing
- **Image Management**: Static file serving for trek images
- **CORS Support**: Cross-origin requests for frontend applications
- **Database Health Monitoring**: Built-in health check endpoints

## 🛠️ Technology Stack

- **Runtime**: .NET 8.0
- **Framework**: ASP.NET Core Web API
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **File Processing**: EPPlus for Excel file processing
- **Security**: BCrypt.Net for password hashing
- **Documentation**: Swagger/OpenAPI

## 📋 Prerequisites

Before running this application, make sure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **One of the following SQL Server options**:
  - [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (recommended for development)
  - SQL Server LocalDB (included with Visual Studio)
  - [Docker SQL Server](https://hub.docker.com/_/microsoft-mssql-server) container

## 🗄️ Database Setup

### Quick Setup
Run the automated setup script:
```powershell
cd Trekify.API
.\setup-database.ps1
```

### Manual Setup
1. **Install Entity Framework CLI tools**:
```bash
dotnet tool install --global dotnet-ef
```

2. **Update connection string** in `appsettings.json` if needed

3. **Run migrations**:
```bash
cd Trekify.API
dotnet ef database update
```

For detailed database setup instructions, see [DATABASE_SETUP.md](./Trekify.API/DATABASE_SETUP.md).

## ⚡ Quick Start

1. **Clone the repository**:
```bash
git clone https://github.com/TheSamarthMehta/Trekify-Backend.git
cd Trekify-Backend
```

2. **Setup the database**:
```bash
cd Trekify.API
.\setup-database.ps1
```

3. **Run the application**:
```bash
dotnet run
```

4. **Verify the setup**:
   - API: http://localhost:5000 or https://localhost:5001
   - Swagger UI: http://localhost:5000/swagger
   - Database health: http://localhost:5000/health/database

## 🔧 Configuration

### Connection Strings
The application supports multiple database configurations in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TrekifyDB;Trusted_Connection=true;...",
    "LocalDbConnection": "Server=(localdb)\\mssqllocaldb;Database=TrekifyDB;...",
    "DockerConnection": "Server=localhost,1433;Database=TrekifyDB;User Id=sa;..."
  }
}
```

### JWT Settings
Configure JWT authentication in `appsettings.json`:
```json
{
  "JwtSettings": {
    "Secret": "your-secret-key",
    "Issuer": "TrekifyAPI",
    "Audience": "TrekifyClient",
    "ExpirationHours": 24
  }
}
```

## 📚 API Documentation

### Authentication Endpoints
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login

### Data Endpoints
- `GET /api/data/treks` - Get all treks
- `GET /api/data/treks/{id}` - Get trek by ID
- `POST /api/data/import` - Import trek data from Excel

### Health Check Endpoints
- `GET /` - API status
- `GET /health/database` - Database connectivity status

### Interactive Documentation
Visit http://localhost:5000/swagger for interactive API documentation when running in development mode.

## 🗃️ Database Schema

### Users Table
| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | User identifier |
| Email | nvarchar(255) | User email (unique) |
| Password | nvarchar(500) | Hashed password |
| Name | nvarchar(100) | User display name |
| CreatedAt | datetime2 | Account creation timestamp |
| UpdatedAt | datetime2 | Last update timestamp |

### Treks Table
| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Trek identifier |
| TrekName | nvarchar(255) | Trek name |
| State | nvarchar(100) | Indian state |
| TrekType | nvarchar(100) | Type of trek |
| DifficultyLevel | nvarchar(50) | Difficulty rating |
| Season | nvarchar(100) | Best season |
| Duration | nvarchar(100) | Trek duration |
| Distance | nvarchar(100) | Trek distance |
| MaxAltitude | nvarchar(100) | Maximum altitude |
| TrekDescription | nvarchar(2000) | Detailed description |
| Image | nvarchar(500) | Image URL/path |
| AgeGroup | nvarchar(50) | Suitable age group |
| GuideNeeded | nvarchar(20) | Guide requirement |
| SnowTrek | nvarchar(10) | Snow trek indicator |
| RecommendedGear | nvarchar(1000) | Equipment recommendations |

## 🚀 Deployment

### Development
```bash
dotnet run --environment Development
```

### Production
1. Update connection string in `appsettings.Production.json`
2. Build the application:
```bash
dotnet publish -c Release -o ./publish
```
3. Deploy to your hosting platform

### Docker Deployment
The project includes Docker support. See `Dockerfile` and `docker-compose.yml` for container deployment.

## 📁 Project Structure

```
Trekify.API/
├── Controllers/         # API controllers
├── Data/               # Entity Framework context
├── DTOs/               # Data transfer objects
├── Middleware/         # Custom middleware
├── Migrations/         # Entity Framework migrations
├── Models/             # Entity models
├── Services/           # Business logic services
├── Properties/         # Launch settings
├── wwwroot/           # Static files
├── appsettings.json   # Configuration
├── Program.cs         # Application entry point
└── Trekify.API.csproj # Project file
```

## 🔍 Troubleshooting

### Database Connection Issues
1. Ensure SQL Server is running
2. Verify connection string in `appsettings.json`
3. Check firewall settings
4. Run `.\setup-database.ps1` for automated diagnosis

### Build Issues
1. Ensure .NET 8 SDK is installed
2. Restore packages: `dotnet restore`
3. Clean and rebuild: `dotnet clean && dotnet build`

### Migration Issues
1. Check Entity Framework tools: `dotnet ef --version`
2. Install if missing: `dotnet tool install --global dotnet-ef`
3. Create new migration: `dotnet ef migrations add NewMigration`

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📧 Support

For support and questions:
- Create an issue in this repository
- Contact: [your-email@example.com]

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Entity Framework team for the ORM
- The open-source community for the amazing packages used in this project