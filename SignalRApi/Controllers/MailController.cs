using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace SignalRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult SendMail([FromBody] SendMailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ReceiverMail) ||
                string.IsNullOrWhiteSpace(request.Subject) ||
                string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest("ReceiverMail, Subject ve Body zorunludur.");
            }

            var senderMail = _configuration["MailSettings:SenderMail"]
                ?? throw new InvalidOperationException("MailSettings:SenderMail yapılandırılmamış.");
            var senderName = _configuration["MailSettings:SenderName"] ?? "S&S Restorant";
            var password   = _configuration["MailSettings:Password"]
                ?? throw new InvalidOperationException("MailSettings:Password yapılandırılmamış.");
            var smtpHost   = _configuration["MailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort   = _configuration.GetValue<int>("MailSettings:SmtpPort", 587);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderMail));
            message.To.Add(new MailboxAddress("", request.ReceiverMail));
            message.Subject = request.Subject;
            message.Body = new BodyBuilder { HtmlBody = request.Body }.ToMessageBody();

            using var client = new SmtpClient();
            client.Connect(smtpHost, smtpPort, false);
            client.Authenticate(senderMail, password);
            client.Send(message);
            client.Disconnect(true);

            return Ok("Mail başarıyla gönderildi.");
        }
    }

    public class SendMailRequest
    {
        public string ReceiverMail { get; set; } = string.Empty;
        public string Subject      { get; set; } = string.Empty;
        public string Body         { get; set; } = string.Empty;
    }
}
