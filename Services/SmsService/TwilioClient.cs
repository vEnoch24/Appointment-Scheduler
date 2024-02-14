using Twilio.Clients;
using Twilio.Http;

namespace Appointment_Scheduler.Services.SmsService
{
    public class TwilioClient : ITwilioRestClient
    {
        private readonly TwilioRestClient _innerClient;

        public TwilioClient( IConfiguration config, System.Net.Http.HttpClient httpClient ) 
        {
            httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "CustomerTwilioRestClient-Demo");
            _innerClient = new TwilioRestClient(
                config["Twilio:AccountSid"],
                config["Twilio:AuthToken"],
                httpClient: new SystemNetHttpClient(httpClient));
        }

        public Response Request(Request request) => _innerClient.Request(request);
        public Task<Response> RequestAsync(Request request) => _innerClient.RequestAsync(request);
        public string AccountSid => _innerClient.AccountSid;
        public string Region => _innerClient.Region;
        public Twilio.Http.HttpClient HttpClient => _innerClient.HttpClient;
    }
}
