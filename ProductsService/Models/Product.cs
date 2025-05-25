using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// Este es el modelo del producto
namespace ProductService.Models
{
    // Clase que representa un producto
    public class Product
    {
        // Identificador único del producto en MongoDB
        [BsonId]
        public Guid _id { get; set; } = Guid.NewGuid();

        // Nombre del producto
        public string Name { get; set; } = "";

        // Precio del producto
        public double Price { get; set; }

        // Cantidad disponible en stock
        public int Stock { get; set; }
    }
}