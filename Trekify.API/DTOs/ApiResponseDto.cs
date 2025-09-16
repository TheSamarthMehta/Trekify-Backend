namespace Trekify.API.DTOs;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int? Count { get; set; }
    public string[]? Headers { get; set; }
    public string? Error { get; set; }
    public string? Path { get; set; }

    public static ApiResponseDto<T> SuccessResponse(string message, T? data = default, int? count = null)
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Count = count
        };
    }

    public static ApiResponseDto<T> ErrorResponse(string message, string? error = null)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            Error = error
        };
    }
}