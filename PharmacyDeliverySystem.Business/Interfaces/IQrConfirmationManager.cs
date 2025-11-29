using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IQrConfirmationManager
    {
        QrConfirmation CreateQrForCustomer(int customerId, int runId);
        void ScanQr(int qrId, string scannedBy);
        bool AllQrScanned(int runId);
        IEnumerable<QrConfirmation> GetQrByCustomer(int customerId);
    }
}
