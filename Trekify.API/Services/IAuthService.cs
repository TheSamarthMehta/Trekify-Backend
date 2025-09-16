using Trekify.API.DTOs;
using Trekify.API.Models;

namespace Trekify.API.Services;

public interface IAuthService
{
    Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponseDto<object>> ValidateTokenAsync(string token);
    string GenerateJwtToken(User user);
}