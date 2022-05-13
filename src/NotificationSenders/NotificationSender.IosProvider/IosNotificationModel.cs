namespace NotificationSender.IosProvider;

public class IosNotificationModel
{
    public string PushToken { get; set; }
    public string Alert { get; set; }
    public int Priority { get; set; }
    public bool IsBackground { get; set; }
}