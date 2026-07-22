using Bogus;
using MiniECommerce.Products.WebAPI.Dtos;
using MiniECommerce.Products.WebAPI.Models;
using MiniECommerce.Products.WebAPI.Repositories;
using TS.Result;

namespace MiniECommerce.Products.WebAPI.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<Result<string>> SeedDataAsync(CancellationToken cancellationToken)
    {
        var products = new List<Product>();
        for (int i = 0; i < 100; i++)
        {
            Faker faker = new();
            Product product = new()
            {
                Name = faker.Commerce.ProductName(),
                Price = Convert.ToDecimal(faker.Commerce.Price()),
                Stock = faker.Commerce.Random.Int(1, 100)
            };
            products.Add(product);
        }
        await _productRepository.AddRangeAsync(products, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Result<string>.Succeed("Seed Data executed successfully");
    }
    public async Task<Result<List<Product>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return Result<List<Product>>.Succeed(products);
    }
    public async Task<Result<string>> CreateAsync(CreateProductDto request, CancellationToken cancellationToken)
    {
        bool isNameExist = await _productRepository.AnyAsync(p => p.Name == request.Name, cancellationToken);

        if (isNameExist)
        {
            return Result<string>.Failure("Product is created before");
        }

        Product product = new()
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Result<string>.Succeed("Product is created successfully");
    }
    public async Task<Result<string>> ChangeStockAsync(List<ChangeProductStockDto> request, CancellationToken cancellationToken)
    {
        foreach(var item in request)
        {
            Product? product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if(product is not null)
            {
                product.Stock -= item.Quantity;
            }
        }
        await _productRepository.SaveChangesAsync(cancellationToken);
        return Result<string>.Succeed("");
    }
}
