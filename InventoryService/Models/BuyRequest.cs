namespace InventoryService.Models
{
    // Modelo para la solicitud de compra de un producto
    public class BuyRequest
    {
        // Cantidad solicitada para comprar
        public int Quantity { get; set; }
    }
}