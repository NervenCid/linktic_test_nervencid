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

        public async Task<Product?> GetProductByIdAsync(Guid id, string? apiKey = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_productsApiBase}/product/{id}");
            if (!string.IsNullOrEmpty(apiKey))
                request.Headers.Add("X-API-KEY", apiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<bool> UpdateProductAsync(Product product, string? apiKey = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_productsApiBase}/product/{product._id}")
            {
                Content = JsonContent.Create(product)
            };
            if (!string.IsNullOrEmpty(apiKey))
                request.Headers.Add("X-API-KEY", apiKey);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
