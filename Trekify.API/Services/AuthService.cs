using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Trekify.API.Data;
using Trekify.API.DTOs;
using Trekify.API.Models;
using BCrypt.Net;

namespace Trekify.API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                return ApiResponseDto<AuthResponseDto>.ErrorResponse("User already exists with this email");
            }

            // Hash password using BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, 12);

            // Create new user
            var user = new User
            {
                Email = request.Email.ToLower().Trim(),
                Password = hashedPassword,
                Name = request.Name.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Use static JWT token from configuration (matching Node.js behavior)
            var token = _configuration["JwtSettings:Secret"];

            var authResponse = new AuthResponseDto
            {
                Token = token!,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name
                }
            };

            return ApiResponseDto<AuthResponseDto>.SuccessResponse("User registered successfully", authResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration error: {ex.Message}");
            return ApiResponseDto<AuthResponseDto>.ErrorResponse("Server error during registration");
        }
    }

    public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                return ApiResponseDto<AuthResponseDto>.ErrorResponse("Invalid email or password");
            }

            // Verify password
            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isValidPassword)
            {
                return ApiResponseDto<AuthResponseDto>.ErrorResponse("Invalid email or password");
            }

            // Use static JWT token from configuration (matching Node.js behavior)
            var token = _configuration["JwtSettings:Secret"];

            var authResponse = new AuthResponseDto
            {
                Token = token!,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name
                }
            };

            return ApiResponseDto<AuthResponseDto>.SuccessResponse("Login successful", authResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return ApiResponseDto<AuthResponseDto>.ErrorResponse("Server error during login");
        }
    }

    public Task<ApiResponseDto<object>> ValidateTokenAsync(string token)
    {
        try
        {
            // Simple token validation (matching Node.js behavior)
            var configToken = _configuration["JwtSettings:Secret"];
            
            if (token == configToken)
            {
                return Task.FromResult(ApiResponseDto<object>.SuccessResponse("Token is valid", new { authenticated = true }));
            }
            
            return Task.FromResult(ApiResponseDto<object>.ErrorResponse("Invalid token"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation error: {ex.Message}");
            return Task.FromResult(ApiResponseDto<object>.ErrorResponse("Token verification failed"));
        }
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            }),
            Expires = DateTime.UtcNow.AddHours(double.Parse(_configuration["JwtSettings:ExpirationHours"]!)),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}