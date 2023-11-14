using System.ComponentModel.DataAnnotations;

namespace Appointment_Scheduler.RequestPayload
{
    public class UserRegisterRequest
    {
        public string Username { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters")]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string confirmPassword { get; set; } = string.Empty;
    }
}
