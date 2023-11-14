namespace Appointment_Scheduler.Dto
{

    public class PushMessageDto
    {
        public PushMessageDto(string Title, string Body, string DeviceToken)
        {
            this.Title = Title;
            this.Body = Body;
            this.DeviceToken = DeviceToken;
        }

        public string Title { get; set; }
        public string Body { get; set; }
        public string DeviceToken { get; set; }
    }
}
