using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trekify.API.DTOs;
using Trekify.API.Services;

namespace Trekify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>User registration response</returns>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponseDto<AuthResponseDto>.ErrorResponse("Please provide email, password, and name"));
        }

        if (request.Password.Length < 6)
        {
            return BadRequest(ApiResponseDto<AuthResponseDto>.ErrorResponse("Password must be at least 6 characters long"));
        }

        var result = await _authService.RegisterAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return StatusCode(201, result);
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Login response with token</returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponseDto<AuthResponseDto>.ErrorResponse("Please provide email and password"));
        }

        var result = await _authService.LoginAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get current user info (validate token)
    /// </summary>
    /// <returns>Token validation response</returns>
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponseDto<object>>> Me()
    {
        try
        {
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("No token provided"));
            }

            var token = authHeader.Replace("Bearer ", "");
            var result = await _authService.ValidateTokenAsync(token);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get user error: {ex.Message}");
            return StatusCode(500, ApiResponseDto<object>.ErrorResponse("Server error"));
        }
    }
}