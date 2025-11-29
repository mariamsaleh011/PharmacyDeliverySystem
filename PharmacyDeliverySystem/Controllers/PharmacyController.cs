using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business;
using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyManager _manager;

        public PharmacyController(IPharmacyManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Pharmacy>> GetAll()
        {
            return Ok(_manager.GetAllPharmacies());
        }

        [HttpGet("{id}")]
        public ActionResult<Pharmacy> Get(int id)
        {
            var pharmacy = _manager.GetById(id);
            if (pharmacy == null) return NotFound();
            return Ok(pharmacy);
        }

        [HttpGet("ByName/{name}")]
        public ActionResult<IEnumerable<Pharmacy>> GetByName(string name)
        {
            return Ok(_manager.GetByName(name));
        }

        [HttpPost]
        public ActionResult Create([FromBody] Pharmacy pharmacy)
        {
            _manager.Create(pharmacy);
            return Ok(pharmacy);
        }

        [HttpPut]
        public ActionResult Update([FromBody] Pharmacy pharmacy)
        {
            _manager.Update(pharmacy);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _manager.Delete(id);
            return NoContent();
        }
    }
}

