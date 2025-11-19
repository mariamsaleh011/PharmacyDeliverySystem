using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class RefundManager : IRefundManager
    {
        private readonly PharmacyDeliveryContext _ctx;

        public RefundManager(PharmacyDeliveryContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Refund> GetAll()
            => _ctx.Refunds
                   .Include(r => r.Pay)
                   .AsNoTracking()
                   .ToList();

        public Refund? GetById(int id)
            => _ctx.Refunds
                   .Include(r => r.Pay)
                   .FirstOrDefault(r => r.RefId == id);

        public IEnumerable<Refund> GetByPayment(string payId)
            => _ctx.Refunds
                   .Where(r => r.PayId == payId)
                   .Include(r => r.Pay)
                   .AsNoTracking()
                   .ToList();

        public void Add(Refund entity)
        {
            _ctx.Refunds.Add(entity);
            _ctx.SaveChanges();
        }

        public void Update(Refund entity)
        {
            _ctx.Refunds.Update(entity);
            _ctx.SaveChanges();
        }

        public void Delete(int id)
        {
            var r = _ctx.Refunds.Find(id);
            if (r == null) return;

            _ctx.Refunds.Remove(r);
            _ctx.SaveChanges();
        }
    }
}
