namespace Appointment_Scheduler.RequestPayload
{
    public class SmsMessageRequest
    {
        public SmsMessageRequest(string To, string Message)
        {
            this.To = To;
            this.Message = Message;
        }

        public string To { get; set; }
        public string Message { get; set; } 
    }
}
