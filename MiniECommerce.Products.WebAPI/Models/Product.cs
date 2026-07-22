namespace MiniECommerce.Products.WebAPI.Models;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
