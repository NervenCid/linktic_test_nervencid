using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using InventoryService.Models;

namespace InventoryService.UnitTests;

// Clase de pruebas de integración para InventoryService
public class InventoryIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    // Cliente HTTP para simular solicitudes a la API de InventoryService
    private readonly HttpClient _client;

    // Constructor que inicializa el cliente HTTP y agrega el header de API Key
    public InventoryIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-API-KEY", "key_test_local"); // Usa tu API Key válida
    }

    // Prueba que verifica que el endpoint GET /stock/{id} responde con 200 OK para un producto existente
    [Fact]
    public async Task GetStock_Endpoint_ReturnsOk_ForExistingProduct()
    {
        // 1. Consulta los productos en ProductsService
        using var productsClient = new HttpClient();
        productsClient.BaseAddress = new Uri("http://localhost:5071");
        productsClient.DefaultRequestHeaders.Add("X-API-KEY", "key_test_local");
        var productsResponse = await productsClient.GetAsync("/products");
        productsResponse.EnsureSuccessStatusCode();

        // Deserializa la lista de productos y verifica que no esté vacía
        var products = await productsResponse.Content.ReadFromJsonAsync<List<Product>>();
        products.Should().NotBeNull();
        products.Should().NotBeEmpty();

        // Toma el primer producto de la lista
        var firstProduct = products.First();

        // 2. Consulta el stock en InventoryService usando el id del primer producto
        var response = await _client.GetAsync($"/stock/{firstProduct._id}");

        // Assert: verifica que la respuesta sea 200 OK y que la información de stock no sea nula
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stockInfo = await response.Content.ReadFromJsonAsync<object>();
        stockInfo.Should().NotBeNull();
    }

    // Prueba que simula la compra de un producto y verifica que el stock se actualiza correctamente
    [Fact]
    public async Task BuyProduct_Endpoint_UpdatesStock_ForExistingProduct()
    {
        // 1. Consulta los productos en ProductsService
        using var productsClient = new HttpClient();
        productsClient.BaseAddress = new Uri("http://localhost:5071");
        productsClient.DefaultRequestHeaders.Add("X-API-KEY", "key_test_local");
        var productsResponse = await productsClient.GetAsync("/products");
        productsResponse.EnsureSuccessStatusCode();

        // Deserializa la lista de productos y verifica que no esté vacía
        var products = await productsResponse.Content.ReadFromJsonAsync<List<Product>>();
        products.Should().NotBeNull();
        products.Should().NotBeEmpty();

        // Toma el primer producto de la lista
        var firstProduct = products.First();

        // 2. Consulta el stock actual en InventoryService
        var stockResponse = await _client.GetAsync($"/stock/{firstProduct._id}");
        stockResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var stockInfo = await stockResponse.Content.ReadFromJsonAsync<StockResponse>();
        stockInfo.Should().NotBeNull();

        // 3. Simula la compra de 1 unidad usando el endpoint /buy/{id}
        var buyRequest = new { Quantity = 1 };
        var buyResponse = await _client.PostAsJsonAsync($"/buy/{firstProduct._id}", buyRequest);

        // Assert: la compra debe ser exitosa (200 OK) y el stock debe disminuir en 1
        buyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedStockInfo = await buyResponse.Content.ReadFromJsonAsync<StockResponse>();
        updatedStockInfo.Should().NotBeNull();
        updatedStockInfo.Stock.Should().Be(stockInfo.Stock - 1);
    }

    // Clase auxiliar para deserializar la respuesta de stock del endpoint
    public class StockResponse
    {
        public string Name { get; set; }
        public int Stock { get; set; }
    }

}