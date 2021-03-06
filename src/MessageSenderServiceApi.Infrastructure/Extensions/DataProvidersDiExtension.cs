using MessageSenderServiceApi.Infrastructure.Data.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageSenderServiceApi.Infrastructure.Extensions
{
    public static class DbMigrationExtension
    { 
        public static IApplicationBuilder RunMigrationsIfNeeds(this IApplicationBuilder app,
            IConfiguration configuration)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotificationDataContext>>();
            try
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<NotificationDataContext>();
                if (configuration.GetValue<bool>("RunMigrationsIfNeeds"))
                {
                    logger.LogInformation("Migration started");
                    dataContext.Database.MigrateAsync().Wait();
                    logger.LogInformation("Migration finished");
                }
            } catch (Exception e)
            {
                logger.LogError(e.ToString());
            }

            return app;
        }
    }
}