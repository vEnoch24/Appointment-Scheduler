using Appointment_Scheduler.Data;
using Appointment_Scheduler.Dto;

namespace Appointment_Scheduler.Services.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailDto request);

        Task<bool> SendMail(MailData mailData);
    }
}