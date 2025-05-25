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
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Inventory Service API",
        Version = "v1"
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