using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSenderServiceApi.Domain.Modules.Notification.Entities
{
    public class NotificationEntity
    {
        public Guid Id { get; set; }

        public string Json { get; set; }

        public bool IsDelivered { get; set; }
    }
}