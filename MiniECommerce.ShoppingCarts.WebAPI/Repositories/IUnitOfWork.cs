namespace MiniECommerce.ShoppingCarts.WebAPI.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
