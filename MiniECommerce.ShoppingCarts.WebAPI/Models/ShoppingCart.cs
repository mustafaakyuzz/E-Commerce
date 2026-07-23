namespace MiniECommerce.ShoppingCarts.WebAPI.Models;

public sealed class ShoppingCart : BaseEntity
{
    public Guid ProductId { get; set; }
    public int Quantity {  get; set; }
}
