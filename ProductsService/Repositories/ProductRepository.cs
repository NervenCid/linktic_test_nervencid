using MongoDB.Driver;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _collection;

        public ProductRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Product>("Products");
        }

        public async Task<List<Product>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<Product?> GetByIdAsync(Guid id) =>
            await _collection.Find(p => p._id == id).FirstOrDefaultAsync();

        public async Task InsertAsync(Product product) =>
            await _collection.InsertOneAsync(product);

        public async Task<bool> ReplaceAsync(Product product)
        {
            var result = await _collection.ReplaceOneAsync(p => p._id == product._id, product);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _collection.DeleteOneAsync(p => p._id == id);
            return result.DeletedCount > 0;
        }
    }
}