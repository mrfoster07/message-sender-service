using Microsoft.Extensions.Logging;

namespace MessageSenderServiceApi.Domain.Modules.Notification;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly INotificationRepository _repository;

    public NotificationService(ILogger<NotificationService> logger, INotificationRepository repository)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task CreateNotification()
    {
        //   _repository.
    }
}