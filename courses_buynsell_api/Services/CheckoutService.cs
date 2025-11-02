using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using courses_buynsell_api.Entities;
using courses_buynsell_api.DTOs.Momo;
using courses_buynsell_api.Interfaces;
using courses_buynsell_api.Data;

public class CheckoutService : ICheckoutService
{
    private readonly MomoOptions _momo;
    private readonly AppDbContext _context;

    public CheckoutService(IOptions<MomoOptions> momoOptions, AppDbContext context)
    {
        _momo = momoOptions.Value;
        _context = context;
    }

    public async Task<string> CreateMomoPaymentAsync(CreateMomoPaymentRequestDto request)
    {
        string orderId = Guid.NewGuid().ToString();
        string requestId = Guid.NewGuid().ToString();
        string amount = ((long)request.Amount).ToString();

        // Raw hash string
        string rawHash = $"accessKey={_momo.AccessKey}&amount={amount}&extraData=&ipnUrl={_momo.NotifyUrl}&orderId={orderId}&orderInfo=Thanh toan khoa hoc&partnerCode={_momo.PartnerCode}&redirectUrl={_momo.ReturnUrl}&requestId={requestId}&requestType={_momo.RequestType}";

        string signature = HmacSHA256(rawHash, _momo.SecretKey);

        var body = new
        {
            partnerCode = _momo.PartnerCode,
            accessKey = _momo.AccessKey,
            requestId = requestId,
            amount = amount,
            orderId = orderId,
            orderInfo = "Thanh toan khoa hoc",
            redirectUrl = _momo.ReturnUrl,
            ipnUrl = _momo.NotifyUrl,
            extraData = "",
            requestType = _momo.RequestType,
            signature = signature,
            lang = "vi"
        };

        using var client = new HttpClient();
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(_momo.ApiUrl, content);
        var result = await response.Content.ReadAsStringAsync();

        var momoResponse = JsonConvert.DeserializeObject<MomoPaymentResponseDto>(result);

        // Lưu Transaction "Pending"
        var transaction = new Transaction
        {
            TransactionCode = orderId,
            PaymentMethod = "MOMO",
            TotalAmount = request.Amount,
            BuyerId = request.BuyerId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        foreach (var courseId in request.CourseIds)
        {
            _context.TransactionDetails.Add(new TransactionDetail
            {
                TransactionId = transaction.Id,
                CourseId = courseId,
                Price = request.Amount / request.CourseIds.Count
            });
        }
        await _context.SaveChangesAsync();

        return momoResponse.PayUrl;
    }

    public async Task HandleMomoCallbackAsync(Dictionary<string, string> formData)
    {
        string signature = formData["signature"];
        formData.Remove("signature");

        var rawHash = string.Join("&", formData.OrderBy(x => x.Key)
            .Select(x => $"{x.Key}={x.Value}"));
        string mySignature = HmacSHA256(rawHash, _momo.SecretKey);

        if (mySignature == signature && formData["resultCode"] == "0")
        {
            string orderId = formData["orderId"];
            var transaction = _context.Transactions.FirstOrDefault(t => t.TransactionCode == orderId);
            if (transaction != null)
            {
                transaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }

    private static string HmacSHA256(string text, string key)
    {
        var encoding = new UTF8Encoding();
        byte[] keyByte = encoding.GetBytes(key);
        byte[] messageBytes = encoding.GetBytes(text);
        using var hmacsha256 = new HMACSHA256(keyByte);
        byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
        return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
    }
}
