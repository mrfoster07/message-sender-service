using MessageSenderServiceApi.Contracts.Notification;

namespace MessageSenderServiceApi.Domain.Modules.Notification.Models
{
    public class NotificationCreateEventModel
    {
        public Guid Id { get; set; }

        public NotificationCreateModel Model { get; set; }

        public bool IsDelivered { get; set; }
    }
}