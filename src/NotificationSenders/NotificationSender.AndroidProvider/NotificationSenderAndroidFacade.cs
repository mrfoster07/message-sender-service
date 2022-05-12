using Microsoft.Extensions.Logging;
using NotificationSender.Domain;
using NotificationSender.Models;

namespace NotificationSender.AndroidProvider
{
    public interface INotificationSenderAndroidFacade
    {
        Task<NotificationSenderResultModel> ProcessNotification(IDictionary<string, string> parameters);
    }

    [NotificationSenderTitle("Android")]
    public sealed class NotificationSenderAndroidFacade : INotificationSenderFacade, INotificationSenderAndroidFacade
    {
        private readonly INotificationSenderAndroidProvider provider;

        public NotificationSenderAndroidFacade(INotificationSenderAndroidProvider provider)
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