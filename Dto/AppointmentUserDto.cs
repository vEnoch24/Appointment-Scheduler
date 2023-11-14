namespace Appointment_Scheduler.Dto
{
    public class AppointmentUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordRestToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
    }
}
