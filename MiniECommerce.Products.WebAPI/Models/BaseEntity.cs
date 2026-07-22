namespace MiniECommerce.Products.WebAPI.Models;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
