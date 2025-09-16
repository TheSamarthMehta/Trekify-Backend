# Database Setup Guide

This document explains how to set up SQL Server database for the Trekify API.

## Prerequisites

You need one of the following SQL Server installations:

### Option 1: SQL Server Express (Recommended for Development)
- Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- Choose "Express" edition (free)
- During installation, make sure to install the Database Engine

### Option 2: SQL Server LocalDB
- Usually installed with Visual Studio
- Lightweight option for development

### Option 3: Docker SQL Server
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

## Connection String Configuration

The application supports multiple connection string configurations in `appsettings.json`:

- **DefaultConnection**: SQL Server Express (default)
- **LocalDbConnection**: SQL Server LocalDB 
- **DockerConnection**: Docker SQL Server container

To use a different connection, modify the `DefaultConnection` in `appsettings.json` or set the `ConnectionStrings:DefaultConnection` environment variable.

## Database Setup Methods

### Method 1: Automated Setup (Recommended)
Run the PowerShell setup script:
```powershell
.\setup-database.ps1
```

This script will:
- Check for available SQL Server instances
- Automatically configure the best available option
- Run Entity Framework migrations
- Create the database and tables

### Method 2: Manual Setup using Entity Framework CLI

1. **Install Entity Framework tools** (if not already installed):
```bash
dotnet tool install --global dotnet-ef
```

2. **Create migration** (if needed):
```bash
dotnet ef migrations add InitialCreateSqlServer
```

3. **Update database**:
```bash
dotnet ef database update
```

### Method 3: Manual SQL Script
If Entity Framework fails, you can manually create the database:

1. Connect to your SQL Server instance using SQL Server Management Studio (SSMS) or sqlcmd
2. Run the script in `database-setup.sql`
3. Then run: `dotnet ef database update`

## Connection String Examples

### SQL Server Express (Windows Authentication)
```json
"Server=.\\SQLEXPRESS;Database=TrekifyDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;"
```

### SQL Server with Username/Password
```json
"Server=localhost;Database=TrekifyDB;User Id=your_username;Password=your_password;TrustServerCertificate=true;MultipleActiveResultSets=true;"
```

### Azure SQL Database
```json
"Server=tcp:your-server.database.windows.net,1433;Database=TrekifyDB;User Id=your_username;Password=your_password;Encrypt=true;MultipleActiveResultSets=true;"
```

## Troubleshooting

### Error: "Unable to locate a Local Database Runtime installation"
- Install SQL Server Express or LocalDB
- Or use Docker SQL Server container

### Error: "A network-related or instance-specific error occurred"
- Ensure SQL Server service is running
- Check if the server name/instance name is correct
- Verify firewall settings allow SQL Server connections

### Error: "Login failed for user"
- Check username/password in connection string
- Ensure the user has appropriate permissions
- For Windows Authentication, ensure the current user has access

### Error: "Cannot open database"
- Run the setup script to create the database
- Verify the database name in the connection string

## Database Schema

The database includes the following main tables:

### Users Table
- Id (Primary Key)
- Email (Unique)
- Password (Hashed)
- Name
- CreatedAt
- UpdatedAt

### Treks Table
- Id (Primary Key)
- TrekName
- State
- TrekType
- DifficultyLevel
- Season
- Duration
- Distance
- MaxAltitude
- TrekDescription
- Image
- AgeGroup
- GuideNeeded
- SnowTrek
- RecommendedGear

## Environment Variables

You can override connection strings using environment variables:

```bash
# Windows
set ConnectionStrings__DefaultConnection="your_connection_string"

# Linux/Mac
export ConnectionStrings__DefaultConnection="your_connection_string"
```

## Production Deployment

For production deployment:

1. Use a proper SQL Server instance (not Express or LocalDB)
2. Update the connection string in `appsettings.Production.json`
3. Ensure proper security (encrypted connections, strong passwords)
4. Set up database backups
5. Configure appropriate user permissions (don't use SA account)

Example production connection string:
```json
"Server=your-production-server;Database=TrekifyDB;User Id=TrekifyUser;Password=SecurePassword123!;Encrypt=true;TrustServerCertificate=false;MultipleActiveResultSets=true;"
```