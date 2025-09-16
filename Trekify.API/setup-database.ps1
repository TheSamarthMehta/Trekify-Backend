# Database Setup Script for Trekify API
# This script helps set up SQL Server database with different configurations

Write-Host "=== Trekify Database Setup ===" -ForegroundColor Green
Write-Host ""

# Function to test database connection
function Test-DatabaseConnection($connectionString) {
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        $connection.Close()
        return $true
    }
    catch {
        return $false
    }
}

# Check for SQL Server instances
Write-Host "Checking for available SQL Server instances..." -ForegroundColor Yellow

# Test different connection options
$connections = @{
    "SQL Server Express" = "Server=.\SQLEXPRESS;Database=master;Trusted_Connection=true;TrustServerCertificate=true;"
    "LocalDB" = "Server=(localdb)\mssqllocaldb;Database=master;Trusted_Connection=true;"
    "Docker SQL Server" = "Server=localhost,1433;Database=master;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;"
}

$availableConnections = @()
foreach ($conn in $connections.GetEnumerator()) {
    Write-Host "Testing $($conn.Key)..." -NoNewline
    if (Test-DatabaseConnection $conn.Value) {
        Write-Host " ✓ Available" -ForegroundColor Green
        $availableConnections += $conn
    } else {
        Write-Host " ✗ Not available" -ForegroundColor Red
    }
}

if ($availableConnections.Count -eq 0) {
    Write-Host ""
    Write-Host "No SQL Server instances found. Please install one of the following:" -ForegroundColor Red
    Write-Host "1. SQL Server Express (free): https://www.microsoft.com/en-us/sql-server/sql-server-downloads"
    Write-Host "2. SQL Server LocalDB (with Visual Studio)"
    Write-Host "3. Docker SQL Server: docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest"
    exit 1
}

Write-Host ""
Write-Host "Setting up database using Entity Framework..." -ForegroundColor Yellow

# Run Entity Framework commands
try {
    # Update database
    Write-Host "Creating/updating database schema..." -NoNewline
    $result = & dotnet ef database update 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host " ✓ Success" -ForegroundColor Green
        Write-Host ""
        Write-Host "Database setup completed successfully!" -ForegroundColor Green
        Write-Host "You can now run: dotnet run" -ForegroundColor Cyan
    } else {
        Write-Host " ✗ Failed" -ForegroundColor Red
        Write-Host "Error details:" -ForegroundColor Red
        Write-Host $result -ForegroundColor Red
    }
} catch {
    Write-Host " ✗ Failed" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}