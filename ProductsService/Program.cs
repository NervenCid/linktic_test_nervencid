using Microsoft.OpenApi.Models;
using ProductService.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.JsonPatch;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using ProductService.Repositories;
using ProductService.Controllers;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// Configuración de MongoDB
var mongoConnectionString = Environment.GetEnvironmentVariable("Mongo__ConnectionString") ?? "mongodb://localhost:27017";

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Configuración de servicios
// ----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    
});

// Registro de MongoDB para inyección de dependencias
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase("ProductsDb"));
builder.Services.AddScoped<ProductRepository>();

var app = builder.Build();

// Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API v1");
        c.RoutePrefix = "swagger";
    });
}

//Mapeamos los endpoints del controlador
ProductsController.RegisterEndpoints(app);

//Corremos la aplicacion
app.Run();