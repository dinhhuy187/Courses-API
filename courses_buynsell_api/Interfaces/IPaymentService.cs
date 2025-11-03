using courses_buynsell_api.DTOs.VNPAY;
namespace courses_buynsell_api.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreatePaymentAsync(int userId, CreatePaymentRequestDto request, string ipAddress);
    Task<bool> ProcessPaymentCallbackAsync(VnPayCallbackDto callback);
    Task<object> GetTransactionByCodeAsync(string transactionCode);
}