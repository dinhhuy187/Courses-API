namespace courses_buynsell_api.DTOs.Transaction;

public class CreateOrderRequest
{
    public int app_id { get; set; }
    public string app_user { get; set; } = string.Empty;
    public string app_trans_id { get; set; } = string.Empty;// e.g. "yyMMdd_xxx" recommended
    public long app_time { get; set; } // unix ms
    public int amount { get; set; }
    public string item { get; set; } = string.Empty;// "[]"
    public string embed_data { get; set; } = string.Empty;// json string
    public string description { get; set; } = string.Empty;
    public string callback_url { get; set; } = string.Empty;
    public string mac { get; set; } = string.Empty;
}