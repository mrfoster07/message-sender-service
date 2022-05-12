using Microsoft.Extensions.Logging;
using NotificationSender.Domain;

namespace NotificationSender.IosProvider
{
    public interface INotificationSenderIosProvider
    {
        IosNotificationModel ParseModel(IDictionary<string, string> parameters);
        bool ValidateModel(IosNotificationModel model);
        Task<bool> ProcessNotification(IosNotificationModel model);
    }

    public sealed class NotificationSenderIosProvider : INotificationSenderBuilder<IosNotificationModel>,
        INotificationSenderIosProvider
    {
        private readonly ILogger<NotificationSenderIosProvider> logger;

        public NotificationSenderIosProvider(ILogger<NotificationSenderIosProvider> logger)
        {
            this.logger = logger;
        }
        
        private static int resetIndex = 0;

        private const int resetValue = 5;

        public IosNotificationModel ParseModel(IDictionary<string, string> parameters)
        {
            var result = new IosNotificationModel();

            foreach (var parameter in parameters)
            {
                switch (parameter.Key)
                {
                    case nameof(IosNotificationModel.Alert):
                        result.Alert = parameter.Value;
                        break;
                    case nameof(IosNotificationModel.IsBackground):
                        result.IsBackground =
                            !bool.TryParse(parameter.Value, out var isBackground) || isBackground;
                        break;
                    case nameof(IosNotificationModel.Priority):
                        result.Priority = int.TryParse(parameter.Value, out var priority) ? priority : 10;
                        break;
                    case nameof(IosNotificationModel.PushToken):
                        result.PushToken = parameter.Value;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ValidateModel(IosNotificationModel model)
        {
            return !string.IsNullOrEmpty(model.PushToken)
                   && model.PushToken.Length < 50
                   && !string.IsNullOrEmpty(model.Alert)
                   && model.Alert.Length < 2000;
        }

        public async Task<bool> ProcessNotification(IosNotificationModel model)
        {
            if (resetValue == Interlocked.Increment(ref resetIndex) 
                && resetValue == Interlocked.Exchange(ref resetIndex, 0))
            {
                return false;
            }

            var threadIdBegin = Thread.CurrentThread.ManagedThreadId;
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            await Task.Delay(new Random().Next(500, 2000));
            watch.Stop();
            var threadIdEnd = Thread.CurrentThread.ManagedThreadId;

            logger.LogDebug(
                $"ThreadId-Begin: {threadIdBegin}. ThreadId-End: {threadIdEnd}. Execution Time: {watch.ElapsedMilliseconds} ms.");

            return true;
        }
    }
}