using InventoryService.Models;
using System;
using System.Threading.Tasks;

namespace InventoryService.Repositories
{
    public interface IInventoryRepository
    {
        Task<Product?> GetProductByIdAsync(Guid id, string? apiKey = null);
        Task<bool> UpdateProductAsync(Product product, string? apiKey = null);
    }
}