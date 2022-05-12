namespace NotificationSender.Domain
{
    public class ProxySendersModel
    {
        public ProxySendersModel()
        {
            NotificationSenders = new Dictionary<string, Type>();
        }

        public Dictionary<string, Type> NotificationSenders { get; set; }
    }
}