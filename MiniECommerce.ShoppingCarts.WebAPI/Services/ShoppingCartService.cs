using MiniECommerce.ShoppingCarts.WebAPI.Dtos;
using MiniECommerce.ShoppingCarts.WebAPI.Models;
using MiniECommerce.ShoppingCarts.WebAPI.Repositories;
using System.Text;
using System.Text.Json;

namespace MiniECommerce.ShoppingCarts.WebAPI.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ShoppingCartService(
        IShoppingCartRepository shoppingCartRepository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<Result<List<ShoppingCartDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var shoppingCarts = await _shoppingCartRepository.GetAllAsync();

        var client = _httpClientFactory.CreateClient();
        string productsEndpoint = $"http://{_configuration.GetSection("HttpRequest:Products").Value}/api/products/getall";
        var message = await client.GetAsync(productsEndpoint, cancellationToken);

        Result<List<ProductDto>>? products = new();

        if (message.IsSuccessStatusCode)
        {
            products = await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>(cancellationToken: cancellationToken);
        }

        var response = shoppingCarts.Select(s => new ShoppingCartDto()
        {
            Id = s.Id,
            ProductId = s.ProductId,
            Quantity = s.Quantity,
            ProductName = products?.Data?.FirstOrDefault(p => p.Id == s.ProductId)?.Name ?? "N/A",
            ProductPrice = products?.Data?.FirstOrDefault(p => p.Id == s.ProductId)?.Price ?? 0
        }).ToList();

        return new Result<List<ShoppingCartDto>>(response);
    }
    public async Task<Result<string>> CreateAsync(CreateShoppingCartDto request, CancellationToken cancellationToken)
    {
        ShoppingCart shoppingCart = new()
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
        };

        await _shoppingCartRepository.AddAsync(shoppingCart, cancellationToken);
        await _shoppingCartRepository.SaveChangesAsync(cancellationToken);

        return new Result<string>("Product has been added to Shopping Cart Successfully!");
    }
    public async Task<Result<string>> CreateOrderAsync(CancellationToken cancellationToken)
    {
        var shoppingCarts = await _shoppingCartRepository.GetAllAsync(cancellationToken);

        var client = _httpClientFactory.CreateClient();

        // Fetch Products
        string productsEndpoint = $"http://{_configuration.GetSection("HttpRequest:Products").Value}/api/products/getall";
        var message = await client.GetAsync(productsEndpoint, cancellationToken);

        Result<List<ProductDto>>? products = new();

        if (message.IsSuccessStatusCode)
        {
            products = await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>(cancellationToken: cancellationToken);
        }

        List<CreateOrderDto> response = shoppingCarts.Select(s => new CreateOrderDto
        {
            ProductId = s.ProductId,
            Quantity = s.Quantity,
            Price = products?.Data?.FirstOrDefault(p => p.Id == s.ProductId)?.Price ?? 0
        }).ToList();

        // Post Order to Order service
        string ordersEndpoint = $"http://{_configuration.GetSection("HttpRequest:Orders").Value}/api/orders/create";
        string stringJson = JsonSerializer.Serialize(response);
        var content = new StringContent(stringJson, Encoding.UTF8, "application/json");

        var orderMessage = await client.PostAsync(ordersEndpoint, content, cancellationToken);

        if (orderMessage.IsSuccessStatusCode)
        {
            // Update Product stock
            List<ChangeProductStockDto> changeProductStockDtos = shoppingCarts
                .Select(s => new ChangeProductStockDto(s.ProductId, s.Quantity))
                .ToList();

            string changeProductEndpoint = $"http://{_configuration.GetSection("HttpRequest:Products").Value}/api/products/change-product-stock";
            string productStringJson = JsonSerializer.Serialize(changeProductStockDtos);
            var productsContent = new StringContent(productStringJson, Encoding.UTF8, "application/json");

            await client.PostAsync(changeProductEndpoint, productsContent, cancellationToken);

            // Clear Shopping Cart
            _shoppingCartRepository.DeleteRange(shoppingCarts);
            await _shoppingCartRepository.SaveChangesAsync(cancellationToken);
        }
        return new Result<string>("Order created successfully");
    }
}
