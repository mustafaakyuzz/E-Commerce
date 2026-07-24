using MiniECommerce.Orders.WebAPI.Dtos;
using MiniECommerce.Orders.WebAPI.Models;
using MiniECommerce.Orders.WebAPI.Repositories;

namespace MiniECommerce.Orders.WebAPI.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public OrderService(IOrderRepository orderRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    public async Task<Result<List<OrderDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        List<OrderDto> orderDtos = new();
        Result<List<ProductDto>>? products = new();

        var httpClient = _httpClientFactory.CreateClient();
        string productsEndpoint = $"http://{_configuration.GetSection("HttpRequests:Products").Value}/api/products/getall";
        var message = await httpClient.GetAsync(productsEndpoint, cancellationToken);

        if (message.IsSuccessStatusCode)
        {
            products = await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>(cancellationToken: cancellationToken);
        }

        foreach(var order in orders)
        {
            OrderDto orderDto = new()
            {
                Id = order.Id,
                CreateAt = order.CreateAt,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                Price = order.Price,
                ProductName = products!.Data!.First(p => p.Id == order.ProductId).Name
            };
            orderDtos.Add(orderDto);
        }
        return new Result<List<OrderDto>>(orderDtos);
    }
    public async Task<Result<string>> CreateAsync(List<CreateOrderDto> request, CancellationToken cancellationToken = default)
    {
        List<Order> orders = new();

        foreach(var item in request)
        {
            Order order = new()
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price,
                CreateAt = DateTime.Now,
            };
            orders.Add(order);
        }
        await _orderRepository.InsertManyAsync(orders, cancellationToken);

        return new Result<string>("Order has been created successfully");
    }
}
