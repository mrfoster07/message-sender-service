using MessageSenderServiceApi.Domain.Modules.Notification;
using MessageSenderServiceApi.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MessageSenderServiceApi.Infrastructure.Extensions.DI
{
    public static class RepositoriesDiExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<INotificationRepository, NotificationRepository>();

            return serviceCollection;
        }
    }
}