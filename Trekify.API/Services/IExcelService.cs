using Trekify.API.DTOs;
using Trekify.API.Models;

namespace Trekify.API.Services;

public interface IExcelService
{
    Task<ApiResponseDto<List<Trek>>> LoadTreksFromExcelAsync();
    Task<ApiResponseDto<List<string>>> GetUniqueStatesAsync();
    Task<ApiResponseDto<List<string>>> GetUniqueTrekTypesAsync();
}