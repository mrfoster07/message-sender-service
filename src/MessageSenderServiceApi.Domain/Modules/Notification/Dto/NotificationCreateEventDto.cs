using MessageSenderServiceApi.Contracts.Notification;

namespace MessageSenderServiceApi.Domain.Modules.Notification.Dto
{
    public class NotificationCreateEventDto
    {
        public Guid Id { get; set; }

        public NotificationCreateModel Model { get; set; }

        public bool IsDelivered { get; set; }
    }
}