using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using QRCoder;
using System.IO;

namespace PharmacyDeliverySystem.Controllers
{
    public class QrConfirmationController : Controller
    {
        private readonly IQrConfirmationManager _qrManager;

        public QrConfirmationController(IQrConfirmationManager qrManager)
        {
            _qrManager = qrManager;
        }

        public IActionResult InvoiceDetails(int orderId)
        {
            var qrConfirmation = _qrManager.GetQrByOrder(orderId);

            if (qrConfirmation == null || qrConfirmation.Order == null)
                return NotFound();

            var order = qrConfirmation.Order;

            // Total amount placeholder (no total in models)
            decimal totalAmount = 0;

            string qrText = $"OrderID:{order.OrderId}; Customer:{order.Customer.Name}; Total:{totalAmount:C}; QR:{qrConfirmation.Code}";

            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using var bitmap = qrCode.GetGraphic(20);

            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            string qrBase64 = $"data:image/png;base64,{System.Convert.ToBase64String(stream.ToArray())}";

            ViewBag.QRCodeImage = qrBase64;
            ViewBag.TotalAmount = totalAmount;

            return View(order);
        }
    }
}
 