using Microsoft.OpenApi.Models;
using ProductService.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

using MongoDB.Driver;

using Microsoft.AspNetCore.JsonPatch;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));


// Configuración de MongoDB
var mongoConnectionString = Environment.GetEnvironmentVariable("Mongo__ConnectionString") ?? "mongodb://localhost:27017";
var mongoClient = new MongoClient(mongoConnectionString);
var database = mongoClient.GetDatabase("ProductsDb");
var productsCollection = database.GetCollection<Product>("Products");

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Configuración de servicios
// ----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Service API",
        Version = "v1"
    });
});

//Creamos la app
var app = builder.Build();

// Configuramos el redireccionamiento HTTPS
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API v1");
    c.RoutePrefix = "swagger"; // Acceso: /swagger
});


//Obtener todos los productos
app.MapGet("/products", async() =>
{

    var products = await productsCollection.Find(_ => true).ToListAsync();
    return Results.Ok(products);
    
})
.WithName("GetProducts");

//Obtener un producto por ID
app.MapGet("/product/{id:guid}", async(Guid id) =>
{
    var product = await productsCollection.Find(p => p._id == id).FirstOrDefaultAsync();
    return product is not null ? Results.Ok(product) : Results.NotFound();
    
})
.WithName("GetProductById");

//Crear un nuevo producto
app.MapPost("/product", async(Product product) =>
{
    product._id = Guid.NewGuid(); // Asegura que el UUID sea único
    await productsCollection.InsertOneAsync(product);
    return Results.Created($"/products/{product._id}", product);
})
.WithName("CreateProduct");

//Actualizar un producto por ID
app.MapPut("/product/{id:guid}", async (Guid id, Product updatedFields) =>
{
    var product = await productsCollection.Find(p => p._id == id).FirstOrDefaultAsync();
    if (product is null) return Results.NotFound();

    // Solo actualiza si el campo recibido es diferente del valor por defecto
    if (!string.IsNullOrEmpty(updatedFields.Name))
        product.Name = updatedFields.Name;
    if (updatedFields.Price != 0)
        product.Price = updatedFields.Price;
    if (updatedFields.Stock != 0)
        product.Stock = updatedFields.Stock;

    var filter = Builders<Product>.Filter.Eq(p => p._id, id);
    await productsCollection.ReplaceOneAsync(filter, product);

    return Results.Ok(product);

})
.WithName("UpdateProduct");

//Eliminar un producto por ID
app.MapDelete("/product/{id:guid}", async (Guid id) =>
{
    var result = await productsCollection.DeleteOneAsync(p => p._id == id);
    if (result.DeletedCount == 0) return Results.NotFound();
    return Results.NoContent();
})
.WithName("DeleteProduct");

app.Run();

