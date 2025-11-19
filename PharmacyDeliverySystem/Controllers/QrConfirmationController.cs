using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QrConfirmationController : ControllerBase
    {
        private readonly IQrConfirmationManager _manager;

        public QrConfirmationController(IQrConfirmationManager manager)
        {
            _manager = manager;
        }

        // إنشاء QR للعميل في Run معين
        public class CreateQrRequest
        {
            public int CustomerId { get; set; }
            public int RunId { get; set; }
        }

        [HttpPost("create")]
        public IActionResult CreateQr([FromBody] CreateQrRequest req)
        {
            try
            {
                var qr = _manager.CreateQrForCustomer(req.CustomerId, req.RunId);
                return Ok(new { Message = "QR created successfully", qr });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Scan QR
        public class ScanQrRequest
        {
            public string? ScannedBy { get; set; }
        }

        [HttpPost("scan/{qrId}")]
        public IActionResult ScanQr(int qrId, [FromBody] ScanQrRequest req)
        {
            try
            {
                if (string.IsNullOrEmpty(req.ScannedBy))
                    return BadRequest("ScannedBy is required");

                _manager.ScanQr(qrId, req.ScannedBy);
                return Ok(new { Message = "QR scanned successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Check if all QR’s in a Run scanned
        [HttpGet("run/{runId}/all-scanned")]
        public IActionResult AllScanned(int runId)
        {
            try
            {
                var result = _manager.AllQrScanned(runId);
                return Ok(new { RunId = runId, AllScanned = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // جلب كل QR لعميل معين
        [HttpGet("customer/{customerId}")]
        public IActionResult GetByCustomer(int customerId)
        {
            try
            {
                var list = _manager.GetQrByCustomer(customerId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
