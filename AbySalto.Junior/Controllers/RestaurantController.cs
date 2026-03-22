using AbySalto.Junior.DTOs;
using AbySalto.Junior.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbySalto.Junior.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public RestaurantController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] bool sortByTotal = false, CancellationToken ct = default)
        {
            var orders = await _orderService.GetAllOrdersAsync(sortByTotal, ct);
            return Ok(orders);
        }

        [HttpGet("orders/{id:int}")]
        public async Task<IActionResult> GetOrder(int id, CancellationToken ct = default)
        {
            var order = await _orderService.GetOrderByIdAsync(id, ct);
            return Ok(order);
        }

        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto, CancellationToken ct = default)
        {
            var created = await _orderService.CreateOrderAsync(dto, ct);
            return CreatedAtAction(nameof(GetOrder), new { id = created.Id }, created);
        }

        [HttpPatch("orders/{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto, CancellationToken ct = default)
        {
            var updated = await _orderService.UpdateOrderStatusAsync(id, dto, ct);
            return Ok(updated);
        }
    }
}