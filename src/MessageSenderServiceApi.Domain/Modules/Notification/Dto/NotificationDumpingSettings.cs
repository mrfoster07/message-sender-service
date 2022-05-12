namespace MessageSenderServiceApi.Domain.Modules.Notification.Dto;

public class NotificationDumpingSettings
{
    public int DumpDelayTimespan { get; set; }
    public int BatchSize { get; set; }
}