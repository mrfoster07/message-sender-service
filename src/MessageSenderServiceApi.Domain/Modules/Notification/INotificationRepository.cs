namespace MessageSenderServiceApi.Domain.Modules.Notification;

public interface INotificationRepository
{

    /// <summary>
    /// Добавление уведомления
    /// </summary>
    /// <returns></returns>
    Task Add((Guid id, string json, string jsonHash, bool status) item, CancellationToken cancellationToken);

    /// <summary>
    /// Является ли уведомление в заданном статусе
    /// </summary>
    /// <returns></returns>
    Task<bool> IsStatus(Guid id, bool status, CancellationToken cancellationToken);
}