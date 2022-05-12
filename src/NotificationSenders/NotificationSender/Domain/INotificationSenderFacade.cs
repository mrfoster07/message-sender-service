namespace NotificationSender.Domain;

public interface INotificationSenderFacade
{
    public Task<bool> ProcessNotification(IDictionary<string, string> parameters);
}