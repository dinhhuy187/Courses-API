namespace courses_buynsell_api.DTOs.VNPAY;

public class CreatePaymentRequestDto
{
    public List<int> CourseIds { get; set; } = new List<int>();
    public string PaymentMethod { get; set; } = "VNPAY"; // VNPAY, MOMO, etc.
}