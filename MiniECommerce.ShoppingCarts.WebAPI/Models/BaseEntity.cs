namespace MiniECommerce.ShoppingCarts.WebAPI.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
