namespace courses_buynsell_api.Controllers;

using courses_buynsell_api.DTOs.Momo;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost("CreateMomoPayment")]
    public async Task<IActionResult> CreateMomoPayment([FromBody] CreateMomoPaymentRequestDto dto)
    {
        var payUrl = await _checkoutService.CreateMomoPaymentAsync(dto);
        return Ok(new { payUrl });
    }

    [HttpPost("MomoNotify")]
    public async Task<IActionResult> MomoNotify()
    {
        var form = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
        await _checkoutService.HandleMomoCallbackAsync(form);
        return Ok();
    }
}
