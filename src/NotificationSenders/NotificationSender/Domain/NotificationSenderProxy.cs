using Microsoft.Extensions.DependencyInjection;
using NotificationSender.Models;

namespace NotificationSender.Domain
{
    public interface INotificationSenderProxy
    {
        Task<NotificationSenderResultModel> ProcessNotification(string senderTitle,
            IDictionary<string, string> parameters);
    }

    public sealed class NotificationSenderProxy : INotificationSenderProxy
    {
        private readonly ProxySendersModel _proxySenders;
        private readonly IServiceProvider _serviceProvider;

        public NotificationSenderProxy(
            IServiceProvider serviceProvider,
            ProxySendersModel proxySenders)
        {
            _serviceProvider = serviceProvider;
            _proxySenders = proxySenders;
        }

        public async Task<NotificationSenderResultModel> ProcessNotification(string senderTitle,
            IDictionary<string, string> parameters)
        {
            if (_proxySenders.NotificationSenders.TryGetValue(senderTitle, out var senderType))
            {
                if (_serviceProvider.GetRequiredService(senderType) is INotificationSenderFacade sender)
                {
                    return await sender.ProcessNotification(parameters);
                }
            }

            return new NotificationSenderResultModel();
        }
    }
}