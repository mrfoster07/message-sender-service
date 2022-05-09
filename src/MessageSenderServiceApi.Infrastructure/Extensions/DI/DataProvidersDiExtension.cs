using MessageSenderServiceApi.Infrastructure.Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageSenderServiceApi.Infrastructure.Extensions.DI
{
    public static class DataProvidersDiExtension
    {
        public static IServiceCollection AddProviders(this IServiceCollection serviceCollection,
            IConfigurationBuilder configuration)
        {
            serviceCollection.AddDbContext<NotificationDataContext>(opt => opt.UseInMemoryDatabase("NotificationDB"));
            return serviceCollection;
        }
    }
}