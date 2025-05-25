using InventoryService.Models;
using System;
using System.Threading.Tasks;

namespace InventoryService.Repositories
{
    public interface IInventoryRepository
    {
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<bool> UpdateProductAsync(Product product);
    }
}