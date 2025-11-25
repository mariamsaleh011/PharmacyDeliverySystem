using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IDeliveryRunManager
    {
        void CreateRun(DeliveryRun run);
        void CompleteRun(int runId);
        IEnumerable<DeliveryRun> GetActiveRuns();
        void AddOrderToRun(int runId, int orderId);
        bool AllOrdersConfirmed(int runId);
    }
}
