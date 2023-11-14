using System.ComponentModel.DataAnnotations;

namespace Appointment_Scheduler.RequestPayload
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token {get; set; }
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters")]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string confirmPassword { get; set; } = string.Empty;
    }
}
