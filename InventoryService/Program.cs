using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

using InventoryService.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

const string productosApiBase = "http://localhost:5071"; // Cambia esto por la URL real


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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

