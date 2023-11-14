using Appointment_Scheduler.Dto;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_Scheduler.Services.PushNotification
{
    public class PushNotificationService : IPushNotificationService
    {
        public async Task<bool> SendMessageAsync([FromBody] PushMessageDto request)
        {
            var message = new Message()
            {
                Notification = new Notification()
                {
                    Title = request.Title,
                    Body = request.Body,
                },
                Token = request.DeviceToken
            };

            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);

            if(!string.IsNullOrEmpty(result))
            {
                return true;
            }
            else
            {
                return false;
            }
   
        }
    }
}
