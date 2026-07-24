using MiniECommerce.Orders.WebAPI.Context;
using MiniECommerce.Orders.WebAPI.Models;
using MongoDB.Driver;

namespace MiniECommerce.Orders.WebAPI.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly MongoDbContext _context;

    public OrderRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = _context.GetCollection<Order>("Orders");
        return await items.Find(items => true).ToListAsync(cancellationToken);
    }
    public async Task InsertManyAsync(IEnumerable<Order> orders, CancellationToken cancellationToken = default)
    {
        var items = _context.GetCollection<Order>("Orders");
        await items.InsertManyAsync(orders, cancellationToken :  cancellationToken);
    }
}
