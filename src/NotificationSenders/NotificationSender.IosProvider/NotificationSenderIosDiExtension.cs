using Microsoft.Extensions.DependencyInjection;
using NotificationSender.Infrastructure;

namespace NotificationSender.IosProvider
{
    public static class NotificationSenderIosDiExtension
    {
        public static NotificationSenderConfiguration AddIosSender(
            this NotificationSenderConfiguration configuration)
        {
            configuration.AddProvider(s =>
                s.AddScoped<INotificationSenderIosProvider, NotificationSenderIosProvider>()
                    .AddScoped<INotificationSenderIosFacade, NotificationSenderIosFacade>());

            configuration.Bind<INotificationSenderIosFacade, NotificationSenderIosFacade>();
            return configuration;
        }
    }
}