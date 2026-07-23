using Microsoft.EntityFrameworkCore;
using MiniECommerce.ShoppingCarts.WebAPI.Context;
using MiniECommerce.ShoppingCarts.WebAPI.Models;
using System.Linq.Expressions;

namespace MiniECommerce.ShoppingCarts.WebAPI.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Set<T>().ToListAsync(cancellationToken);
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Set<T>().FindAsync(new object[] {id}, cancellationToken);
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Set<T>().AnyAsync(predicate, cancellationToken);
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _context.Set<T>().AddAsync(entity, cancellationToken);
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => await _context.Set<T>().AddRangeAsync(entities, cancellationToken);
    public void Update(T entity) => _context.Set<T>().Update(entity);
    public void Delete(T entity) => _context.Set<T>().Remove(entity);
    public void DeleteRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
