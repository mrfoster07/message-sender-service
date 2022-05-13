using MessageSenderServiceApi.Contracts.Notification;
using MessageSenderServiceApi.Domain.Modules.Notification;
using Microsoft.AspNetCore.Mvc;

namespace MessageSenderServiceApi.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> logger;
        private readonly INotificationService notificationService;

        public NotificationsController(ILogger<NotificationsController> logger,
            INotificationService notificationService)
        {
            this.logger = logger;
            this.notificationService = notificationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(NotificationCreateResultModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNotification(NotificationCreateModel model)
        {
            var result = await notificationService.CreateNotification(model); 
            return Ok(result);
        }

        [HttpGet("{id}/status")]
        [ProducesResponseType(typeof(NotificationStatusModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotificationStatus(Guid id, CancellationToken cancellationToken)
        {
            var result = await notificationService.GetNotificationStatus(id, cancellationToken);
            return Ok(result);
        }
    }
}