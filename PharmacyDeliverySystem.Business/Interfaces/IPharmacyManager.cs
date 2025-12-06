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
        // IPharmacyManager.cs
        void SendMessage(int chatId, string message, string senderType);

        int GetOrdersCount(int pharmId);
        int GetProductsCount(int pharmId);


        // For Pharmacy Chat
        List<Chat> GetChatsByPharmacyId(int pharmacyId);
        Chat GetChatById(int chatId);
        Pharmacy GetPharmacyByEmail(string email);


    }
}
