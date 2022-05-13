using System.Text.Json;
using MessageSenderServiceApi.Contracts.Notification;
using MessageSenderServiceApi.Domain.Helpers;
using MessageSenderServiceApi.Domain.Modules.Notification.Helpers;
using MessageSenderServiceApi.Domain.Providers;
using Microsoft.Extensions.Logging;
using NotificationSender.Domain;

namespace MessageSenderServiceApi.Domain.Modules.Notification;

public interface INotificationService
{
    Task<NotificationCreateResultModel> CreateNotification(
        NotificationCreateModel model);

    Task<NotificationStatusModel> GetNotificationStatus(
        Guid id,
        CancellationToken cancellationToken);
}

public class NotificationService : INotificationService
{
    private const int maxNotificationCreateModelSize = 100;

    private readonly ILogger<NotificationService> logger;

    private readonly INotificationRepository repository;

    private readonly INotificationSenderProxy notificationSenderProxy;

    private readonly IGuidProvider guidProvider;

    private readonly IStringHashHelper stringHashHelper;

    public NotificationService(
        ILogger<NotificationService> logger,
        INotificationRepository repository,
        INotificationSenderProxy notificationSenderProxy,
        IGuidProvider guidProvider,
        IStringHashHelper stringHashHelper)
    {
        this.logger = logger;
        this.repository = repository;
        this.notificationSenderProxy = notificationSenderProxy;
        this.guidProvider = guidProvider;
        this.stringHashHelper = stringHashHelper;
    }


    public async Task<NotificationCreateResultModel> CreateNotification(NotificationCreateModel model)
    {
        var result = new NotificationCreateResultModel();

        if (string.IsNullOrEmpty(model.TargetType)
            || model.Parameters.Count == 0
            || model.Parameters.Count > maxNotificationCreateModelSize)
        {
            result.Status =
                NotificationStatusParseHelper.GetStatusMessage(false);
            return result;
        }

        result.Id = guidProvider.CreateGuid();

        var notificationResult =
            await notificationSenderProxy.ProcessNotification(model.TargetType, model.Parameters);

        if (notificationResult.IsValid)
        {
            await SaveNotificationWithResult(result.Id, notificationResult.IsDelivered, model);
        }

        result.Status =
            NotificationStatusParseHelper.GetStatusMessage(notificationResult.IsDelivered);

        return result;
    }

    public async Task<NotificationStatusModel> GetNotificationStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = new NotificationStatusModel();

        if (id == Guid.Empty)
        {
            result.Status =
                NotificationStatusParseHelper.GetStatusMessage(false);
            return result;
        }

        var isDelivered = await repository.IsStatus(id, true, cancellationToken);
        result.Status =
            NotificationStatusParseHelper.GetStatusMessage(isDelivered);

        return result;
    }

    private async Task SaveNotificationWithResult(
        Guid notificationId,
        bool notificationIsDelivered,
        NotificationCreateModel bodyModel)
    {
        var json = JsonSerializer.Serialize(bodyModel);
        var hashedJsonString = stringHashHelper.GetStringHashSHA512(json);

        await repository.Add(
            (notificationId,
                json,
                hashedJsonString,
                notificationIsDelivered),
            default);
    }
}