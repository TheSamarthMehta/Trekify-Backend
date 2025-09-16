using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trekify.API.Data;

namespace Trekify.API.Services;

/// <summary>
/// Service to handle database initialization and health checks
/// </summary>
public interface IDatabaseService
{
    Task<bool> CanConnectAsync();
    Task<bool> EnsureDatabaseCreatedAsync();
    Task<string> GetDatabaseInfoAsync();
}

public class DatabaseService : IDatabaseService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(ApplicationDbContext context, ILogger<DatabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> CanConnectAsync()
    {
        try
        {
            await _context.Database.CanConnectAsync();
            _logger.LogInformation("Database connection successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection failed");
            return false;
        }
    }

    public async Task<bool> EnsureDatabaseCreatedAsync()
    {
        try
        {
            var created = await _context.Database.EnsureCreatedAsync();
            if (created)
            {
                _logger.LogInformation("Database was created");
            }
            else
            {
                _logger.LogInformation("Database already exists");
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure database creation");
            return false;
        }
    }

    public async Task<string> GetDatabaseInfoAsync()
    {
        try
        {
            var connection = _context.Database.GetDbConnection();
            var canConnect = await CanConnectAsync();
            
            var info = new
            {
                ConnectionString = connection.ConnectionString,
                DatabaseName = connection.Database,
                ServerVersion = canConnect ? _context.Database.ProviderName : "Unable to connect",
                CanConnect = canConnect,
                Tables = canConnect ? new
                {
                    UsersCount = await _context.Users.CountAsync(),
                    TreksCount = await _context.Treks.CountAsync()
                } : null
            };

            return System.Text.Json.JsonSerializer.Serialize(info, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get database info");
            return $"Error getting database info: {ex.Message}";
        }
    }
}