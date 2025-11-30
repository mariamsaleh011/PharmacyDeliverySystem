using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IQrConfirmationManager
    {
        QrConfirmation CreateQrForCustomer(int customerId, int runId);
        QrConfirmation CreateQrForOrder(int orderId);
        void ScanQr(int qrId, string scannedBy);
        QrConfirmation GetQrById(int qrId);
        QrConfirmation GetQrByOrder(int orderId);
        bool AllOrdersConfirmed(int runId);
        void CompleteRun(int runId);
        IEnumerable<QrConfirmation> GetQrByCustomer(int customerId);
    }
}
