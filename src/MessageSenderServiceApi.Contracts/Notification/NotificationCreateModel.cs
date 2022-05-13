using System.Collections.Generic;

namespace MessageSenderServiceApi.Contracts.Notification
{
    public class NotificationCreateModel
    {
        /// <summary>
        /// Тип получателя
        /// </summary>
        public string TargetType { get; set; }

        /// <summary>
        /// Параметры
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }
    }
}