using Appointment_Scheduler.Dto;
using Appointment_Scheduler.Services.EmailService;
using Appointment_Scheduler.Services.PushNotification;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_Scheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushNotificationController : Controller
    {
        private readonly IPushNotificationService _pushNotificationService;

        public PushNotificationController(IPushNotificationService pushNotificationService)
        {
            this._pushNotificationService = pushNotificationService;
        }

        [HttpPost("PushNotification")]
        public async Task<IActionResult> SendPushNotification([FromBody] PushMessageDto request)
        {
            await _pushNotificationService.SendMessageAsync(request);

            return Ok("message sent!");
        }
    }
}
