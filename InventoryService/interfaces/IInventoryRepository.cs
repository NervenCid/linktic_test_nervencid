using InventoryService.Models;
using System;
using System.Threading.Tasks;

namespace InventoryService.Repositories
{
    // Interfaz para el repositorio de inventario
    public interface IInventoryRepository
    {
        // Obtiene un producto por su identificador único y API Key (opcional)
        Task<Product?> GetProductByIdAsync(Guid id, string? apiKey = null);

        // Actualiza un producto usando la API Key (opcional)
        Task<bool> UpdateProductAsync(Product product, string? apiKey = null);
    }
}