using Microsoft.EntityFrameworkCore;
using MiniECommerce.Products.WebAPI.Models;

namespace MiniECommerce.Products.WebAPI.Context
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(builder =>
            {
                builder.Property(p => p.Price).HasColumnType("money");
            });
        }
    }
}
