using Bogus;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MiniECommerce.Products.WebAPI.Context;
using MiniECommerce.Products.WebAPI.Dtos;
using MiniECommerce.Products.WebAPI.Models;
using TS.Result;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/seedData", (ApplicationDbContext context) =>
{
    for (int i = 0; i < 100; i++)
    {
        Faker faker = new();

        Product product = new()
        {
            Name = faker.Commerce.ProductName(),
            Price = Convert.ToDecimal(faker.Commerce.Price()),
            Stock = faker.Commerce.Random.Int(1,100)
        };
        context.Products.Add(product);
    }
    context.SaveChanges();

    return Results.Ok(Result<string>.Succeed("Seed Data executed successfully"));
});

app.MapGet("/getall", async (ApplicationDbContext context, CancellationToken cancellationToken) =>
{
    var products = await context.Products.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    Result<List<Product>> response = products;
    return response;
});

app.MapPost("/create", async (CreateProductDto request, ApplicationDbContext context, CancellationToken cancellationToken) =>
{
    bool isNameExist = await context.Products.AnyAsync(p => p.Name == request.Name, cancellationToken);

    if (isNameExist)
    {
        var response = Result<string>.Failure("Product is created before!");
        return Results.BadRequest(response);
    }

    Product product = new()
    {
        Name = request.Name,
        Price = request.Price,
        Stock = request.Stock,
    };

    await context.AddAsync(product, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);

    return Results.Ok(Result<string>.Succeed("Products created successfully"));
});

using (var scoped = app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context = srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

    app.Run();
