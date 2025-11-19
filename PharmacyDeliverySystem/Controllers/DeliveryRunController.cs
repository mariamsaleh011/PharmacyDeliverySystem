using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryRunController : ControllerBase
    {
        private readonly IDeliveryRunManager _runManager;

        public DeliveryRunController(IDeliveryRunManager runManager)
        {
            _runManager = runManager;
        }

        // =============================
        // 1) Create new Delivery Run
        // =============================
        [HttpPost("create")]
        public IActionResult CreateRun([FromBody] DeliveryRun run)
        {
            if (run == null || run.RiderId <= 0)
                return BadRequest("Invalid DeliveryRun data.");

            try
            {
                _runManager.CreateRun(run);
                return Ok(new { Message = "Delivery Run Created Successfully", run });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =============================
        // 2) Complete a Delivery Run
        // =============================
        [HttpPost("complete/{runId}")]
        public IActionResult CompleteRun(int runId)
        {
            try
            {
                _runManager.CompleteRun(runId);
                return Ok(new { Message = "Run completed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =============================
        // 3) Get Active Runs
        // =============================
        [HttpGet("active")]
        public IActionResult GetActiveRuns()
        {
            var runs = _runManager.GetActiveRuns();
            return Ok(runs);
        }

        // =============================
        // 4) Add Order to Run
        // =============================
        [HttpPost("add-order")]
        public IActionResult AddOrderToRun(int runId, int orderId)
        {
            try
            {
                _runManager.AddOrderToRun(runId, orderId);
                return Ok(new { Message = "Order added to run successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =============================
        // 5) Check if all Orders in Run confirmed with QR
        // =============================
        [HttpGet("confirmed/{runId}")]
        public IActionResult AllOrdersConfirmed(int runId)
        {
            try
            {
                bool confirmed = _runManager.AllOrdersConfirmed(runId);
                return Ok(new { RunId = runId, AllConfirmed = confirmed });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
