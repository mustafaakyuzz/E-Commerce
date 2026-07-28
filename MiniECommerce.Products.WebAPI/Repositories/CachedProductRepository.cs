using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using MiniECommerce.Products.WebAPI.Context;
using MiniECommerce.Products.WebAPI.Models;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace MiniECommerce.Products.WebAPI.Repositories;

public class CachedProductRepository : IProductRepository
{
    private readonly IProductRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly ApplicationDbContext _context;

    private const string GetAllCacheKey = "products-getall";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
    };
    
    public CachedProductRepository(IProductRepository decorated, IDistributedCache distributedCache, ApplicationDbContext context)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _context = context;
    }
    public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        await _decorated.AddAsync(entity, cancellationToken);
        await _distributedCache.RemoveAsync(GetAllCacheKey, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Product> entities, CancellationToken cancellationToken = default)
    {
        await _decorated.AddRangeAsync(entities, cancellationToken);
        await _distributedCache.RemoveAsync(GetAllCacheKey, cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _decorated.AnyAsync(predicate, cancellationToken);
    }

    public void Delete(Product entity)
    {
        _decorated.Delete(entity);
        _distributedCache.Remove(GetAllCacheKey);
        _distributedCache.Remove($"products-{entity.Id}");
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        string? cachedProducts = await _distributedCache.GetStringAsync(GetAllCacheKey, cancellationToken);

        if (string.IsNullOrEmpty(cachedProducts))
        {
            var products = await _decorated.GetAllAsync(cancellationToken);

            if(products is null || !products.Any())
            {
                return products ?? new List<Product>();
            }

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            await _distributedCache.SetStringAsync(
                GetAllCacheKey,
                JsonConvert.SerializeObject(products, _jsonSettings),
                cacheOptions,
                cancellationToken);

            return products;
        }
        var deserializeProducts = JsonConvert.DeserializeObject<List<Product>>(cachedProducts, _jsonSettings) ?? new List<Product>();

        foreach(var item in deserializeProducts)
        {
            _context.Set<Product>().Attach(item);
        }

        return deserializeProducts;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        string key = $"products-{id}";
        string? cachedProduct = await _distributedCache.GetStringAsync(
            key,
            cancellationToken);

        Product? product;
        if (string.IsNullOrEmpty(cachedProduct))
        {
            product = await _decorated.GetByIdAsync(id, cancellationToken);
            
            if (product is null)
            {
                return product;
            }

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };  

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(product, _jsonSettings),
                cacheOptions,
                cancellationToken);
            return product;
        }
        product = JsonConvert.DeserializeObject<Product>(cachedProduct, _jsonSettings);

        if (product is not null)
        {
            _context.Set<Product>().Attach(product);
        }

        return product;
    }

    public void Update(Product entity)
    {
        _decorated.Update(entity);
        _distributedCache.Remove(GetAllCacheKey);
        _distributedCache.Remove($"products-{entity.Id}");
    }
}
