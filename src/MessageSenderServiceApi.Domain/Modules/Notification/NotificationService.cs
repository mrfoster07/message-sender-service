using System.Text.Json;
using MessageSenderServiceApi.Contracts.Notification;
using MessageSenderServiceApi.Domain.Helpers;
using MessageSenderServiceApi.Domain.Modules.Notification.Dto;
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
    private readonly ILogger<NotificationService> _logger;

    private readonly INotificationRepository _repository;

    private readonly INotificationDumpingService _notificationDumpingService;
    private readonly INotificationSenderProxy _notificationSenderProxy;

    public NotificationService(
        ILogger<NotificationService> logger,
        INotificationRepository repository,
        INotificationDumpingService notificationDumpingService,
        INotificationSenderProxy notificationSenderProxy)
    {
        _logger = logger;
        _repository = repository;
        _notificationDumpingService = notificationDumpingService;
        _notificationSenderProxy = notificationSenderProxy;
    }


    //CancellationToken  - при отправке нужно не дожидаться, он не нужен. А при получении необходим.
    //Validation - нужно все перепроверить
    //репозитории
    //отладка


    /// <summary>
    /// По условию нет необходимости фильтровать дубликаты,
    /// производится рассылка всех без исключения нотификаций,
    /// а прочесс сохранения сообщения и результата его выполнение произфодится в паралельной задаче.
    ///
    /// если есть проблема с СУБД, нужно перестать вынимать сообщения из очереди
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>в
    /// 
    public async Task<NotificationCreateResultModel> CreateNotification(NotificationCreateModel model)
    {
        var result = new NotificationCreateResultModel();
        result.Id = Guid.NewGuid();

        var isDelivered = await _notificationSenderProxy.ProcessNotification(model.TargetType, model.Parameters);
        result.Status =
            NotificationStatusParseHelper.GetStatusMessage(isDelivered);

        _notificationDumpingService.AddParallel(result.Id, isDelivered, model);
        // await Add(result.Id, isDelivered, model);

        return result;
    }

    public async Task<NotificationStatusModel> GetNotificationStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = new NotificationStatusModel();

        var isDelivered = await _repository.IsStatus(id, true, cancellationToken);
        result.Status =
            NotificationStatusParseHelper.GetStatusMessage(isDelivered);

        return result;
    }

    private async Task Add(Guid notificationId, bool notificationIsDelivered,
        NotificationCreateModel bodyModel)
    {
        var json = JsonSerializer.Serialize(bodyModel);
        var hashedJsonString = StringHashHelper.GetStringHash(json);

        await _repository.Add(
            (notificationId,
                json,
                hashedJsonString,
                notificationIsDelivered),
            default);
    }
}