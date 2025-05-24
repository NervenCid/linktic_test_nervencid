using Microsoft.OpenApi.Models;
using ProductService.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API v1");
    c.RoutePrefix = "swagger"; // Acceso: /swagger
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


var products = new List<Product>
{
    new() { Id = 1, Name = "Product A", Price = 10.99 },
    new() { Id = 2, Name = "Product B", Price = 12.99 },
    new() { Id = 3, Name = "Product C", Price = 15.99 }
};

/*
app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");*/

//Obtener todos los productos
app.MapGet("/products", () =>
{

    return Results.Ok(products);
    
})
.WithName("GetProducts");

//Obtener un producto por ID
app.MapGet("/product/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
    
})
.WithName("GetProductById");

//Crear un nuevo producto
app.MapPost("/product", (Product product) =>
{
    product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;
    products.Add(product);
    return Results.Created($"/products/{product.Id}", product);
})
.WithName("CreateProduct");

//Actualizar un producto por ID
app.MapPut("/product/{id:int}", (int id, Product updatedProduct) =>
{
    var index = products.FindIndex(p => p.Id == id);
    if (index == -1) return Results.NotFound();

    updatedProduct.Id = id;
    products[index] = updatedProduct;
    return Results.Ok(updatedProduct);
})
.WithName("UpdateProduct");

//Eliminar un producto por ID
app.MapDelete("/product/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null) return Results.NotFound();

    products.Remove(product);
    return Results.NoContent();
})
.WithName("DeleteProduct");

app.Run();

/*
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
*/