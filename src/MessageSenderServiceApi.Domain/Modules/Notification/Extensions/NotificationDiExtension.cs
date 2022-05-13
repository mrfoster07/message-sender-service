using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationSender;
using NotificationSender.AndroidProvider;
using NotificationSender.Infrastructure;
using NotificationSender.IosProvider;

namespace MessageSenderServiceApi.Domain.Modules.Notification.Extensions
{
    public static class NotificationDiExtension
    {
        public static IServiceCollection AddNotificationModule(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<INotificationService, NotificationService>();

            serviceCollection.AddNotificationSenders(
                config =>
                    config.AddIosSender()
                        .AddAndroidSender());

            return serviceCollection;
        }
    }
}