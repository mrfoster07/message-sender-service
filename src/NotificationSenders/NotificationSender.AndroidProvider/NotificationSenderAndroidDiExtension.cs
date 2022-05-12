using Microsoft.Extensions.DependencyInjection;
using NotificationSender.Infrastructure;

namespace NotificationSender.AndroidProvider
{
    public static class NotificationSenderAndroidDiExtension
    {
        public static NotificationSenderConfiguration AddAndroidSender(
            this NotificationSenderConfiguration configuration)
        {
            configuration.AddProvider(s =>
                s.AddScoped<INotificationSenderAndroidProvider, NotificationSenderAndroidProvider>()
                    .AddScoped<INotificationSenderAndroidFacade, NotificationSenderAndroidFacade>());

            configuration.Bind<INotificationSenderAndroidFacade, NotificationSenderAndroidFacade>();
            return configuration;
        }
    }
}