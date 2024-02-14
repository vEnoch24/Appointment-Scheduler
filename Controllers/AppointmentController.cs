using Appointment_Scheduler.Data;
using Appointment_Scheduler.Dto;
using Appointment_Scheduler.Model;
using Appointment_Scheduler.Repository;
using Appointment_Scheduler.RequestPayload;
using Appointment_Scheduler.Services.EmailService;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Vonage.Request;
using Vonage;
using System.Reflection;

namespace Appointment_Scheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IGenericRepository<Appointment> _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<AppointmentUser> _userRepository;
        private readonly IEmailService _mailService;
        private readonly AppointmentDbContext _context;

        public AppointmentController(IGenericRepository<Appointment> appointmentRepository, IGenericRepository<AppointmentUser> userRepository,
            AppointmentDbContext context, IEmailService mailService, IMapper mapper)
        {
            this._appointmentRepository = appointmentRepository;
            this._mapper = mapper;
            this._userRepository = userRepository;
            this._context = context;
            this._mailService = mailService;
        }

        [HttpPost("CreateAppointment")]
        public async Task<ActionResult<Appointment>> CreateAppointment( AppointmentRequest appointmentItems)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == appointmentItems.AppointmentUserId);
            //if(user == null)
            //{
            //    return BadRequest("Cannot create Appointment");
            //}


            var result = _mapper.Map<Appointment>(appointmentItems);
            await _appointmentRepository.Create(result);

            var remainerRequest = new AppointmentRemainderRequest
            {
                email = user.Email,
                AppointmentId = result.AppointmentId,
            };
            
            await ScheduleRemainder(remainerRequest);

            await _appointmentRepository.Save();

            return Ok(result);            
        }

        [HttpPut("EditAppointment")]
        public async Task<ActionResult<AppointmentDto>> EditAppointment(Guid id, EditAppointmentRequest request)
        {
            var appointment = await _appointmentRepository.GetAllAsQueryable().Where(x => x.AppointmentId == id ).FirstOrDefaultAsync();

            if (appointment != null)
            {
                appointment.AppointmentTitle = request.AppointmentTitle;
                appointment.StartTime = request.StartTime;
                appointment.EndTime = request.EndTime;
                appointment.NumberOfAttedees = request.NumberOfAttendees;
                appointment.Location = request.Location;
            }
            else
            {
                return BadRequest();
            }
            await _appointmentRepository.Update(appointment);
            await _appointmentRepository.Save();

            return Ok(appointment);
        }

        [HttpGet("GetAllAppointments")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAllAppointments(Guid id)
        {
            
            var result = await _appointmentRepository.GetAllAsQueryable().Where(x => x.AppointmentUserId == id).ToListAsync();

            result.ForEach( (appointment) =>
            {
                if(appointment.StartTime <= DateTime.Now && DateTime.Now < appointment.EndTime)
                {
                    appointment.AppointmentStatus = AppointmentStatusEnum.Started.ToString();
                }
                else if(appointment.EndTime <= DateTime.Now && DateTime.Now > appointment.StartTime)
                {
                    appointment.AppointmentStatus = AppointmentStatusEnum.Ended.ToString();
                }
            });

            return Ok(result);
        }

        [HttpGet("GetAnAppointment")]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
        {
            var result = await _appointmentRepository.GetById(id);
            return Ok(result);
        }

        [HttpDelete("DeleteAppointment")]
        public async Task<ActionResult<Appointment>> DeleteAppointment(Guid id)
        {
            var appointment = await _appointmentRepository.GetById(id);
            await _appointmentRepository.Delete(appointment);
            await _appointmentRepository.Save();

            return Ok("Appointment Deleted successfully");
        }

        [HttpPost("Remainder")]
        public async Task<IActionResult> ScheduleRemainder(AppointmentRemainderRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.email);
            var appointment = await _appointmentRepository.GetById(request.AppointmentId);

            var remainderTime = appointment.StartTime.AddHours(-1);

            var currentTime = DateTime.UtcNow;

            var emailBody = "<!DOCTYPE html>" +
                "<html lang=\"en\">" +
                "<head>" +
                "</head>" +
                "<body> " +
                " <div style=\"background-color: #f4f4f4; padding: 20px;\">" +
                "<p style=\"font-size: 18px;\"><b>Hello, <span style=\"color: #007bff;\"> " + user.Username + "</span>!</b></p>" +
                "<p style=\"font-size: 16px;\">You Have an appointment by <span style=\"color: #007bff;\">" + appointment.StartTime.ToUniversalTime() + "</span>, With title <span style=\"color: #007bff;\">" + appointment.AppointmentTitle + "</span>!</p>" +
                " </body>" +
                "</html>";

            var message = "Hello " + user.Username + ", Your appointment "+ appointment.AppointmentTitle + " starts by "+ appointment.StartTime.ToUniversalTime();

            var emailDto = new EmailDto(user.Email, "Appointment Remainder", emailBody);
            var smsRequest = new SmsMessageRequest(user.PhoneNumber.ToString(), message);
            if (remainderTime < appointment.StartTime)
            {
                BackgroundJob.Schedule(() => _mailService.SendEmailAsync(emailDto), remainderTime);
                BackgroundJob.Schedule(() => SendSms(smsRequest), remainderTime);

                return Ok("Remainder schduled successfully");
            }
            else
            {
                return BadRequest("Remainder time has passed. No Remainder Scheduled");
            }


        }

        [HttpPost("Send-Sms")]
        public IActionResult SendSms([FromForm] SmsMessageRequest model)
        {
            var credentials = Credentials.FromApiKeyAndSecret(
                "939ddbb6",
                "zXfP4KGzaP195jzD"
                );

            var VonageClient = new VonageClient(credentials);

            var response = VonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
            {
                To = model.To,
                From = "Zaptime",
                Text = model.Message
            });


            return Ok("success " + response);
        }

    }
}

