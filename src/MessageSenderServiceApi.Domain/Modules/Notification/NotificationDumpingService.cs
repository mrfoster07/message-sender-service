using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MessageSenderServiceApi.Contracts.Notification;
using MessageSenderServiceApi.Domain.Helpers;
using MessageSenderServiceApi.Domain.Modules.Notification.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MessageSenderServiceApi.Domain.Modules.Notification
{
    public interface INotificationDumpingService
    {
        void AddParallel(Guid notificationId, bool notificationIsDelivered, NotificationCreateModel bodyModel);
    }

    public class NotificationDumpingService : IDisposable, INotificationDumpingService
    {
        private readonly ConcurrentBag<NotificationCreateEventDto> _notificationQueue;
        private readonly ILogger<NotificationDumpingService> _logger;
        private readonly INotificationDumpRepository _repository;
        private readonly Task notificationDumpTask;

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly CancellationToken _notificationDumpToken;

        private const int _dumpDelayTimespan = 10;

        private const int _batchSize = 5;


        public NotificationDumpingService(
            ILogger<NotificationDumpingService> logger,
            INotificationDumpRepository repository)
        {
            _logger = logger;
            _repository = repository;
            _notificationQueue = new ConcurrentBag<NotificationCreateEventDto>();
            _notificationDumpToken = _cancellationTokenSource.Token;
            notificationDumpTask = createNotificationDumpTask();
            notificationDumpTask.Start();
        }

        public void AddParallel(Guid notificationId, bool notificationIsDelivered, NotificationCreateModel bodyModel)
        {
            _notificationQueue.Add(new NotificationCreateEventDto
            {
                Id = notificationId,
                IsDelivered = notificationIsDelivered,
                Model = bodyModel
            });
        }

        private IEnumerable<NotificationCreateEventDto> dequeueNotifications()
        {
            var increment = _batchSize;
            while (_notificationQueue.TryTake(out var localValue) && increment > 0)
            {
                --increment;
                yield return localValue;
            }
        }

        private Task createNotificationDumpTask()
        {
            return new Task(async () =>
            {
                try
                {
                    do
                    {
                        if (!_notificationQueue.IsEmpty)
                        {
                            try
                            {
                                await dumpNotifications();
                            } catch (Exception e)
                            {
                                _logger.LogError(e.ToString());
                            }
                        } else
                        {
                            await Task.Delay(_dumpDelayTimespan, _notificationDumpToken);
                        }
                    } while (!_notificationDumpToken.IsCancellationRequested);
                } catch (OperationCanceledException)
                {
                    _logger.LogInformation("CreateNotificationDump task is canceled");
                }
            }, _notificationDumpToken);
        }

        private async Task dumpNotifications()
        {
            var items = dequeueNotifications()
                .Select(s =>
                {
                    var json = JsonSerializer.Serialize(s.Model);
                    var hashedJsonString = StringHashHelper.GetStringHash(json);

                    return (s.Id, json, hashedJsonString, s.IsDelivered);
                })
                .ToArray();

            if (items.Length > 0)
            {
                await _repository.AddRange(items, _notificationDumpToken);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}