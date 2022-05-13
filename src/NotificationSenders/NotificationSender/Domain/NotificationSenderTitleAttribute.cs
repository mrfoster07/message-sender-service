namespace NotificationSender.Domain
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class NotificationSenderTitleAttribute : Attribute
    {
        public string Name { get; }

        public NotificationSenderTitleAttribute(string name)
        {
            this.Name = name;
        }
    }
}