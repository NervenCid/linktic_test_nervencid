// Middleware/ApiKeyMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<string> _validApiKeys;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;

        // Intenta cargar las keys desde configuración o variable de entorno
        _validApiKeys = configuration.GetSection("ApiKeys").Get<List<string>>() 
            ?? configuration["API_KEYS"]?.Split(';').ToList()
            ?? new List<string>();
    }

    public async Task InvokeAsync(HttpContext context)
    {

        var path = context.Request.Path.Value;

        // ✅ Ignorar rutas relacionadas con Swagger y favicon
        if (path != null && (path.StartsWith("/swagger") || path == "/favicon.ico"))
        {
            await _next(context);
            return;
        }
        
        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key was not provided.");
            return;
        }

        if (!_validApiKeys.Contains(extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        await _next(context);
    }
}