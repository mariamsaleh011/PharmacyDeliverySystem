using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class ReturnManager : IReturnManager
    {
        private readonly PharmacyDeliveryContext _ctx;

        public ReturnManager(PharmacyDeliveryContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Returnn> GetAll()
            => _ctx.Returns
                   .Include(r => r.Order)
                   .AsNoTracking()
                   .ToList();

        public Returnn? GetById(int id)
            => _ctx.Returns
                   .Include(r => r.Order)
                   .FirstOrDefault(r => r.ReturnId == id);

        public IEnumerable<Returnn> GetByOrder(int orderId)
            => _ctx.Returns
                   .Where(r => r.OrderID == orderId)
                   .AsNoTracking()
                   .ToList();

        public void Add(Returnn entity)
        {
            _ctx.Returns.Add(entity);
            _ctx.SaveChanges();
        }

        public void Update(Returnn entity)
        {
            _ctx.Returns.Update(entity);
            _ctx.SaveChanges();
        }

        public void SetStatus(int id, string status)
        {
            var allowed = new[] { "Requested", "Approved", "Rejected", "Completed" };
            if (!allowed.Contains(status))
                throw new ArgumentException("Invalid return status.", nameof(status));

            var ret = _ctx.Returns.Find(id);
            if (ret == null) return;

            ret.Status = status;
            _ctx.SaveChanges();
        }

        public void Delete(int id)
        {
            var ret = _ctx.Returns.Find(id);
            if (ret == null) return;

            _ctx.Returns.Remove(ret);
            _ctx.SaveChanges();
        }
    }
}
