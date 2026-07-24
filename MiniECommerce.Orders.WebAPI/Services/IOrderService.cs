using MiniECommerce.Orders.WebAPI.Dtos;

namespace MiniECommerce.Orders.WebAPI.Services;

public interface IOrderService
{
    Task<Result<List<OrderDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<string>> CreateAsync(List<CreateOrderDto> request, CancellationToken cancellationToken = default);
}
