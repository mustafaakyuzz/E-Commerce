using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Products.WebAPI.Dtos;
using MiniECommerce.Products.WebAPI.Services;

namespace MiniECommerce.Products.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    
    [HttpGet("seedData")]
    public async Task<IActionResult> SeedData(CancellationToken cancellationToken)
    {
        var result = await _productService.SeedDataAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _productService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateProductDto request,  CancellationToken cancellationToken)
    {
        var result = await _productService.CreateAsync(request, cancellationToken);
        if (!result.IsSuccessful)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("change-product-stock")]
    public async Task<IActionResult> ChangeStock(List<ChangeProductStockDto> request, CancellationToken cancellationToken)
    {
        var result = await _productService.ChangeStockAsync(request, cancellationToken);
        return NoContent();
    }
}
