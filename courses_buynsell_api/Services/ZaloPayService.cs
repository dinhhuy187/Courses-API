using courses_buynsell_api.Config;
using courses_buynsell_api.Interfaces;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace courses_buynsell_api.Services
{
    public class ZaloPayService : IZaloPayService
    {
        private readonly ZaloPayConfig _config;
        private readonly HttpClient _httpClient;

        public ZaloPayService(ZaloPayConfig config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<string> CreateOrderAsync(string orderId, decimal amount, string description)
        {
            var embedData = new { };
            var items = new object[] { };

            var data = new SortedDictionary<string, string>
            {
                { "app_id", _config.AppId },
                { "app_trans_id", $"{DateTime.Now:yyMMdd}_{orderId}" }, // Mã giao dịch unique
                { "app_user", "demo_user" },
                { "app_time", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() },
                { "amount", ((int)amount).ToString() },
                { "item", JsonConvert.SerializeObject(items) },
                { "embed_data", JsonConvert.SerializeObject(embedData) },
                { "description", description },
                { "bank_code", "" },
                { "callback_url", _config.CallbackUrl }
            };

            // 🔐 Tạo MAC theo ZaloPay yêu cầu
            string macInput = $"{_config.AppId}|{data["app_trans_id"]}|{data["app_user"]}|{data["amount"]}|{data["app_time"]}|{data["embed_data"]}|{data["item"]}";
            data.Add("mac", HmacSHA256(macInput, _config.Key1));

            var content = new FormUrlEncodedContent(data);

            var response = await _httpClient.PostAsync(_config.Endpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        private string HmacSHA256(string text, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "").ToLower();
        }
    }
}
