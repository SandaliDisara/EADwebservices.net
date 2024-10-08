using EADwebservice.Models;
using EADwebservice.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using YourNamespace.Models;

namespace EADwebservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // POST: api/notification
        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification([FromBody] Notification notification)
        {
            var createdNotification = await _notificationService.CreateNotificationAsync(notification);
            return CreatedAtAction(nameof(GetNotificationById), new { id = createdNotification.NotificationId }, createdNotification);
        }

        // GET: api/notification
        [HttpGet]
        public async Task<ActionResult<List<Notification>>> GetNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        // GET: api/notification/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotificationById(string id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }

        // DELETE: api/notification/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(string id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            if (result)
            {
                return NoContent(); // 204 No Content
            }
            return NotFound(); // 404 Not Found if the notification was not found
        }
    }
}
