using System.Text.Json;
using MessageSenderServiceApi.Contracts.Notification;
using MessageSenderServiceApi.Domain.Helpers;
using MessageSenderServiceApi.Domain.Modules.Notification.Helpers;
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
    private readonly ILogger<NotificationService> logger;

    private readonly INotificationRepository repository;

    private readonly INotificationSenderProxy notificationSenderProxy;

    public NotificationService(
        ILogger<NotificationService> logger,
        INotificationRepository repository,
        INotificationSenderProxy notificationSenderProxy)
    {
        this.logger = logger;
        this.repository = repository;
        this.notificationSenderProxy = notificationSenderProxy;
    }


    public async Task<NotificationCreateResultModel> CreateNotification(NotificationCreateModel model)
    {
        var result = new NotificationCreateResultModel();

        if (string.IsNullOrEmpty(model.TargetType) || model.Parameters.Count == 0)
        {
            result.Status =
                NotificationStatusParseHelper.GetStatusMessage(false);
            return result;
        }

        result.Id = Guid.NewGuid();

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
        var hashedJsonString = StringHashHelper.GetStringHash(json);

        await repository.Add(
            (notificationId,
                json,
                hashedJsonString,
                notificationIsDelivered),
            default);
    }
}