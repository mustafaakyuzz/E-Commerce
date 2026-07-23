using MiniECommerce.ShoppingCarts.WebAPI.Dtos;

namespace MiniECommerce.ShoppingCarts.WebAPI.Services;

public interface IShoppingCartService
{
    Task<Result<List<ShoppingCartDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<string>> CreateAsync(CreateShoppingCartDto request, CancellationToken cancellationToken);
    Task<Result<string>> CreateOrderAsync(CancellationToken cancellationToken);
}
