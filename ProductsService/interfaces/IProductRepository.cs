using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductService.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task InsertAsync(Product product);
        Task<bool> ReplaceAsync(Product product);
        Task<bool> DeleteAsync(Guid id);
    }
}