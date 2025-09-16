using OfficeOpenXml;
using System.Text.RegularExpressions;
using Trekify.API.DTOs;
using Trekify.API.Models;

namespace Trekify.API.Services;

public class ExcelService : IExcelService
{
    private readonly string _dataPath;

    public ExcelService()
    {
        _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "data");
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public Task<ApiResponseDto<List<Trek>>> LoadTreksFromExcelAsync()
    {
        try
        {
            var excelPath = Path.Combine(_dataPath, "Flutter Data Set.xlsx");
            
            if (!File.Exists(excelPath))
            {
                return Task.FromResult(ApiResponseDto<List<Trek>>.ErrorResponse($"Excel file not found", excelPath));
            }

            var treks = new List<Trek>();

            using var package = new ExcelPackage(new FileInfo(excelPath));
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null)
            {
                return Task.FromResult(ApiResponseDto<List<Trek>>.ErrorResponse("No worksheet found in Excel file"));
            }

            var result = ParseTreksFromWorksheet(worksheet);
            
            return Task.FromResult(ApiResponseDto<List<Trek>>.SuccessResponse(
                $"Successfully loaded {result.Count} treks from Excel", 
                result, 
                result.Count));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading Excel data: {ex.Message}");
            return Task.FromResult(ApiResponseDto<List<Trek>>.ErrorResponse("Error loading trek data from Excel", ex.Message));
        }
    }

    public async Task<ApiResponseDto<List<string>>> GetUniqueStatesAsync()
    {
        try
        {
            var treksResult = await LoadTreksFromExcelAsync();
            if (!treksResult.Success || treksResult.Data == null)
            {
                return ApiResponseDto<List<string>>.ErrorResponse("Error loading trek data");
            }

            var states = treksResult.Data
                .Where(t => !string.IsNullOrWhiteSpace(t.State))
                .Select(t => t.State.Trim())
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            return ApiResponseDto<List<string>>.SuccessResponse("States loaded successfully", states);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching states: {ex.Message}");
            return ApiResponseDto<List<string>>.ErrorResponse("Error fetching states", ex.Message);
        }
    }

    public async Task<ApiResponseDto<List<string>>> GetUniqueTrekTypesAsync()
    {
        try
        {
            var treksResult = await LoadTreksFromExcelAsync();
            if (!treksResult.Success || treksResult.Data == null)
            {
                return ApiResponseDto<List<string>>.ErrorResponse("Error loading trek data");
            }

            var trekTypes = treksResult.Data
                .Where(t => !string.IsNullOrWhiteSpace(t.TrekType))
                .Select(t => t.TrekType.Trim())
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            return ApiResponseDto<List<string>>.SuccessResponse("Trek types loaded successfully", trekTypes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching trek types: {ex.Message}");
            return ApiResponseDto<List<string>>.ErrorResponse("Error fetching trek types", ex.Message);
        }
    }

    private List<Trek> ParseTreksFromWorksheet(ExcelWorksheet worksheet)
    {
        var treks = new List<Trek>();
        var rowCount = worksheet.Dimension?.Rows ?? 0;
        
        if (rowCount <= 1) return treks;

        // Build header index
        var headers = new List<string>();
        for (int col = 1; col <= (worksheet.Dimension?.Columns ?? 0); col++)
        {
            headers.Add(worksheet.Cells[1, col].Text);
        }

        var headerIndex = BuildHeaderIndex(headers);

        // Parse data rows
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var trek = ParseTrekFromRow(worksheet, row, headerIndex);
                if (trek != null)
                {
                    treks.Add(trek);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing row {row}: {ex.Message}");
            }
        }

        return treks;
    }

    private Dictionary<string, int> BuildHeaderIndex(List<string> headers)
    {
        var index = new Dictionary<string, int>();
        for (int i = 0; i < headers.Count; i++)
        {
            var normalized = NormalizeHeader(headers[i]);
            if (!string.IsNullOrEmpty(normalized))
            {
                index[normalized] = i + 1; // Excel columns are 1-based
            }
        }
        return index;
    }

    private string NormalizeHeader(string header)
    {
        if (string.IsNullOrEmpty(header)) return string.Empty;
        
        return Regex.Replace(header.ToLower().Trim(), @"[^a-z0-9]+", " ")
                    .Trim()
                    .Replace("  ", " ");
    }

    private int? FindColumnIndex(Dictionary<string, int> headerIndex, params string[] candidates)
    {
        foreach (var candidate in candidates)
        {
            var normalized = NormalizeHeader(candidate);
            if (headerIndex.TryGetValue(normalized, out int index))
            {
                return index;
            }
        }
        return null;
    }

    private Trek? ParseTrekFromRow(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerIndex)
    {
        var trekNameCol = FindColumnIndex(headerIndex, "trek name", "name") ?? 2;
        var stateCol = FindColumnIndex(headerIndex, "state") ?? 3;

        var trekName = worksheet.Cells[row, trekNameCol].Text;
        var state = worksheet.Cells[row, stateCol].Text;

        if (string.IsNullOrWhiteSpace(trekName) || string.IsNullOrWhiteSpace(state))
        {
            return null;
        }

        var serialCol = FindColumnIndex(headerIndex, "serial no", "serial number", "s no", "sno", "sr no") ?? 1;
        var trekTypeCol = FindColumnIndex(headerIndex, "trek type", "type") ?? 4;
        var difficultyCol = FindColumnIndex(headerIndex, "difficulty", "difficulty level") ?? 5;
        var seasonCol = FindColumnIndex(headerIndex, "season", "best season", "best time") ?? 6;
        var durationCol = FindColumnIndex(headerIndex, "duration") ?? 7;
        var distanceCol = FindColumnIndex(headerIndex, "distance") ?? 8;
        var altitudeCol = FindColumnIndex(headerIndex, "max altitude", "altitude", "height", "maxaltitude") ?? 9;
        var descCol = FindColumnIndex(headerIndex, "description", "trek description", "about") ?? 10;
        var imageCol = FindColumnIndex(headerIndex, "image", "image url", "image link", "cloudinary url", "photo");
        var ageCol = FindColumnIndex(headerIndex, "age group", "age", "recommended age");
        var guideCol = FindColumnIndex(headerIndex, "guide needed", "guide need", "need guide", "guide required");
        var snowCol = FindColumnIndex(headerIndex, "snow trek", "snow", "is snow trek");
        var gearCol = FindColumnIndex(headerIndex, "recommended gear", "gear", "gears", "what to carry");

        var image = DetectImageUrl(worksheet, row) ?? (imageCol.HasValue ? worksheet.Cells[row, imageCol.Value].Text : "");
        var guideNeeded = ParseGuideNeeded(guideCol.HasValue ? worksheet.Cells[row, guideCol.Value].Text : "");
        var snowTrek = ParseYesNo(snowCol.HasValue ? worksheet.Cells[row, snowCol.Value].Text : "");

        return new Trek
        {
            Id = row - 1,
            SerialNumber = int.TryParse(worksheet.Cells[row, serialCol].Text, out int serial) ? serial : row - 1,
            TrekName = trekName,
            State = state,
            TrekType = worksheet.Cells[row, trekTypeCol].Text,
            DifficultyLevel = worksheet.Cells[row, difficultyCol].Text,
            Season = worksheet.Cells[row, seasonCol].Text,
            Duration = worksheet.Cells[row, durationCol].Text,
            Distance = worksheet.Cells[row, distanceCol].Text,
            MaxAltitude = worksheet.Cells[row, altitudeCol].Text,
            TrekDescription = worksheet.Cells[row, descCol].Text,
            Image = image,
            AgeGroup = ageCol.HasValue ? worksheet.Cells[row, ageCol.Value].Text : "",
            GuideNeeded = guideNeeded,
            SnowTrek = snowTrek,
            RecommendedGear = gearCol.HasValue ? worksheet.Cells[row, gearCol.Value].Text : ""
        };
    }

    private string? DetectImageUrl(ExcelWorksheet worksheet, int row)
    {
        var colCount = worksheet.Dimension?.Columns ?? 0;
        for (int col = 1; col <= colCount; col++)
        {
            var cellValue = worksheet.Cells[row, col].Text;
            if (!string.IsNullOrEmpty(cellValue) && 
                (cellValue.Contains("cloudinary.com") || 
                 cellValue.Contains("res.cloudinary") || 
                 cellValue.StartsWith("https://res.cloudinary")))
            {
                return cellValue;
            }
        }
        return null;
    }

    private string ParseGuideNeeded(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        
        var v = value.ToLower().Trim();
        return v switch
        {
            "required" or "yes" or "y" or "true" or "1" => "YES",
            "not needed" or "not required" or "no" or "false" or "0" or "n" => "NO",
            "recommended" or "recommend" or "advisable" or "advised" => "RECOMMENDED",
            "optional" or "maybe" => "OPTIONAL",
            _ => v.ToUpper()
        };
    }

    private string ParseYesNo(string value)
    {
        if (string.IsNullOrEmpty(value)) return "NO";
        
        var v = value.ToLower().Trim();
        return (v == "yes" || v == "true" || v == "y" || v == "1") ? "YES" : "NO";
    }
}