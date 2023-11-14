using System.ComponentModel.DataAnnotations;

namespace Appointment_Scheduler.RequestPayload
{
    public class UserLoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters")]
        public string Password { get; set; }
    }
}
