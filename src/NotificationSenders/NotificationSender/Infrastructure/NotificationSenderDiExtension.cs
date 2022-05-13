using Microsoft.Extensions.DependencyInjection;
using NotificationSender.Domain;

namespace NotificationSender.Infrastructure;

public static class NotificationSenderDiExtension
{
    public static IServiceCollection AddNotificationSenders(
        this IServiceCollection serviceCollection,
        Action<NotificationSenderConfiguration> configurationAction)
    {
        var notificationSenderConfiguration = new NotificationSenderConfiguration(serviceCollection);
        configurationAction.Invoke(notificationSenderConfiguration);

        notificationSenderConfiguration.AddProvidersProxy();

        serviceCollection.AddScoped<INotificationSenderProxy, NotificationSenderProxy>();

        return serviceCollection;
    }
}