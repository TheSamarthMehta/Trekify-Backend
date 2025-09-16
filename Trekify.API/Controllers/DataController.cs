using Microsoft.AspNetCore.Mvc;
using Trekify.API.DTOs;
using Trekify.API.Models;
using Trekify.API.Services;

namespace Trekify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IExcelService _excelService;

    public DataController(IExcelService excelService)
    {
        _excelService = excelService;
    }

    /// <summary>
    /// Load trek data from Excel file
    /// </summary>
    /// <returns>Trek data with headers information</returns>
    [HttpGet("load-excel")]
    public async Task<ActionResult<ApiResponseDto<List<Trek>>>> LoadExcel()
    {
        try
        {
            var result = await _excelService.LoadTreksFromExcelAsync();
            
            if (!result.Success)
            {
                if (result.Error?.Contains("not found") == true)
                {
                    return NotFound(new ApiResponseDto<List<Trek>>
                    {
                        Success = false,
                        Message = "Excel file not found",
                        Path = result.Error
                    });
                }
                return StatusCode(500, result);
            }

            // Add headers property for compatibility with Node.js response
            var response = new ApiResponseDto<List<Trek>>
            {
                Success = result.Success,
                Message = result.Message,
                Count = result.Count,
                Data = result.Data,
                Headers = new[] { "Serial No", "Trek Name", "State", "Trek Type", "Difficulty Level", 
                                "Season", "Duration", "Distance", "Max Altitude", "Trek Description", 
                                "Age Group", "Guide Needed", "Snow Trek", "Recommended Gear", "Image" }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LoadExcel: {ex.Message}");
            return StatusCode(500, ApiResponseDto<List<Trek>>.ErrorResponse("Error loading trek data from Excel", ex.Message));
        }
    }

    /// <summary>
    /// Get all trek data
    /// </summary>
    /// <returns>All trek data</returns>
    [HttpGet("")]
    public async Task<ActionResult<ApiResponseDto<List<Trek>>>> GetAllTreks()
    {
        try
        {
            var result = await _excelService.LoadTreksFromExcelAsync();
            
            if (!result.Success)
            {
                if (result.Error?.Contains("not found") == true)
                {
                    return NotFound(ApiResponseDto<List<Trek>>.ErrorResponse("Excel file not found"));
                }
                return StatusCode(500, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching trek data: {ex.Message}");
            return StatusCode(500, ApiResponseDto<List<Trek>>.ErrorResponse("Error fetching trek data"));
        }
    }

    /// <summary>
    /// Get unique states
    /// </summary>
    /// <returns>List of unique states</returns>
    [HttpGet("states")]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetStates()
    {
        try
        {
            var result = await _excelService.GetUniqueStatesAsync();
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching states: {ex.Message}");
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResponse("Error fetching states", ex.Message));
        }
    }

    /// <summary>
    /// Get unique trek types
    /// </summary>
    /// <returns>List of unique trek types</returns>
    [HttpGet("trek-types")]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetTrekTypes()
    {
        try
        {
            var result = await _excelService.GetUniqueTrekTypesAsync();
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching trek types: {ex.Message}");
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResponse("Error fetching trek types", ex.Message));
        }
    }
}