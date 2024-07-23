using Appointment_Scheduler.Data;
using Appointment_Scheduler.Dto;
using Appointment_Scheduler.Model;
using Appointment_Scheduler.Repository;
using Appointment_Scheduler.RequestPayload;
using Appointment_Scheduler.Services.EmailService;
using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace Appointment_Scheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<AppointmentUser> _userRepository;
        private readonly AppointmentDbContext _context;
        private readonly IEmailService _mailService;
        private readonly ITwilioRestClient _client;
        public AuthController(IConfiguration configuration, AppointmentDbContext context, IEmailService mailService, ITwilioRestClient client, IGenericRepository<AppointmentUser> userResopitory)
        {
            _configuration = configuration;
            _userRepository = userResopitory;
            _context = context;
            _mailService = mailService;
            _client = client;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppointmentUser>> Register([FromBody] UserRegisterRequest request)
        {
            if (_context.Users.Any(e => e.Email == request.Email))
            {
                return BadRequest("Email already exists");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            if (_context.Users.Any(e => e.Username == request.Username))
            {
                return BadRequest("Username alread Exists");
            }

            var user = new AppointmentUser
            {
                Email = request.Email,
                Username = request.Username,
                PhoneNumber = request   .PhoneNumber,
                passwordhash = passwordHash,
                passwordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()
            };



            await _userRepository.Create(user);
            await _userRepository.Save();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (user.VerifiedAt == null)
            {
                return BadRequest("Not Verified");
            }

            if (!VerifyPasswordHash(request.Password, user.passwordhash, user.passwordSalt))
            {
                return BadRequest("Wrong Password!");
            }

            string token = CreateToken(user);
            return Ok(token);
        }

        [HttpPost("SendVerificationEmail")]
        public async Task<IActionResult> SendVerificationEmail(VerificationRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.userId);
            if (user == null)
            {
                return BadRequest("Invalid User");
            }
            if (user.VerifiedAt.HasValue)
            {
                return BadRequest("Email is already verified");
            }

            //var verificationLink = $"http://127.0.0.1:5501/EmailVerification.html?token={request.token}";
            //var verificationLink = $"http://127.0.0.1:5501/App/MainPage/index.html?token={request.token}";
            var verificationLink = $"http://127.0.0.1:5501/App/MainPage/EmailVerification.html?token={request.token}";

            //var confirmationLink = Url.Action("Verify", "Auth", new { token = request.token }, Request.Scheme);


            //var verificationCode = CreateRandomNumber();
            var emailBody = "<!DOCTYPE html>" +
                "<html lang=\"en\">" +
                "<head>" +
                "</head>" +
                "<body> " +
                " <div style=\"background-color: #f4f4f4; padding: 20px;\">" +
                "<p style=\"font-size: 18px;\"><b>Hello, <span style=\"color: #007bff;\"> " + user.Username + "</span>!</b></p>" +
                "<p style=\"font-size: 16px;\">Please click on the button below to complete verifying your email address and login:</p>" +
                "<a href=\"" + verificationLink + "\" style=\"display: inline-block; background-color: #007bff; color: #ffffff; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px;\">Verify Email</a>" +
                " </body>" +
                "</html>";

            var emailDto = new EmailDto(user.Email, "Email Verification", emailBody);
            await _mailService.SendEmailAsync(emailDto);

            return Ok("User Verification Sent");
        }

        [HttpGet("Verify")]
        public async Task<IActionResult> Verify([FromQuery] EmailVerificationRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == request.token);
            if (user == null)
            {
                return BadRequest("Invalid token");
            }
            if (user.VerifiedAt.HasValue)
            {
                return BadRequest("Email is already verified");
            }

            user.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok("Email Verified Successfully");
        }

        [HttpGet("GetUserById")]
        public async Task<ActionResult<AppointmentUserDto>> GetUserById(Guid id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            return Ok(user);
        }

        [HttpGet("GetUserByEmail")]
        public async Task<ActionResult<AppointmentUserDto>> GetUserByEmail([EmailAddress] string email)
        {
            var user = await _context.Users.Where(e => e.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("User not found");
            }

            return Ok(user);
        }

        //[HttpPost("SendMail")]
        //public async Task<bool> SendMail(MailData mailData)
        //{
        //    return await _mailService.SendMail(mailData);
        //}
        [HttpPost("SendMail")]
        public async Task<bool> SendMail(EmailDto mailData)
        {
            return await _mailService.SendEmailAsync(mailData);
        }

        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword([EmailAddress] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            user.PasswordRestToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);
            await _context.SaveChangesAsync();

            return Ok("You may now reset your password");
        }

        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordRestToken == request.Token);
            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return BadRequest("Invalid Token");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.passwordhash = passwordHash;
            user.passwordSalt = passwordSalt;
            user.PasswordRestToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return Ok("Password successfully reset.");
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([Required, EmailAddress]string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
            if(user == null)
            {
                return BadRequest("User not found.");
            }

            await _userRepository.Delete(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

       

        private string CreateRandomNumber()
        {
            var random = new Random();
            int randonNumber = random.Next(9000);
            string formattedString = randonNumber.ToString("D4");

            return formattedString;
        }

        private string CreateToken(AppointmentUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) 
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }



        /*************** Twilio *************************/
        //[HttpPost("Send-Sms")]
        //public IActionResult SendSms([FromForm] SmsMessageRequest model)
        //{
        //    string accountSid = "ACd86bea79ff931a4f748e5b5518901412";
        //    string authToken = "05dcd5d4c01d8f831b53d63d8972f91b";

        //    TwilioClient.Init(accountSid, authToken);

        //    string from = "+16592215187";

        //    var message = MessageResource.Create(
        //        to: new PhoneNumber(model.To),
        //        from: new PhoneNumber(from),
        //        body: model.Message);

        //    return Ok("success" + message.Sid);
        //}

        [HttpPost("Send-WhatsappMessage")]
        public IActionResult SendWAMessage([FromForm] SmsMessageRequest model)
        {
            string accountSid = "ACd86bea79ff931a4f748e5b5518901412";
            string authToken = "05dcd5d4c01d8f831b53d63d8972f91b";

            TwilioClient.Init(accountSid, authToken);

            string from = "+14155238886";

            var message = MessageResource.Create(
                to: new PhoneNumber("whatsapp:" + model.To),
                from: new PhoneNumber("whatsapp:" + from),
                body: model.Message);

            return Ok("success" + message.Sid);
        }
        /***************************************/

    }
}
