
namespace courses_buynsell_api.Config;

public class VnPaySettings
{
    public string TmnCode { get; set; } = string.Empty;
    public string HashSecret { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
}