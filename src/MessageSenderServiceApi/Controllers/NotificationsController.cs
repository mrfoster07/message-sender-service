using MessageSenderServiceApi.Contracts.Notification;
using Microsoft.AspNetCore.Mvc;

namespace MessageSenderServiceApi.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ILogger<NotificationsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(NotificationStatusModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotificationStatusModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNotification(CancellationToken cancellationToken, NotificationCreateModel model)
        {
            return Ok();
        }

        [HttpGet("{id}/status")]
        [ProducesResponseType(typeof(NotificationStatusModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotificationStatus(CancellationToken cancellationToken, Guid id)
        { 
            return Ok();
        }
    }
}