using System.ComponentModel.DataAnnotations;

namespace Appointment_Scheduler.Model
{
    public class AppointmentUser
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] passwordhash { get; set; } = new byte[32];
        public byte[] passwordSalt { get; set; } = new byte[32];
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set;}
        public string? PasswordRestToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }    


        public List<Appointment> Appointments { get; set; }

    }
}
