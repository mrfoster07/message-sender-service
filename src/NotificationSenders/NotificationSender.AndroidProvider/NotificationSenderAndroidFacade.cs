using Microsoft.Extensions.Logging;
using NotificationSender.Domain;

namespace NotificationSender.AndroidProvider
{
    public interface INotificationSenderAndroidFacade
    {
        Task<bool> ProcessNotification(IDictionary<string, string> parameters);
    }

    [NotificationSenderTitle("Android")]
    public sealed class NotificationSenderAndroidFacade : INotificationSenderFacade, INotificationSenderAndroidFacade
    {
        private readonly INotificationSenderAndroidProvider provider;

        public NotificationSenderAndroidFacade(INotificationSenderAndroidProvider provider)
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