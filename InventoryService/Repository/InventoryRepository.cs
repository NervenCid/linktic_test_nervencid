using InventoryService.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace InventoryService.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _productsApiBase;

        public InventoryRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _productsApiBase = Environment.GetEnvironmentVariable("PRODUCTS_API_BASE") ?? "http://localhost:5071";
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_productsApiBase}/product/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_productsApiBase}/product/{product._id}", product);
            return response.IsSuccessStatusCode;
        }
    }
}
