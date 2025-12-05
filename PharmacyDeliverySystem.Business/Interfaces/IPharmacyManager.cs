using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IPharmacyManager
    {
        IEnumerable<Pharmacy> GetAllPharmacies();
        Pharmacy? GetById(int id);
        IEnumerable<Pharmacy> GetByName(string name);
        void Create(Pharmacy pharmacy);
        void Update(Pharmacy pharmacy);
        void Delete(int id);

        // For Pharmacy Chat
        List<Chat> GetChatsByPharmacyId(int pharmacyId);
        Chat GetChatById(int chatId);
        void SendMessage(int chatId, string message, string senderType);
        Pharmacy GetPharmacyByEmail(string email);


    }
}
