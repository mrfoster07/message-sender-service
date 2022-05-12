namespace MessageSenderServiceApi.Domain.Modules.Notification;

public interface INotificationRepository
{
    Task Add((Guid id, string json, string jsonHash, bool status) item, CancellationToken cancellationToken);
    Task<bool> IsStatus(Guid id, bool status, CancellationToken cancellationToken);
}