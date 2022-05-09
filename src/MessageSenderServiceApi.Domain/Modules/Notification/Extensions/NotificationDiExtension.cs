using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSenderServiceApi.Domain.Modules.Notification.Extensions
{
    public static class NotificationDiExtension
    {
        public static IServiceCollection AddNotificationModule(this IServiceCollection serviceCollection)
        {

            return serviceCollection;
        }
    }
}