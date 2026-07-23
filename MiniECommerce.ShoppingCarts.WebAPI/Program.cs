using Microsoft.EntityFrameworkCore;
using MiniECommerce.ShoppingCarts.WebAPI.Context;
using MiniECommerce.ShoppingCarts.WebAPI.Dtos;
using MiniECommerce.ShoppingCarts.WebAPI.Models;
using MiniECommerce.ShoppingCarts.WebAPI.Repositories;
using MiniECommerce.ShoppingCarts.WebAPI.Services;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));
});

// IHttpClientFactory Registration
builder.Services.AddHttpClient();

// Generic & Specific Repository Registration
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

// Service Registration
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping Carts v1 API");
});

app.MapControllers();

// Automatic Migration
using (var scoped = app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context = srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
