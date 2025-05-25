// Este es el modelo del producto
namespace InventoryService.Models
{
    // Clase que representa un producto en el inventario
    public class Product
    {
        // Identificador único del producto
        public Guid _id { get; set; } = Guid.NewGuid();

        // Nombre del producto
        public string Name { get; set; } = "";

        // Precio del producto
        public double Price { get; set; }

        // Cantidad disponible en stock
        public int Stock { get; set; }
    }
}