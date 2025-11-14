using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class PrescriptionManager : IPrescriptionManager
    {
        private readonly PharmacyDeliveryContext _ctx;

        public PrescriptionManager(PharmacyDeliveryContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Prescription> GetAll()
            => _ctx.Prescriptions
                   .Include(p => p.Customer)
                   .Include(p => p.Pharm)
                   .Include(p => p.Order)
                   .AsNoTracking()
                   .ToList();

        public Prescription? GetById(int id)
            => _ctx.Prescriptions
                   .Include(p => p.Customer)
                   .Include(p => p.Pharm)
                   .Include(p => p.Order)
                   .FirstOrDefault(p => p.PreId == id);

        public IEnumerable<Prescription> GetByCustomer(int customerId)
            => _ctx.Prescriptions
                   .Where(p => p.CustomerId == customerId)
                   .Include(p => p.Order)
                   .AsNoTracking()
                   .ToList();

        public IEnumerable<Prescription> GetByPharmacy(int pharmacyId)
            => _ctx.Prescriptions
                   .Where(p => p.PharmId == pharmacyId)
                   .Include(p => p.Order)
                   .AsNoTracking()
                   .ToList();

        public IEnumerable<Prescription> GetByOrder(int orderId)
            => _ctx.Prescriptions
                   .Where(p => p.OrderId == orderId)
                   .AsNoTracking()
                   .ToList();

        public void Add(Prescription entity)
        {
            _ctx.Prescriptions.Add(entity);
            _ctx.SaveChanges();
        }

        public void Update(Prescription entity)
        {
            _ctx.Prescriptions.Update(entity);
            _ctx.SaveChanges();
        }

        public void SetStatus(int id, string status)
        {
            var allowed = new[] { "Uploaded", "UnderReview", "Approved", "Rejected", "Fulfilled" };
            if (!allowed.Contains(status))
                throw new ArgumentException("Invalid prescription status.");

            var p = _ctx.Prescriptions.Find(id);
            if (p == null) return;

            p.Status = status;
            _ctx.SaveChanges();
        }

        public void Delete(int id)
        {
            var p = _ctx.Prescriptions.Find(id);
            if (p == null) return;

            _ctx.Prescriptions.Remove(p);
            _ctx.SaveChanges();
        }
    }
}
