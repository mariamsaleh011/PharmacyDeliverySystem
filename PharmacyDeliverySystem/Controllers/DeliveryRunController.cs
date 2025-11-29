using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels.DeliveryRun;
using System.Linq;

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
        public IActionResult CreateRun([FromBody] DeliveryRunViewModels.CreateDeliveryRunViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var run = new DeliveryRun
                {
                    RiderId = model.RiderId,
                    StartAt = System.DateTime.Now,
                    Orders = model.OrderIds.Select(id => new Order { OrderId = id }).ToList()
                };

                _runManager.CreateRun(run);
                return Ok(new { Message = "Delivery Run Created Successfully", run });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =============================
        // 2) Complete a Delivery Run
        // =============================
        [HttpPost("complete")]
        public IActionResult CompleteRun([FromBody] DeliveryRunViewModels.CompleteRunViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _runManager.CompleteRun(model.RunId);
                return Ok(new { Message = "Run completed successfully" });
            }
            catch (System.Exception ex)
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
        public IActionResult AddOrderToRun([FromBody] DeliveryRunViewModels.AddOrderToRunViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _runManager.AddOrderToRun(model.RunId, model.OrderId);
                return Ok(new { Message = "Order added to run successfully" });
            }
            catch (System.Exception ex)
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
            if (runId <= 0)
                return BadRequest("Invalid RunId");

            try
            {
                bool confirmed = _runManager.AllOrdersConfirmed(runId);
                return Ok(new { RunId = runId, AllConfirmed = confirmed });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
