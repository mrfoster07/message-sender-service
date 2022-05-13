namespace MessageSenderServiceApi.Infrastructure.Data.DataContext.NotificationEntities
{
    public class NotificationEntity
    {
        public Guid Id { get; set; }

        public string JsonHash { get; set; }

        public string Json { get; set; }

        public bool IsDelivered { get; set; }
    }
}