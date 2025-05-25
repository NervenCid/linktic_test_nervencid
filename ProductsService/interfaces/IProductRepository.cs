// Importa el modelo Product
using ProductService.Models;
// Importa tipos para manejo de Guid y listas
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Define el namespace para los repositorios
namespace ProductService.Repositories
{
    // Interfaz para el repositorio de productos
    public interface IProductRepository
    {
        // Obtiene todos los productos de manera asíncrona
        Task<List<Product>> GetAllAsync();
        // Obtiene un producto por su Guid de manera asíncrona
        Task<Product?> GetByIdAsync(Guid id);
        // Inserta un nuevo producto de manera asíncrona
        Task InsertAsync(Product product);
        // Reemplaza (actualiza) un producto de manera asíncrona
        Task<bool> ReplaceAsync(Product product);
        // Elimina un producto por su Guid de manera asíncrona
        Task<bool> DeleteAsync(Guid id);
    }
}