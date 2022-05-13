using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<NotificationSenderProxy> logger;

        public NotificationSenderProxy(
            ILogger<NotificationSenderProxy> logger,
            IServiceProvider serviceProvider,
            ProxySendersModel proxySenders)
        {
            this.logger = logger;
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
                    var result = await sender.ProcessNotification(parameters);

                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation(
                            $"Sender title: {senderTitle}. Notification: {JsonSerializer.Serialize(parameters)}. IsValid: {result.IsValid} IsDelivered: {result.IsDelivered}.");
                    }

                    return result;
                }
            }

            return new NotificationSenderResultModel();
        }
    }
}