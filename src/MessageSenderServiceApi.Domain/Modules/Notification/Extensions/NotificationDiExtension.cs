using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageSenderServiceApi.Domain.Modules.Notification.Models;
using MessageSenderServiceApi.Domain.Modules.NotificationDump;
using Microsoft.Extensions.Configuration;
using NotificationSender;
using NotificationSender.AndroidProvider;
using NotificationSender.Infrastructure;
using NotificationSender.IosProvider;

namespace MessageSenderServiceApi.Domain.Modules.Notification.Extensions
{
    public static class NotificationDiExtension
    {
        public static IServiceCollection AddNotificationModule(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.AddScoped<INotificationService, NotificationService>();
            serviceCollection.AddSingleton<INotificationDumpingService, NotificationDumpingService>();
            serviceCollection.AddHostedService<NotificationDumpingBackgroundService>();

            serviceCollection.Configure<NotificationDumpingSettings>(
                configuration.GetSection("NotificationDumpingSettings"));

            serviceCollection.AddNotificationSenders(
                config =>
                    config.AddIosSender()
                        .AddAndroidSender());

            return serviceCollection;
        }
    }
}