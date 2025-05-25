//Este es el modelo del producto
namespace InventoryService.Models
{
    public class Product
    {

        public Guid _id { get; set; } = Guid.NewGuid();
        //public int Id { get; set; }
        public string Name { get; set; } = "";
        public double Price { get; set; }
        public int Stock { get; set; }

    }
}