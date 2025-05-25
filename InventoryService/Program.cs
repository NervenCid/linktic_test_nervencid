using Microsoft.OpenApi.Models;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

using InventoryService.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();



// ----------------------------
// Configuración de servicios
// ----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Service API",
        Version = "v1"
    });
});

var app = builder.Build();

var productosApiBase = Environment.GetEnvironmentVariable("PRODUCTS_API_BASE") ?? "http://localhost:5071"; // Cambia esto por la URL real


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Consultar la cantidad disponible de un producto específico por ID
app.MapGet("/stock/{id:guid}", async (Guid id, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync($"{productosApiBase}/product/{id}");


    if (!response.IsSuccessStatusCode)
        return Results.NotFound();

    var product = await response.Content.ReadFromJsonAsync<Product>();
    if (product is null)
        return Results.NotFound();

    return Results.Ok(new { Name = product.Name, Stock = product.Stock });
});

// Actualizar la cantidad disponible de un producto tras una compra
app.MapPost("/buy/{id:guid}", async (Guid id, BuyRequest buyRequest, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    // Obtener el producto actual
    var response = await client.GetAsync($"{productosApiBase}/product/{id}");
    if (!response.IsSuccessStatusCode)
        return Results.NotFound();

    var product = await response.Content.ReadFromJsonAsync<Product>();
    if (product is null)
        return Results.NotFound();

    if (product.Stock < buyRequest.Quantity)
        return Results.BadRequest(new { Message = "Stock insuficiente" });

    product.Stock -= buyRequest.Quantity;

    // Actualizar el producto en el API de productos
    var updateResponse = await client.PutAsJsonAsync($"{productosApiBase}/product/{id}", product);
    if (!updateResponse.IsSuccessStatusCode)
        return Results.StatusCode((int)updateResponse.StatusCode);

    return Results.Ok(new { Name = product.Name, Stock = product.Stock });
});


app.Run();

