using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courses_buynsell_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZaloPayController : ControllerBase
    {
        private readonly IZaloPayService _zaloPayService;

        public ZaloPayController(IZaloPayService zaloPayService)
        {
            _zaloPayService = zaloPayService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest req)
        {
            var result = await _zaloPayService.CreateOrderAsync(req.OrderId, req.Amount, req.Description);
            return Ok(result);
        }
    }

    public class CreateOrderRequest
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
