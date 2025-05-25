// Pruebas de integración para los endpoints de ProductsService

using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using ProductService.Models;

namespace ProductsService.UnitTests;

// Clase de pruebas de integración para ProductsService
public class ProductsIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    // Cliente HTTP para simular solicitudes a la API
    private readonly HttpClient _client;

    // Constructor que inicializa el cliente HTTP y agrega el header de API Key
    public ProductsIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-API-KEY", "key_test_local"); // Usa tu API Key válida
    }

    // Prueba que verifica que el endpoint GET /products responde con 200 OK
    [Fact]
    public async Task GetProducts_Endpoint_ReturnsOk()
    {
        // Act: realiza la solicitud GET al endpoint
        var response = await _client.GetAsync("/products");

        // Assert: verifica que la respuesta sea 200 OK
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Prueba que verifica que el endpoint POST /product crea un producto y responde con 201 Created
    [Fact]
    public async Task PostProduct_Endpoint_ReturnsCreated()
    {
        // Arrange: prepara un nuevo producto para enviar
        var newProduct = new Product
        {
            Name = "Producto de prueba",
            Price = 99.99,
            Stock = 10
        };

        // Act: realiza la solicitud POST al endpoint
        var response = await _client.PostAsJsonAsync("/product", newProduct);

        // Assert: verifica que la respuesta sea 201 Created y que el producto retornado tenga los datos correctos
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
        createdProduct.Should().NotBeNull();
        createdProduct.Name.Should().Be("Producto de prueba");
        createdProduct.Price.Should().Be(99.99);
        createdProduct.Stock.Should().Be(10);
    }

    // Prueba que verifica que el endpoint PUT /product/{id} actualiza un producto existente correctamente
    [Fact]
    public async Task PutProduct_Endpoint_UpdatesProduct()
    {
        // Arrange: crea un producto primero
        var newProduct = new Product
        {
            Name = "Producto original",
            Price = 50.0,
            Stock = 5
        };
        var createResponse = await _client.PostAsJsonAsync("/product", newProduct);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdProduct = await createResponse.Content.ReadFromJsonAsync<Product>();
        createdProduct.Should().NotBeNull();

        // Prepara los nuevos datos para actualizar
        var updatedProduct = new Product
        {
            Name = "Producto actualizado",
            Price = 75.0,
            Stock = 8
        };

        // Act: actualiza el producto usando su GUID
        var updateResponse = await _client.PutAsJsonAsync($"/product/{createdProduct._id}", updatedProduct);

        // Assert: la respuesta debe ser OK y los datos deben estar actualizados
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedResult = await updateResponse.Content.ReadFromJsonAsync<Product>();
        updatedResult.Should().NotBeNull();
        updatedResult.Name.Should().Be("Producto actualizado");
        updatedResult.Price.Should().Be(75.0);
        updatedResult.Stock.Should().Be(8);
    }
}