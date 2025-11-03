using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Options;
using System.Globalization;
using courses_buynsell_api.Config;

namespace courses_buynsell_api.Services;

public class VnPayService
{
    private readonly VnPaySettings _settings;

    public VnPayService(IOptions<VnPaySettings> settings)
    {
        _settings = settings.Value;
    }

    public string CreatePaymentUrl(string transactionCode, decimal amount, string orderInfo, string ipAddress)
    {
        var vnpay = new VnPayLibrary();

        vnpay.AddRequestData("vnp_Version", _settings.Version);
        vnpay.AddRequestData("vnp_Command", _settings.Command);
        vnpay.AddRequestData("vnp_TmnCode", _settings.TmnCode);
        vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
        vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", ipAddress);
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
        vnpay.AddRequestData("vnp_OrderType", "other");
        vnpay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
        vnpay.AddRequestData("vnp_TxnRef", transactionCode);

        return vnpay.CreateRequestUrl(_settings.Url, _settings.HashSecret);
    }

    public bool ValidateSignature(Dictionary<string, string> queryParams, string inputHash)
    {
        var vnpay = new VnPayLibrary();
        foreach (var (key, value) in queryParams)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnpay.AddResponseData(key, value);
            }
        }

        string calculatedHash = vnpay.CreateResponseHash(inputHash, _settings.HashSecret);
        return calculatedHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }
}

public class VnPayLibrary
{
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _requestData.Add(key, value);
        }
    }

    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _responseData.Add(key, value);
        }
    }

    public string CreateRequestUrl(string baseUrl, string hashSecret)
    {
        StringBuilder data = new StringBuilder();
        foreach (var kv in _requestData)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                data.Append(HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value) + "&");
            }
        }

        string queryString = data.ToString();
        if (queryString.EndsWith("&"))
        {
            queryString = queryString.Substring(0, queryString.Length - 1);
        }

        string signData = queryString;
        string vnpSecureHash = HmacSHA512(hashSecret, signData);

        return $"{baseUrl}?{queryString}&vnp_SecureHash={vnpSecureHash}";
    }

    public string CreateResponseHash(string inputHash, string hashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in _responseData)
        {
            if (!string.IsNullOrEmpty(kv.Value) && kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
            {
                data.Append(HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value) + "&");
            }
        }

        string signData = data.ToString();
        if (signData.EndsWith("&"))
        {
            signData = signData.Substring(0, signData.Length - 1);
        }

        return HmacSHA512(hashSecret, signData);
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (byte b in hashValue)
            {
                hash.Append(b.ToString("x2"));
            }
        }

        return hash.ToString();
    }
}

public class VnPayCompare : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var vnpCompare = CompareInfo.GetCompareInfo("en-US");
        return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
    }
}