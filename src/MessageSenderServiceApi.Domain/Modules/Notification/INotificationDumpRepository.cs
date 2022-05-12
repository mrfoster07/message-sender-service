namespace MessageSenderServiceApi.Domain.Modules.Notification;

public interface INotificationDumpRepository
{

    Task AddRange((Guid id, string json, string jsonHash, bool status)[] items, CancellationToken cancellationToken);
}