using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using courses_buynsell_api.DTOs.VNPAY;
using courses_buynsell_api.Interfaces;
using System.Security.Claims;

namespace courses_buynsell_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
    {
        try
        {
            // Lấy userId từ HttpContext.Items giống như các controller khác
            int userId = HttpContext.Items["UserId"] as int? ?? -1;

            if (userId == -1)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            var result = await _paymentService.CreatePaymentAsync(userId, request, ipAddress);

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnPayReturn([FromQuery] VnPayCallbackDto callback)
    {
        try
        {
            bool isSuccess = await _paymentService.ProcessPaymentCallbackAsync(callback);

            if (isSuccess)
            {
                // Redirect to frontend success page
                return Redirect($"http://localhost:3000/payment/success?transactionCode={callback.vnp_TxnRef}");
            }
            else
            {
                // Redirect to frontend failure page
                return Redirect($"http://localhost:3000/payment/failure?transactionCode={callback.vnp_TxnRef}");
            }
        }
        catch (Exception ex)
        {
            return Redirect($"http://localhost:3000/payment/error?message={ex.Message}");
        }
    }

    [HttpGet("transaction/{transactionCode}")]
    [Authorize]
    public async Task<IActionResult> GetTransaction(string transactionCode)
    {
        try
        {
            int userId = HttpContext.Items["UserId"] as int? ?? -1;

            if (userId == -1)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var transaction = await _paymentService.GetTransactionByCodeAsync(transactionCode);
            return Ok(new { success = true, data = transaction });
        }
        catch (Exception ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }
}