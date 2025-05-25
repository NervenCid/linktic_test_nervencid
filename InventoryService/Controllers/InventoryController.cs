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
            app.MapGet("/stock/{id:guid}", async (Guid id, IInventoryRepository repository) =>
            {
                var product = await repository.GetProductByIdAsync(id);
                if (product is null)
                    return Results.NotFound();

                return Results.Ok(new { Name = product.Name, Stock = product.Stock });
            });

            app.MapPost("/buy/{id:guid}", async (Guid id, BuyRequest buyRequest, IInventoryRepository repository) =>
            {
                var product = await repository.GetProductByIdAsync(id);
                if (product is null)
                    return Results.NotFound();

                if (product.Stock < buyRequest.Quantity)
                    return Results.BadRequest(new { Message = "Stock insuficiente" });

                product.Stock -= buyRequest.Quantity;

                var success = await repository.UpdateProductAsync(product);
                if (!success)
                    return Results.StatusCode(500);

                return Results.Ok(new { Name = product.Name, Stock = product.Stock });
            });
        }
    }
}