namespace courses_buynsell_api.Interfaces
{
    public interface IZaloPayService
    {
        Task<string> CreateOrderAsync(string orderId, decimal amount, string description);
    }
}
