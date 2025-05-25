using Microsoft.OpenApi.Models;
using InventoryService.Repositories;
using InventoryService.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Registrar HttpClient para consumo de otros microservicios
builder.Services.AddHttpClient();

// Configurar Swagger para documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Incluir los comentarios XML para la documentación de Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Definición del esquema de seguridad para el API Key
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Ingresa la API Key en el header usando el siguiente formato: X-API-KEY: tu-clave",
        Name = "X-API-KEY", // Nombre del header
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    // Requisito global de seguridad para Swagger
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Registrar repositorio con interfaz para inyección de dependencias
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

var app = builder.Build();

// Configuración de Swagger en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirección a HTTPS
app.UseHttpsRedirection();

// Registrar endpoints del controlador de inventario
InventoryController.RegisterEndpoints(app);

// Ejecutamos
app.Run();

// Permitimos las pruebas unitarias
public partial class Program { }