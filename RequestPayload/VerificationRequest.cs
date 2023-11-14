using System.ComponentModel.DataAnnotations;

namespace Appointment_Scheduler.RequestPayload
{
    public class VerificationRequest
    {
        public Guid userId { get; set; }
        public string token { get; set; }
    }
}
