using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class PharmacyManager : IPharmacyManager
    {
        private readonly PharmacyDeliveryContext _context;

        public PharmacyManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        public IEnumerable<Pharmacy> GetAllPharmacies()
        {
            return _context.Pharmacies.ToList();
        }

        public Pharmacy? GetById(int id)
        {
            return _context.Pharmacies.FirstOrDefault(p => p.PharmId == id);
        }

        public IEnumerable<Pharmacy> GetByName(string name)
        {
            return _context.Pharmacies
                .Where(p => p.Name.Contains(name))
                .ToList();
        }

        public void Create(Pharmacy pharmacy)
        {
            _context.Pharmacies.Add(pharmacy);
            _context.SaveChanges();
        }

        public void Update(Pharmacy pharmacy)
        {
            _context.Pharmacies.Update(pharmacy);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var pharmacy = _context.Pharmacies.FirstOrDefault(p => p.PharmId == id);
            if (pharmacy == null) return;

            _context.Pharmacies.Remove(pharmacy);
            _context.SaveChanges();
        }
    }
}
