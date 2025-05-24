using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


//Este es el modelo del producto
namespace ProductService.Models
{
    public class Product
    {

        [BsonId]
        public Guid _id { get; set; } = Guid.NewGuid();
        //public int Id { get; set; }
        public string Name { get; set; } = "";
        public double Price { get; set; }
        public int Stock { get; set; }

    }
}