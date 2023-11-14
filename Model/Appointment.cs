using System.ComponentModel.DataAnnotations;

namespace Appointment_Scheduler.Model
{
    public class Appointment
    {
        public Guid AppointmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? AppointmentTitle { get; set; }
        public int NumberOfAttedees { get; set; }
        public string? Location { get; set; }
        public string AppointmentStatus { get; set; } = AppointmentStatusEnum.Pending.ToString();

        public Guid AppointmentUserId { get; set; }
        //public AppointmentUser CreatedBy { get; set; }
    }

    public enum AppointmentStatusEnum
    {
        [Display(Name = "Pending")]
        Pending = 1,
        [Display(Name = "Started")]
        Started,
        [Display(Name = "Ended")]
        Ended
    }
}
