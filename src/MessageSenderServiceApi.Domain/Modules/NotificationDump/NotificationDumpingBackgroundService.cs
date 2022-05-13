using System.Text.Json;
using MessageSenderServiceApi.Domain.Helpers;
using MessageSenderServiceApi.Domain.Modules.Notification.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MessageSenderServiceApi.Domain.Modules.NotificationDump;

public class NotificationDumpingBackgroundService : BackgroundService
{
    private readonly ILogger<NotificationDumpingBackgroundService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly INotificationDumpingService notificationDumpingService;
    private CancellationToken notificationDumpToken;
    private readonly Task[] notificationDumpTask;

    private readonly int dumpDelayTimespan = 10;
    private readonly int notificationDumpTaskNumber = 10;
    private readonly int batchSize = 5;

    public NotificationDumpingBackgroundService(
        ILogger<NotificationDumpingBackgroundService> logger,
        IServiceProvider serviceProvider,
        INotificationDumpingService notificationDumpingService,
        IOptions<NotificationDumpingSettings> settings)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.notificationDumpingService = notificationDumpingService;
        dumpDelayTimespan = settings.Value.DumpDelayTimespan;
        notificationDumpTaskNumber = settings.Value.TasksNumber;
        batchSize = settings.Value.BatchSize;
        this.notificationDumpTask = new Task[notificationDumpTaskNumber];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            $"{nameof(NotificationDumpingBackgroundService)} is running.");

        this.notificationDumpToken = stoppingToken;

        for (int i = 0; i < notificationDumpTaskNumber; i++)
        {
            this.notificationDumpTask[i] = createNotificationDumpTask();
            this.notificationDumpTask[i].Start();
        }
    }

    private Task createNotificationDumpTask()
    {
        return new Task(async () =>
        {
            try
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                var repository =
                    scope.ServiceProvider.GetRequiredService<INotificationDumpingRepository>();
                var stringHashHelper =
                    scope.ServiceProvider.GetRequiredService<IStringHashHelper>();

                do
                {
                    if (!notificationDumpingService.IsEmpty)
                    {
                        try
                        {
                            await dumpNotifications(repository, stringHashHelper);
                        } catch (Exception e)
                        {
                            logger.LogError(e.ToString());
                        }
                    } else
                    {
                        await Task.Delay(dumpDelayTimespan, notificationDumpToken);
                    }
                    notificationDumpingService.MoveIndex();

                } while (!notificationDumpToken.IsCancellationRequested);
            } catch (OperationCanceledException)
            {
                logger.LogInformation("CreateNotificationDump task is canceled");
            }
        }, notificationDumpToken);
    }

    private async Task dumpNotifications(INotificationDumpingRepository repository,
        IStringHashHelper stringHashHelper)
    {
        var items = notificationDumpingService.Take(batchSize)
            .Select(s =>
            {
                var json = JsonSerializer.Serialize(s.Model);
                var hashedJsonString = stringHashHelper.GetStringHashSHA512(json);

                return (s.Id, json, hashedJsonString, s.IsDelivered);
            })
            .ToArray();

        if (items.Length > 0)
        {
            await repository.AddRange(items, notificationDumpToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            $"{nameof(NotificationDumpingBackgroundService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}