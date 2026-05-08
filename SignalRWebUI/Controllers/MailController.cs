using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SignalRWebUI.Dtos.MailDtos;
using MailKit.Net.Smtp;

namespace SignalRWebUI.Controllers
{
    public class MailController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(CreateMailDto createMailDto)
        {
            MimeMessage mimeMessage = new MimeMessage();
            //mailin kimden gideceği 
            MailboxAddress mailboxAddressFrom = new MailboxAddress("S&S Restorant Rezervasyon", "ss.deneme47@gmail.com");
            mimeMessage.From.Add(mailboxAddressFrom);
            // mailin kime gideceği
            MailboxAddress mailboxAddressTo = new MailboxAddress("User", createMailDto.ReceiverMail);
            mimeMessage.To.Add(mailboxAddressTo);
            // mailin konusu
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = createMailDto.Body;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            mimeMessage.Subject = createMailDto.Subject;
            // mailin gönderilmesi
            SmtpClient client = new SmtpClient();
           client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("ss.deneme47@gmail.com", "mkyz kuax eikl tdbg");
          
            client.Send(mimeMessage);
            client.Disconnect(true);

            return RedirectToAction("Index","Statistic");
        }
    }
}
