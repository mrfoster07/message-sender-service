using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using NotificationSender.Domain;

namespace NotificationSender.AndroidProvider
{
    public interface INotificationSenderAndroidProvider
    {
        AndroidNotificationModel ParseModel(IDictionary<string, string> parameters);
        bool ValidateModel(AndroidNotificationModel model);
        Task<bool> ProcessNotification(AndroidNotificationModel model);
    }

    public sealed class NotificationSenderAndroidProvider : INotificationSenderBuilder<AndroidNotificationModel>,
        INotificationSenderAndroidProvider
    {
        private readonly ILogger<NotificationSenderAndroidProvider> logger;

        public NotificationSenderAndroidProvider(ILogger<NotificationSenderAndroidProvider> logger)
        {
            this.logger = logger;
        }

        private static int resetIndex = 0;

        private const int resetValue = 5;

        public AndroidNotificationModel ParseModel(IDictionary<string, string> parameters)
        {
            var result = new AndroidNotificationModel();

            foreach (var parameter in parameters)
            {
                switch (parameter.Key)
                {
                    case nameof(AndroidNotificationModel.Title):
                        result.Title = parameter.Value;
                        break;
                    case nameof(AndroidNotificationModel.Condition):
                        result.Condition = parameter.Value;
                        break;
                    case nameof(AndroidNotificationModel.DeviceToken):
                        result.DeviceToken = parameter.Value;
                        break;
                    case nameof(AndroidNotificationModel.Message):
                        result.Message = parameter.Value;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public bool ValidateModel(AndroidNotificationModel model)
        {
            return
                !string.IsNullOrEmpty(model.DeviceToken)
                && model.DeviceToken.Length < 50
                && !string.IsNullOrEmpty(model.Title)
                && model.Title.Length < 255
                && !string.IsNullOrEmpty(model.Message)
                && model.Message.Length < 2000
                && (string.IsNullOrEmpty(model.Condition)
                    || model.Condition.Length < 2000);
        }

        public async Task<bool> ProcessNotification(AndroidNotificationModel model)
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

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation($"ResetIndex is {resetIndex}/{resetValue}. Execution Time: {watch.ElapsedMilliseconds} ms.");
            }

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"ThreadId-Begin: {threadIdBegin}. ThreadId-End: {threadIdEnd}. {JsonSerializer.Serialize(model)}");
            }

            return true;
        }
    }
}