using Microsoft.EntityFrameworkCore;
using MiniECommerce.ShoppingCarts.WebAPI.Models;

namespace MiniECommerce.ShoppingCarts.WebAPI.Context
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    }
}
