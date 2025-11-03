namespace courses_buynsell_api.DTOs.VNPAY;

public class PaymentResponseDto
{
    public string PaymentUrl { get; set; } = string.Empty;
    public string TransactionCode { get; set; } = string.Empty;
}