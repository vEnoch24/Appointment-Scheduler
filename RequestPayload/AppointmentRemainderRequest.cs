using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Appointment_Scheduler.RequestPayload
{
    public class AppointmentRemainderRequest
    {
        public Guid AppointmentId { get; set; }
        [EmailAddress]
        public string email { get; set; }  
    }
}
