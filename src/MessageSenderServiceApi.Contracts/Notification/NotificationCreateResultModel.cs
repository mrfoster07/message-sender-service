using System;
using System.Collections.Generic;

namespace MessageSenderServiceApi.Contracts.Notification
{
    public class NotificationCreateResultModel
    {
        public Guid Id { get; set; }

        public string Status { get; set; }
    }
}