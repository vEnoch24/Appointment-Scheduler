namespace Appointment_Scheduler.RequestPayload
{
    public class EditAppointmentRequest
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string AppointmentTitle { get; set; }
        public int NumberOfAttendees { get; set; }
        public string Location { get; set; }
    }
}
