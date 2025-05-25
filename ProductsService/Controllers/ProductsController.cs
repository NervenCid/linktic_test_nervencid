using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Repositories;

namespace ProductService.Controllers;

public static class ProductsController
{
    public static void RegisterEndpoints(this WebApplication app)
    {
        // Endpoint para obtener todos los productos
        app.MapGet("/products", async (ProductRepository repo) =>
        {
            // Obtiene la lista de productos desde el repositorio
            var products = await repo.GetAllAsync();
            // Retorna la lista de productos con código 200 OK
            return Results.Ok(products);
        })
        .WithName("GetProducts")
        .WithSummary("Obtiene todos los productos disponibles.")
        .WithDescription("Retorna la lista completa de productos almacenados en la base de datos.");

        // Endpoint para obtener un producto por su identificador único (GUID)
        app.MapGet("/product/{id:guid}", async (Guid id, ProductRepository repo) =>
        {
            // Busca el producto por su ID
            var product = await repo.GetByIdAsync(id);
            // Si existe, retorna el producto; si no, retorna 404 NotFound
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProductById")
        .WithSummary("Obtiene un producto por su identificador único (GUID).")
        .WithDescription("Busca y retorna un producto específico usando su identificador único. Si no existe, retorna NotFound.");

        // Endpoint para crear un nuevo producto
        app.MapPost("/product", async (Product product, ProductRepository repo) =>
        {
            // Genera un nuevo GUID para el producto
            product._id = Guid.NewGuid();
            // Inserta el producto en la base de datos
            await repo.InsertAsync(product);
            // Retorna el producto creado con código 201 Created
            return Results.Created($"/products/{product._id}", product);
        })
        .WithName("CreateProduct")
        .WithSummary("Crea un nuevo producto.")
        .WithDescription("Agrega un nuevo producto a la base de datos. El identificador único se genera automáticamente.");

        // Endpoint para actualizar un producto existente
        app.MapPut("/product/{id:guid}", async (Guid id, Product updatedFields, ProductRepository repo) =>
        {
            // Busca el producto original por su ID
            var product = await repo.GetByIdAsync(id);
            if (product is null) return Results.NotFound();

            // Actualiza los campos solo si se envían valores nuevos
            if (!string.IsNullOrEmpty(updatedFields.Name))
                product.Name = updatedFields.Name;
            if (updatedFields.Price != 0)
                product.Price = updatedFields.Price;
            if (updatedFields.Stock != 0)
                product.Stock = updatedFields.Stock;

            // Guarda los cambios en la base de datos
            var updated = await repo.ReplaceAsync(product);
            // Retorna el producto actualizado o error 500 si falla
            return updated ? Results.Ok(product) : Results.StatusCode(500);
        })
        .WithName("UpdateProduct")
        .WithSummary("Actualiza un producto existente.")
        .WithDescription("Actualiza los campos enviados de un producto específico. Si el producto no existe, retorna NotFound.");

        // Endpoint para eliminar un producto por su identificador único (GUID)
        app.MapDelete("/product/{id:guid}", async (Guid id, ProductRepository repo) =>
        {
            // Intenta eliminar el producto por su ID
            var deleted = await repo.DeleteAsync(id);
            // Retorna 204 NoContent si se elimina, o 404 NotFound si no existe
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .WithSummary("Elimina un producto por su identificador único (GUID).")
        .WithDescription("Elimina un producto específico de la base de datos. Si el producto no existe, retorna NotFound.");
    }
}