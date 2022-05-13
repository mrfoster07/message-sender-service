using System.Collections.Concurrent;
using System.Text.Json;
using MessageSenderServiceApi.Contracts.Notification;
using MessageSenderServiceApi.Domain.Helpers;
using MessageSenderServiceApi.Domain.Modules.Notification.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MessageSenderServiceApi.Domain.Modules.NotificationDump
{
    public interface INotificationDumpingService
    {
        void AddParallel(Guid notificationId, bool notificationIsDelivered, NotificationCreateModel bodyModel);
        IEnumerable<NotificationCreateEventModel> Take(int batchSize);
        bool IsEmpty { get; }
        int MoveIndex();
    }

    public class NotificationDumpingService : INotificationDumpingService
    {
        private readonly ConcurrentBag<NotificationCreateEventModel>[] notificationBags;
        private readonly ILogger<NotificationDumpingService> logger;
        private readonly int notificationDumpTaskNumber = 10;

        public static int CurrentTaskIndex = 0;

        private static int CurrentTaskIncremetIndex = 0;

        private const int CurrentTaskIncremetIndexMax = 100;

        public NotificationDumpingService(
            ILogger<NotificationDumpingService> logger,
            IOptions<NotificationDumpingSettings> settings)
        {
            this.logger = logger;
            notificationDumpTaskNumber = settings.Value.TasksNumber;
            notificationBags = new ConcurrentBag<NotificationCreateEventModel>[notificationDumpTaskNumber];

            for (int i = 0; i < notificationBags.Length; i++)
            {
                notificationBags[i] = new ConcurrentBag<NotificationCreateEventModel>();
            }
        }

        public bool IsEmpty => notificationBags[CurrentTaskIndex].IsEmpty;

        public void AddParallel(Guid notificationId, bool notificationIsDelivered, NotificationCreateModel bodyModel)
        {
            //Math.Abs(notificationId.GetHashCode() % notificationDumpTaskNumber)
            notificationBags[CurrentTaskIndex].Add(
                new NotificationCreateEventModel
                {
                    Id = notificationId,
                    IsDelivered = notificationIsDelivered,
                    Model = bodyModel
                });
        }

        public int MoveIndex()
        {
            if (CurrentTaskIncremetIndexMax == Interlocked.Increment(ref CurrentTaskIncremetIndex)
                && CurrentTaskIncremetIndexMax == Interlocked.Exchange(ref CurrentTaskIncremetIndex, 0)
                && notificationDumpTaskNumber == Interlocked.Increment(ref CurrentTaskIndex)
                && notificationDumpTaskNumber == Interlocked.Exchange(ref CurrentTaskIndex, 0))
            {
                return CurrentTaskIndex;
            }

            return CurrentTaskIndex;
        }

        public IEnumerable<NotificationCreateEventModel> Take(int batchSize)
        {
            var increment = batchSize;
            while (notificationBags[CurrentTaskIndex]
                       .TryTake(out var localValue) && increment > 0)
            {
                --increment;
                yield return localValue;
            }
        }
    }
}