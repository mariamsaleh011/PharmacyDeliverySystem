//using Microsoft.AspNetCore.Mvc;
//using PharmacyDeliverySystem.Business.Interfaces;
//using PharmacyDeliverySystem.Models;
//using QRCoder;
//using System.IO;

//namespace PharmacyDeliverySystem.Controllers
//{
//    public class QrConfirmationController : Controller
//    {
//        private readonly IOrderManager _orderManager;
//        private readonly IRatingManager _ratingManager;

//        public QrConfirmationController(IOrderManager orderManager, IRatingManager ratingManager)
//        {
//            _orderManager = orderManager;
//            _ratingManager = ratingManager;
//        }

//        // ===== Invoice Details View =====
//        public IActionResult InvoiceDetails(int orderId)
//        {
//            // 1) نجيب الأوردر من جدول Orders
//            var order = _orderManager.GetOrderById(orderId);
//            if (order == null)
//                return NotFound();

//            // 2) نحسب إجمالي الفاتورة من TotalPrice اللي خزّناها في الـ Checkout
//            decimal totalAmount = order.TotalPrice ?? 0m;
//            ViewBag.TotalAmount = totalAmount;

//            // 3) نكوّن نص الـ QR (من غير جدول QrConfirmations خالص)
//            string qrText =
//                $"OrderID:{order.OrderId}; Customer:{order.Customer?.Name}; Total:{totalAmount:C}";

//#pragma warning disable CA1416
//            using var qrGenerator = new QRCodeGenerator();
//            var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
//            using var qrCode = new QRCode(qrData);
//            using var bitmap = qrCode.GetGraphic(20);

//            using var stream = new MemoryStream();
//            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
//#pragma warning restore CA1416

//            string qrBase64 = $"data:image/png;base64,{System.Convert.ToBase64String(stream.ToArray())}";

//            ViewBag.QRCodeImage = qrBase64;

//            // نفس الـ View اللي عندك: Views/QrConfirmation/InvoiceDetails.cshtml
//            return View(order);
//        }

//        // ===== Confirm Delivery (QR scan) =====
//        [HttpGet]
//        public IActionResult ConfirmDelivery(string qrData)
//        {
//            if (string.IsNullOrEmpty(qrData))
//                return Json(new { success = false });

//            int orderId;
//            try
//            {
//                // qrData متوقَّع بالشكل:
//                // "OrderID:123; Customer:...; Total:..."
//                orderId = int.Parse(qrData.Split(';')[0].Split(':')[1]);
//            }
//            catch
//            {
//                return Json(new { success = false });
//            }

//            var order = _orderManager.GetOrderById(orderId);
//            if (order == null)
//                return Json(new { success = false });

//            order.Status = "Delivered";
//            _orderManager.Update(order);

//            return Json(new
//            {
//                success = true,
//                orderId = orderId,
//                message = "Order marked as Delivered"
//            });
//        }

//        // ===== Submit Rating =====
//        [HttpPost]
//        public IActionResult SubmitRating([FromBody] Rating rating)
//        {
//            if (rating == null || rating.Stars < 1 || rating.Stars > 5)
//                return Json(new { success = false });

//            _ratingManager.Add(rating);
//            return Json(new { success = true });
//        }
//    }
//}
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

        // Invoice details view
        public IActionResult InvoiceDetails(int orderId)
        {
            var order = _orderManager.GetOrderById(orderId);
            if (order == null) return NotFound();

            decimal totalAmount = order.TotalPrice ?? 0m;
            ViewBag.TotalAmount = totalAmount;

            // Generate simple QR that encodes OrderID:5 (you can extend text if needed)
            string qrText = $"OrderID:{order.OrderId}";

#pragma warning disable CA1416
            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using var bitmap = qrCode.GetGraphic(20);
            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
#pragma warning restore CA1416

            ViewBag.QRCodeImage = $"data:image/png;base64,{System.Convert.ToBase64String(stream.ToArray())}";

            return View(order);
        }

        // Confirm delivery by decoded QR content (called by client JS after decoding)
        // expecting qrData like: "OrderID:5" or "OrderID:5;..."
        [HttpGet]
        public IActionResult ConfirmDelivery(string qrData)
        {
            if (string.IsNullOrEmpty(qrData))
                return Json(new { success = false });

            int orderId;
            try
            {
                // try parse the first token "OrderID:5"
                var token = qrData.Split(';')[0];
                orderId = int.Parse(token.Split(':')[1]);
            }
            catch
            {
                return Json(new { success = false });
            }

            var order = _orderManager.GetOrderById(orderId);
            if (order == null)
                return Json(new { success = false });

            // update status and persist
            order.Status = "Delivered";
            _orderManager.UpdateOrder(order);

            return Json(new { success = true, orderId = orderId, message = "Order marked as Delivered" });
        }

        // Submit rating (stores rating inside Orders.Rating)
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
