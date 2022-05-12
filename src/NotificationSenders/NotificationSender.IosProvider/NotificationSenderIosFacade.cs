using NotificationSender.Domain;
using NotificationSender.Models;

namespace NotificationSender.IosProvider
{
    public interface INotificationSenderIosFacade
    {
        Task<NotificationSenderResultModel> ProcessNotification(IDictionary<string, string> parameters);
    }

    [NotificationSenderTitle("iOS")]
    public sealed class NotificationSenderIosFacade : INotificationSenderFacade, INotificationSenderIosFacade
    {
        private readonly INotificationSenderIosProvider provider;

        public NotificationSenderIosFacade(INotificationSenderIosProvider provider)
        {
            this.provider = provider;
        }

        public async Task<NotificationSenderResultModel> ProcessNotification(IDictionary<string, string> parameters)
        {
            var result = new NotificationSenderResultModel();

            var parsedModel = provider.ParseModel(parameters);

            result.IsValid = provider.ValidateModel(parsedModel);

            if (result.IsValid)
            {
                result.IsDelivered = await provider.ProcessNotification(parsedModel);
            }

            return result;
        }
    }
}