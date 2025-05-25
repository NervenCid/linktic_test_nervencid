using Microsoft.OpenApi.Models;
using InventoryService.Repositories;
using InventoryService.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Registrar HttpClient
builder.Services.AddHttpClient();

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 🔹 Comentarios XML para documentación de controladores y modelos
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // 🔐 Definición de seguridad para API Key
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Ingresa la API Key en el header usando el siguiente formato: X-API-KEY: tu-clave",
        Name = "X-API-KEY", // Nombre del header
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    // 🔐 Requisito de seguridad para todos los endpoints
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

// Registrar repositorio con interfaz
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Registrar endpoints del controlador
InventoryController.RegisterEndpoints(app);

app.Run();