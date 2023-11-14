namespace Appointment_Scheduler.Dto
{
    public class AppointmentDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Appointment { get; set; }  
        public int NumberOfAttendees { get; set; }
        public string Location { get; set; }
    }
}
