using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Repositories;

namespace ProductService.Controllers;

public static class ProductsController
{
    public static void RegisterEndpoints(this WebApplication app)
    {

        app.MapGet("/products", async (ProductRepository repo) =>
        {
            var products = await repo.GetAllAsync();
            return Results.Ok(products);
        })
        .WithName("GetProducts")
        .WithSummary("Obtiene todos los productos disponibles.")
        .WithDescription("Retorna la lista completa de productos almacenados en la base de datos.");


        app.MapGet("/product/{id:guid}", async (Guid id, ProductRepository repo) =>
        {
            var product = await repo.GetByIdAsync(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProductById")
        .WithSummary("Obtiene un producto por su identificador único (GUID).")
        .WithDescription("Busca y retorna un producto específico usando su identificador único. Si no existe, retorna NotFound.");

        app.MapPost("/product", async (Product product, ProductRepository repo) =>
        {
            product._id = Guid.NewGuid();
            await repo.InsertAsync(product);
            return Results.Created($"/products/{product._id}", product);
        })
        .WithName("CreateProduct")
        .WithSummary("Crea un nuevo producto.")
        .WithDescription("Agrega un nuevo producto a la base de datos. El identificador único se genera automáticamente.");

        app.MapPut("/product/{id:guid}", async (Guid id, Product updatedFields, ProductRepository repo) =>
        {
            var product = await repo.GetByIdAsync(id);
            if (product is null) return Results.NotFound();

            if (!string.IsNullOrEmpty(updatedFields.Name))
                product.Name = updatedFields.Name;
            if (updatedFields.Price != 0)
                product.Price = updatedFields.Price;
            if (updatedFields.Stock != 0)
                product.Stock = updatedFields.Stock;

            var updated = await repo.ReplaceAsync(product);
            return updated ? Results.Ok(product) : Results.StatusCode(500);
        })
        .WithName("UpdateProduct")
        .WithSummary("Actualiza un producto existente.")
        .WithDescription("Actualiza los campos enviados de un producto específico. Si el producto no existe, retorna NotFound.");

        app.MapDelete("/product/{id:guid}", async (Guid id, ProductRepository repo) =>
        {
            var deleted = await repo.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .WithSummary("Elimina un producto por su identificador único (GUID).")
        .WithDescription("Elimina un producto específico de la base de datos. Si el producto no existe, retorna NotFound.");
    }
}