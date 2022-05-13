namespace NotificationSender.Domain;

public interface INotificationSenderBuilder<TNotificationModel>
{
    public TNotificationModel ParseModel(IDictionary<string, string> parameters);

    public bool ValidateModel(TNotificationModel model);

    public Task<bool> ProcessNotification(TNotificationModel model);
}