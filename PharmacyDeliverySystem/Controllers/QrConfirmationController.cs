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

        // ===== Invoice Details View =====
        public IActionResult InvoiceDetails(int orderId)
        {
            // 1) نجيب الأوردر
            var order = _orderManager.GetOrderById(orderId);
            if (order == null) return NotFound();

            // 2) إجمالي الفاتورة
            decimal totalAmount = order.TotalPrice ?? 0m;
            ViewBag.TotalAmount = totalAmount;

            // 3) نكوّن نص الـ QR
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

            ViewBag.QRCodeImage =
                $"data:image/png;base64,{System.Convert.ToBase64String(stream.ToArray())}";

            // نفس الـ View: Views/QrConfirmation/InvoiceDetails.cshtml
            return View(order);
        }

        // ===== Confirm Delivery (QR scan) =====
        // متوقع qrData زي:
        // "OrderID:123; Customer:...; Total:..."
        [HttpGet]
        public IActionResult ConfirmDelivery(string qrData)
        {
            if (string.IsNullOrEmpty(qrData))
                return Json(new { success = false });

            int orderId;
            try
            {
                // نقرأ أول جزء "OrderID:123"
                var firstPart = qrData.Split(';')[0];  // "OrderID:123"
                var idPart = firstPart.Split(':')[1];  // "123"
                orderId = int.Parse(idPart);
            }
            catch
            {
                return Json(new { success = false });
            }

            var order = _orderManager.GetOrderById(orderId);
            if (order == null)
                return Json(new { success = false });

            // نحدّث حالة الأوردر إلى Delivered
            order.Status = "Delivered";
            _orderManager.UpdateOrder(order);

            return Json(new
            {
                success = true,
                orderId = orderId,
                message = "Order marked as Delivered"
            });
        }

        // ===== Submit Rating (stores rating inside Orders.Rating) =====
        public class RatingRequest
        {
            public int OrderId { get; set; }
            public int Rating { get; set; }
        }

        [HttpPost]
        public IActionResult SubmitRating([FromBody] RatingRequest req)
        {
            if (req == null || req.Rating < 1 || req.Rating > 5)
                return Json(new { success = false });

            var order = _orderManager.GetOrderById(req.OrderId);
            if (order == null)
                return Json(new { success = false });

            order.Rating = req.Rating;
            _orderManager.UpdateOrder(order);

            return Json(new { success = true });
        }
    }
}
