using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing.Windows.Compatibility;

namespace SignalRWebUI.Controllers
{
    public class QRCodeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string? value, IFormFile? qrImage)
        {
            // QR oluşturma 
            if (!string.IsNullOrWhiteSpace(value))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    QRCodeGenerator createQRCode = new QRCodeGenerator();
                    QRCodeGenerator.QRCode squareCode = createQRCode.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);

                    using (Bitmap image = squareCode.GetGraphic(10))
                    {
                        image.Save(memoryStream, ImageFormat.Png);
                        ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }

            // QR çözme
            if (qrImage != null && qrImage.Length > 0)
            {
                using var stream = qrImage.OpenReadStream();
                using var bitmap = (Bitmap)System.Drawing.Image.FromStream(stream);

                var reader = new BarcodeReader();
                var result = reader.Decode(bitmap);

                ViewBag.DecodedText = result?.Text ?? "QR kod çözülemedi.";
            }

            return View();
        }
    }
}