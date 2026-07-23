using MiniECommerce.ShoppingCarts.WebAPI.Models;
using System.Linq.Expressions;

namespace MiniECommerce.ShoppingCarts.WebAPI.Repositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
