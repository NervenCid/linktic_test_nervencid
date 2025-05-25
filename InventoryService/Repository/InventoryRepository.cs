using InventoryService.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace InventoryService.Repositories
{
    // Implementación del repositorio de inventario
    public class InventoryRepository : IInventoryRepository
    {
        // Cliente HTTP para consumir el microservicio de productos
        private readonly HttpClient _httpClient;
        // URL base del microservicio de productos
        private readonly string _productsApiBase;

        // Constructor que recibe el HttpClient y configura la URL base
        public InventoryRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _productsApiBase = Environment.GetEnvironmentVariable("PRODUCTS_API_BASE") ?? "http://localhost:5071";
        }

        // Obtiene un producto por su identificador único y API Key (opcional)
        public async Task<Product?> GetProductByIdAsync(Guid id, string? apiKey = null)
        {
            // Crea la solicitud HTTP GET
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_productsApiBase}/product/{id}");
            // Agrega el header X-API-KEY si se proporciona
            if (!string.IsNullOrEmpty(apiKey))
                request.Headers.Add("X-API-KEY", apiKey);

            // Envía la solicitud y obtiene la respuesta
            var response = await _httpClient.SendAsync(request);
            // Si la respuesta no es exitosa, retorna null
            if (!response.IsSuccessStatusCode)
                return null;

            // Deserializa el producto desde el contenido de la respuesta
            return await response.Content.ReadFromJsonAsync<Product>();
        }

        // Actualiza un producto usando la API Key (opcional)
        public async Task<bool> UpdateProductAsync(Product product, string? apiKey = null)
        {
            // Crea la solicitud HTTP PUT con el producto como contenido
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_productsApiBase}/product/{product._id}")
            {
                Content = JsonContent.Create(product)
            };
            // Agrega el header X-API-KEY si se proporciona
            if (!string.IsNullOrEmpty(apiKey))
                request.Headers.Add("X-API-KEY", apiKey);

            // Envía la solicitud y retorna true si fue exitosa
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}