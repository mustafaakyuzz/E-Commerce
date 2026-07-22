using MiniECommerce.Products.WebAPI.Dtos;
using MiniECommerce.Products.WebAPI.Models;
using TS.Result;

namespace MiniECommerce.Products.WebAPI.Services;

public interface IProductService
{
    Task<Result<string>> SeedDataAsync(CancellationToken cancellationToken);
    Task<Result<List<Product>>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<string>> CreateAsync(CreateProductDto request, CancellationToken cancellationToken);
    Task<Result<string>> ChangeStockAsync(List<ChangeProductStockDto> request, CancellationToken cancellationToken);
}
