using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.ViewModels;
using PharmacyDeliverySystem.ViewModels.QR;

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
        [HttpPost("create")]
        public IActionResult CreateQr([FromBody] QrConfirmationViewModels.CreateQrRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var qr = _manager.CreateQrForCustomer(model.CustomerId, model.RunId);
                return Ok(new { Message = "QR created successfully", qr });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Scan QR
        [HttpPost("scan/{qrId}")]
        public IActionResult ScanQr(int qrId, [FromBody] QrConfirmationViewModels.ScanQrRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _manager.ScanQr(qrId, model.ScannedBy);
                return Ok(new { Message = "QR scanned successfully." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Check if all QR’s in a Run scanned
        [HttpGet("run/{runId}/all-scanned")]
        public IActionResult AllScanned(int runId)
        {
            if (runId <= 0)
                return BadRequest("Invalid RunId");

            try
            {
                var result = _manager.AllQrScanned(runId);
                return Ok(new { RunId = runId, AllScanned = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // جلب كل QR لعميل معين
        [HttpGet("customer/{customerId}")]
        public IActionResult GetByCustomer(int customerId)
        {
            if (customerId <= 0)
                return BadRequest("Invalid CustomerId");

            try
            {
                var list = _manager.GetQrByCustomer(customerId);
                return Ok(list);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
