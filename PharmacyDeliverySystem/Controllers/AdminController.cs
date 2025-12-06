using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System.Linq;

namespace PharmacyDeliverySystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly PharmacyDeliveryContext _context;

        public AdminController(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(PharmacyAdmin admin)
        {
            var pharmId = HttpContext.Session.GetInt32("PharmId");
            if (pharmId == null)
                return Unauthorized();

            admin.PharmId = pharmId.Value;
            admin.CreatedAt = DateTime.Now;

            _context.PharmacyAdmins.Add(admin);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "Pharmacy");
        }

        public IActionResult List()
        {
            var pharmId = HttpContext.Session.GetInt32("PharmId");
            if (pharmId == null)
                return Unauthorized();

            var admins = _context.PharmacyAdmins
                .Where(a => a.PharmId == pharmId.Value)
                .ToList();

            return View(admins);
        }
    }
}
