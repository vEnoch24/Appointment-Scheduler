namespace Appointment_Scheduler.RequestPayload
{
    public class AppointmentRequest
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string AppointmentTitle { get; set; }
        public int NumberOfAttedees { get; set;}
        public string Location { get; set; }
        public Guid AppointmentUserId { get; set; }
    }
}
