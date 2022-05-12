using NotificationSender.Domain;

namespace NotificationSender.IosProvider
{
    public interface INotificationSenderIosFacade
    {
        Task<bool> ProcessNotification(IDictionary<string, string> parameters);
    }

    [NotificationSenderTitle("iOS")]
    public sealed class NotificationSenderIosFacade : INotificationSenderFacade, INotificationSenderIosFacade
    {
        private readonly INotificationSenderIosProvider provider;

        public NotificationSenderIosFacade(INotificationSenderIosProvider provider)
        {
            this.provider = provider;
        }

        public async Task<bool> ProcessNotification(IDictionary<string, string> parameters)
        {
            var parsedModel = provider.ParseModel(parameters);
            if (provider.ValidateModel(parsedModel))
            {
                return await provider.ProcessNotification(parsedModel);
            }

            return false;
        }
    }
}