using MiniECommerce.Orders.WebAPI.Context;
using MiniECommerce.Orders.WebAPI.Dtos;
using MiniECommerce.Orders.WebAPI.Models;
using MiniECommerce.Orders.WebAPI.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/getall", async (MongoDbContext context, IConfiguration configuration) =>
{
    var items = context.GetCollection<Order>("Orders");

    var orders = await items.Find(items => true).ToListAsync();

    List<OrderDto> orderDtos = new();

    Result<List<ProductDto>>? products = new();

    HttpClient httpClient = new();
    string productsEndpoint = $"http://{ configuration.GetSection("HttpRequests:Products").Value}/getall";
    var message = await httpClient.GetAsync(productsEndpoint);

    if (message.IsSuccessStatusCode)
    {
        products = await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>();
    }

    foreach (var order in orders)
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
    return Results.Ok(new Result<List<OrderDto>>(orderDtos));
});

app.MapPost("/create", async (MongoDbContext context, List<CreateOrderDto> request) =>
{
    var items = context.GetCollection<Order>("Orders");
    List<Order> orders = new();

    foreach (var item in request)
    {
        Order order = new()
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            Price = item.Price,
            CreateAt = DateTime.Now
        };
        orders.Add(order);
    }
    await items.InsertManyAsync(orders);
    return Results.Ok(new Result<string>("Order has been created successfully!"));
});

app.Run();
