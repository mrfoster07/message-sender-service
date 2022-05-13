namespace MessageSenderServiceApi.Domain.Modules.NotificationDump;

public interface INotificationDumpingRepository
{

    Task AddRange((Guid id, string json, string jsonHash, bool status)[] items, CancellationToken cancellationToken);
}