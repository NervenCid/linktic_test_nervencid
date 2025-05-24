namespace ProductService.Models;

//Este es el modelo del producto
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
}