using MiniECommerce.Products.WebAPI.Context;
using MiniECommerce.Products.WebAPI.Models;

namespace MiniECommerce.Products.WebAPI.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }
}
