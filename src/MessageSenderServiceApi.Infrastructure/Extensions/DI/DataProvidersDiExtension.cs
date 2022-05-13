using MessageSenderServiceApi.Infrastructure.Data.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageSenderServiceApi.Infrastructure.Extensions.DI
{
    public static class DataProvidersDiExtension
    {
        public static IServiceCollection AddDataProviders(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                serviceCollection.AddDbContext<NotificationDataContext>(
                    opt =>
                        opt.UseInMemoryDatabase("MessageSenderServiceDb"));
            } else
            {
                var dbConnectionString =
                    configuration.GetConnectionString("MessageSenderServiceDb");

                serviceCollection.AddDbContext<NotificationDataContext>(options =>
                {
                    options.UseNpgsql(dbConnectionString);
                });
            }

            return serviceCollection;
        }
    }
}