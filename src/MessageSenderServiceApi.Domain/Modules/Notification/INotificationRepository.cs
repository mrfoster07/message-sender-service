using MessageSenderServiceApi.Domain.Modules.Notification.Entities;

namespace MessageSenderServiceApi.Domain.Modules.Notification;

public interface INotificationRepository
{
    Task Add(NotificationEntity entity);
    Task UpdateStatus(Guid id, bool status);
    Task<bool> HasStatus(Guid id, bool status);
}