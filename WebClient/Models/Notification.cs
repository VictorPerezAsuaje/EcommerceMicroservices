namespace WebClient.Models;

public enum NotificationIcon { error, info, success, warning }

public class Notification
{
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationIcon Icon { get; set; } = NotificationIcon.info;
    public string? ButtonText { get; set; } = null;
}