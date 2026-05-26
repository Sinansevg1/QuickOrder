using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SignalRWebUI.Dtos.MailDtos;
using MailKit.Net.Smtp;

namespace SignalRWebUI.Controllers
{
    public class MailController : Controller
    {
        private readonly IConfiguration _configuration;

        public MailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(CreateMailDto createMailDto)
        {
            var senderMail = _configuration["MailSettings:SenderMail"]
                ?? throw new InvalidOperationException("MailSettings:SenderMail yapılandırılmamış.");
            var senderName = _configuration["MailSettings:SenderName"] ?? "S&S Restorant Rezervasyon";
            var password   = _configuration["MailSettings:Password"]
                ?? throw new InvalidOperationException("MailSettings:Password yapılandırılmamış.");
            var smtpHost   = _configuration["MailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort   = _configuration.GetValue<int>("MailSettings:SmtpPort", 587);

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(senderName, senderMail));
            mimeMessage.To.Add(new MailboxAddress("User", createMailDto.ReceiverMail));

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = createMailDto.Body;
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            mimeMessage.Subject = createMailDto.Subject;

            SmtpClient client = new SmtpClient();
            client.Connect(smtpHost, smtpPort, false);
            client.Authenticate(senderMail, password);
            client.Send(mimeMessage);
            client.Disconnect(true);

            return RedirectToAction("Index", "Statistic");
        }
    }
}
