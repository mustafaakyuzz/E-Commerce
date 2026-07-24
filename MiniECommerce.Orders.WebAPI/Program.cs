using MiniECommerce.Orders.WebAPI.Context;
using MiniECommerce.Orders.WebAPI.Dtos;
using MiniECommerce.Orders.WebAPI.Models;
using MiniECommerce.Orders.WebAPI.Options;
using MiniECommerce.Orders.WebAPI.Repositories;
using MiniECommerce.Orders.WebAPI.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// BSON Serializer Settings
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// Configuration & Mongo Context
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

// DI Registrations
builder.Services.AddHttpClient();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
