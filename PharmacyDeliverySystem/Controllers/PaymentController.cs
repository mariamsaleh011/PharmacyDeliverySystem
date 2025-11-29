using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentManager _manager;

        public PaymentController(IPaymentManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Payment>> GetAll()
        {
            return Ok(_manager.GetAllPayments());
        }

        [HttpGet("{payId}")]
        public ActionResult<Payment> Get(string payId)
        {
            var payment = _manager.GetPaymentById(payId);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [HttpGet("ByStatus/{status}")]
        public ActionResult<IEnumerable<Payment>> GetByStatus(string status)
        {
            return Ok(_manager.GetPaymentsByStatus(status));
        }

        [HttpPost]
        public ActionResult Create([FromBody] Payment payment)
        {
            _manager.CreatePayment(payment);
            return CreatedAtAction(nameof(Get), new { payId = payment.PayId }, payment);
        }

        [HttpPut]
        public ActionResult Update([FromBody] Payment payment)
        {
            _manager.UpdatePayment(payment);
            return NoContent();
        }

        [HttpDelete("{payId}")]
        public ActionResult Delete(string payId)
        {
            _manager.DeletePayment(payId);
            return NoContent();
        }
    }
}
