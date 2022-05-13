namespace MessageSenderServiceApi.Domain.Modules.Notification.Models;

public class NotificationDumpingSettings
{
    public int DumpDelayTimespan { get; set; }
    public int BatchSize { get; set; }
    public bool SaveInBatch { get; set; }
    public int TasksNumber { get; set; }
}