namespace courses_buynsell_api.DTOs.Momo;

public class CreateMomoPaymentRequestDto
{
    public decimal Amount { get; set; }
    public int BuyerId { get; set; }
    public List<int> CourseIds { get; set; } = new();
}
