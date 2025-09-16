namespace Trekify.API.Middleware;

public class StaticFileMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _imagesPath;

    public StaticFileMiddleware(RequestDelegate next)
    {
        _next = next;
        _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Images");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        // Handle /images requests
        if (path != null && path.StartsWith("/images/", StringComparison.OrdinalIgnoreCase))
        {
            var relativePath = path[8..]; // Remove "/images/" prefix
            var filePath = Path.Combine(_imagesPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
            
            if (File.Exists(filePath))
            {
                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                var contentType = GetContentType(ext);
                
                context.Response.ContentType = contentType;
                await context.Response.SendFileAsync(filePath);
                return;
            }
        }

        await _next(context);
    }

    private static string GetContentType(string extension)
    {
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".avif" => "image/avif",
            ".svg" => "image/svg+xml",
            ".bmp" => "image/bmp",
            ".ico" => "image/x-icon",
            _ => "application/octet-stream"
        };
    }
}