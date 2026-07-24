using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Orders.WebAPI.Dtos;
using MiniECommerce.Orders.WebAPI.Services;

namespace MiniECommerce.Orders.WebAPI.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] List<CreateOrderDto> request, CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }
}
