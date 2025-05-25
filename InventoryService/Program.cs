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
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    
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