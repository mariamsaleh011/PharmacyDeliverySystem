using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using QRCoder;
using System;
using System.IO;

namespace PharmacyDeliverySystem.Controllers
{
    public class QrConfirmationController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly PharmacyDeliveryContext _context;

        public QrConfirmationController(IOrderManager orderManager,
                                        PharmacyDeliveryContext context)
        {
            _orderManager = orderManager;
            _context = context;
        }

        // ===== Invoice Details View =====
        public IActionResult InvoiceDetails(int orderId, bool isPharmacy = false)
        {
            // 1) نجيب الأوردر
            var order = _orderManager.GetOrderById(orderId);
            if (order == null)
                return NotFound();

            // 2) إجمالي الفاتورة
            decimal totalAmount = order.TotalPrice ?? 0m;
            ViewBag.TotalAmount = totalAmount;

            // مين اللي فاتح الفاتورة؟ (فارمسي ولا كاستمر)
            ViewBag.IsPharmacy = isPharmacy;

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
                $"data:image/png;base64,{Convert.ToBase64String(stream.ToArray())}";

            // نفس الـ View: Views/QrConfirmation/InvoiceDetails.cshtml
            return View(order);
        }

        // ===== Confirm Delivery (QR scan) =====
        // متوقع qrData زي:
        // "OrderID:123; Customer:...; Total:..."
        [HttpGet]
        public IActionResult ConfirmDelivery(string qrData)
        {
            if (string.IsNullOrWhiteSpace(qrData))
                return Json(new { success = false, message = "Empty QR data." });

            int orderId;

            try
            {
                // نقرأ أول جزء "OrderID:123"
                var parts = qrData.Split(';');
                if (parts.Length == 0)
                    return Json(new { success = false, message = "Invalid QR format." });

                var firstPart = parts[0].Trim(); // "OrderID:123"
                var idParts = firstPart.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (idParts.Length < 2)
                    return Json(new { success = false, message = "Invalid QR format." });

                if (!int.TryParse(idParts[1], out orderId))
                    return Json(new { success = false, message = "Invalid order id in QR." });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to parse QR data." });
            }

            // نجيب الأوردر بالـ Items عشان نحدّثهم
            var order = _context.Orders
                                .Include(o => o.OrderItems)
                                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
                return Json(new { success = false, message = "Order not found." });

            // نحدّث حالة الأوردر والـ Items إلى Delivered لو لسه مش Delivered
            if (order.Status != "Delivered")
            {
                order.Status = "Delivered";

                foreach (var item in order.OrderItems)
                {
                    item.Status = "Delivered";
                }

                _context.SaveChanges();
            }

            return Json(new
            {
                success = true,
                orderId,
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
            // تأكيد إن البادي جاي صح
            if (req == null)
                return Json(new { success = false, message = "No data received." });

            if (req.Rating < 1 || req.Rating > 5)
                return Json(new { success = false, message = "Rating must be between 1 and 5." });

            var order = _orderManager.GetOrderById(req.OrderId);
            if (order == null)
                return Json(new { success = false, message = "Order not found." });

            order.Rating = req.Rating;
            _orderManager.UpdateOrder(order);

            return Json(new { success = true, message = "Rating saved successfully." });
        }
    }
}
