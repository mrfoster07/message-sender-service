using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSenderServiceApi.Domain.Modules.Notification.Helpers
{
    internal static class NotificationStatusParseHelper
    {
        public static string GetStatusMessage(bool status) => status ? "Доставлено" : "Не доставлено";
    }
}