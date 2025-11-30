using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using QRCoder;
using System.IO;

namespace PharmacyDeliverySystem.Controllers
{
    public class QrConfirmationController : Controller
    {
        private readonly IOrderManager _orderManager;

        public QrConfirmationController(IOrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        public IActionResult InvoiceDetails(int orderId)
        {
            // 1) نجيب الأوردر من جدول Orders
            var order = _orderManager.GetOrderById(orderId);
            if (order == null)
                return NotFound();

            // 2) نحسب إجمالي الفاتورة من TotalPrice اللي خزّناها في الـ Checkout
            decimal totalAmount = order.TotalPrice ?? 0m;

            // 3) نكوّن نص الـ QR (من غير جدول QrConfirmations خالص)
            string qrText =
                $"OrderID:{order.OrderId}; Customer:{order.Customer?.Name}; Total:{totalAmount:C}";

#pragma warning disable CA1416
            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using var bitmap = qrCode.GetGraphic(20);

            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
#pragma warning restore CA1416

            string qrBase64 = $"data:image/png;base64,{System.Convert.ToBase64String(stream.ToArray())}";

            ViewBag.QRCodeImage = qrBase64;
            ViewBag.TotalAmount = totalAmount;

            // نفس الـ View اللي عندك: Views/QRconfirmation/InvoiceDetails.cshtml
            return View(order);
        }
    }
}
