using MongoDB.Driver;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductService.Repositories
{
    // Implementación del repositorio de productos
    public class ProductRepository : IProductRepository
    {
        // Colección de productos en MongoDB
        private readonly IMongoCollection<Product> _collection;

        // Constructor que recibe la base de datos y obtiene la colección
        public ProductRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Product>("Products");
        }

        // Obtiene todos los productos de manera asíncrona
        public async Task<List<Product>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        // Obtiene un producto por su Guid de manera asíncrona
        public async Task<Product?> GetByIdAsync(Guid id) =>
            await _collection.Find(p => p._id == id).FirstOrDefaultAsync();

        // Inserta un nuevo producto de manera asíncrona
        public async Task InsertAsync(Product product) =>
            await _collection.InsertOneAsync(product);

        // Reemplaza (actualiza) un producto de manera asíncrona
        public async Task<bool> ReplaceAsync(Product product)
        {
            var result = await _collection.ReplaceOneAsync(p => p._id == product._id, product);
            return result.ModifiedCount > 0;
        }

        // Elimina un producto por su Guid de manera asíncrona
        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _collection.DeleteOneAsync(p => p._id == id);
            return result.DeletedCount > 0;
        }
    }
}