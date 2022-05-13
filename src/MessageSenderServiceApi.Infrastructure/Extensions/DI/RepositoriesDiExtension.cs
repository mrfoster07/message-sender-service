using MessageSenderServiceApi.Domain.Modules.Notification;
using MessageSenderServiceApi.Domain.Modules.NotificationDump;
using MessageSenderServiceApi.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MessageSenderServiceApi.Infrastructure.Extensions.DI
{
    public static class RepositoriesDiExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<INotificationRepository, NotificationRepository>();
            serviceCollection.AddScoped<INotificationDumpingRepository, NotificationDumpRepository>();

            return serviceCollection;
        }
    }
}