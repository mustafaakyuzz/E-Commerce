using MiniECommerce.Orders.WebAPI.Models;

namespace MiniECommerce.Orders.WebAPI.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<Order> orders, CancellationToken cancellationToken = default);
}
