using System.ComponentModel.DataAnnotations;

namespace Appointment_Scheduler.RequestPayload
{
    public class EmailVerificationRequest
    {
        public string token { get; set; }
    }
}
