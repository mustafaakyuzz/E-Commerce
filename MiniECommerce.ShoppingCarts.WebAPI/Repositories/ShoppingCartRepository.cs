using MiniECommerce.ShoppingCarts.WebAPI.Context;
using MiniECommerce.ShoppingCarts.WebAPI.Models;

namespace MiniECommerce.ShoppingCarts.WebAPI.Repositories;

public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
{
    public ShoppingCartRepository(ApplicationDbContext context) : base(context)
    {

    }
}
