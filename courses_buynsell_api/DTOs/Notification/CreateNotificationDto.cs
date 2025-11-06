namespace courses_buynsell_api.DTOs.Notification;

public class CreateNotificationDto
{
    public string Message { get; set; } = string.Empty;
    public int SellerId { get; set; }
}