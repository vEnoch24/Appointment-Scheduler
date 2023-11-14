using Appointment_Scheduler.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_Scheduler.Services.PushNotification
{
    public interface IPushNotificationService
    {
        Task<bool> SendMessageAsync([FromBody] PushMessageDto request);
    }
}
