// Middleware para validar la API Key en las solicitudes HTTP
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Clase del middleware de API Key
public class ApiKeyMiddleware
{
    // Delegado al siguiente middleware en la tubería
    private readonly RequestDelegate _next;
    // Lista de API Keys válidas
    private readonly List<string> _validApiKeys;

    // Constructor que recibe el siguiente middleware y la configuración
    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;

        // Intenta cargar las keys desde configuración o variable de entorno
        _validApiKeys = configuration.GetSection("ApiKeys").Get<List<string>>() 
            ?? configuration["API_KEYS"]?.Split(';').ToList()
            ?? new List<string>();
    }

    // Método principal que intercepta cada solicitud HTTP
    public async Task InvokeAsync(HttpContext context)
    {
        // Obtiene la ruta de la solicitud
        var path = context.Request.Path.Value;

        // Ignorar rutas relacionadas con Swagger y favicon
        if (path != null && (path.StartsWith("/swagger") || path == "/favicon.ico"))
        {
            await _next(context);
            return;
        }
        
        // Verifica si el header X-API-KEY está presente
        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key was not provided.");
            return;
        }

        // Valida si la API Key es válida
        if (!_validApiKeys.Contains(extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        // Continúa con el siguiente middleware si la API Key es válida
        await _next(context);
    }
}