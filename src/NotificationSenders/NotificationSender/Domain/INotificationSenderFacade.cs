using NotificationSender.Models;

namespace NotificationSender.Domain;

public interface INotificationSenderFacade
{
    public Task<NotificationSenderResultModel> ProcessNotification(IDictionary<string, string> parameters);
}