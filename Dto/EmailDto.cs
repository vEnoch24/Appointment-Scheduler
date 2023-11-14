namespace Appointment_Scheduler.Dto
{
    public class EmailDto
    {
        public EmailDto(string to, string subject, string body)
        {
            this.To = to;
            this.Subject = subject; 
            this.Body = body;
        }
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
