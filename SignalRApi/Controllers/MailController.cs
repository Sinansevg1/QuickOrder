using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace SignalRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendMail([FromBody] SendMailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ReceiverMail) ||
                string.IsNullOrWhiteSpace(request.Subject) ||
                string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest("ReceiverMail, Subject ve Body zorunludur.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("S&S Restorant", "ss.deneme47@gmail.com"));
            message.To.Add(new MailboxAddress("", request.ReceiverMail));
            message.Subject = request.Subject;
            message.Body = new BodyBuilder { HtmlBody = request.Body }.ToMessageBody();

            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("ss.deneme47@gmail.com", "mkyz kuax eikl tdbg");
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
