using Microsoft.AspNetCore.Mvc;
using MiniECommerce.ShoppingCarts.WebAPI.Dtos;
using MiniECommerce.ShoppingCarts.WebAPI.Services;

namespace MiniECommerce.ShoppingCarts.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly IShoppingCartService _shoppingCartService;

    public ShoppingCartController(IShoppingCartService shoppingCartService)
    {
        _shoppingCartService = shoppingCartService;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _shoppingCartService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateShoppingCartDto request,  CancellationToken cancellationToken)
    {
        var result = await _shoppingCartService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("createOrder")]
    public async Task<IActionResult> CreateOrder(CancellationToken cancellationToken)
    {
        var result = await _shoppingCartService.CreateOrderAsync(cancellationToken);
        return Ok(result);
    }
}
