using Microsoft.AspNetCore.Builder;
using InventoryService.Repositories;
using InventoryService.Models;
using System;

namespace InventoryService.Controllers
{
    public static class InventoryController
    {
        public static void RegisterEndpoints(WebApplication app)
        {
            // Endpoint para consultar el stock de un producto por su ID
            app.MapGet("/stock/{id:guid}", async (Guid id, IInventoryRepository repository, HttpContext context) =>
            {
                // Leer el API Key del request entrante
                var apiKey = context.Request.Headers["X-API-KEY"].FirstOrDefault();

                // Consultar el producto usando el repositorio y la API Key
                var product = await repository.GetProductByIdAsync(id, apiKey);

                // Si el producto no existe, retornar 404 NotFound
                if (product is null)
                    return Results.NotFound();

                // Retornar el nombre y stock del producto
                return Results.Ok(new { Name = product.Name, Stock = product.Stock });
            })
            .WithName("GetProductStock")
            .WithSummary("Consulta el stock disponible de un producto.")
            .WithDescription("Obtiene la cantidad disponible en inventario de un producto específico por su identificador único (GUID). Retorna NotFound si el producto no existe.");

            // Endpoint para realizar una compra y actualizar el stock
            app.MapPost("/buy/{id:guid}", async (Guid id, BuyRequest buyRequest, IInventoryRepository repository, HttpContext context) =>
            {
                // Leer el API Key del request entrante
                var apiKey = context.Request.Headers["X-API-KEY"].FirstOrDefault();

                // Consultar el producto usando el repositorio y la API Key
                var product = await repository.GetProductByIdAsync(id, apiKey);

                // Si el producto no existe, retornar 404 NotFound
                if (product is null)
                    return Results.NotFound();

                // Validar si hay suficiente stock para la compra
                if (product.Stock < buyRequest.Quantity)
                    return Results.BadRequest(new { Message = "Stock insuficiente" });

                // Descontar la cantidad comprada del stock
                product.Stock -= buyRequest.Quantity;

                // Actualizar el producto en el repositorio usando la API Key
                var success = await repository.UpdateProductAsync(product, apiKey);

                // Si la actualización falla, retornar error 500
                if (!success)
                    return Results.StatusCode(500);

                // Retornar el nombre y stock actualizado del producto
                return Results.Ok(new { Name = product.Name, Stock = product.Stock });
            })
            .WithName("BuyProduct")
            .WithSummary("Realiza una compra y actualiza el stock de un producto.")
            .WithDescription("Descuenta la cantidad comprada del stock de un producto específico. Retorna error si no hay suficiente stock o si ocurre un problema al actualizar.");
        }
    }
}