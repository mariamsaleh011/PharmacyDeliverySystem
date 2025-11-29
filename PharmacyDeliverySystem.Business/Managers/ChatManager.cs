using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.Business.Interfaces;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class ChatManager : IChatManager
    {
        private readonly PharmacyDeliveryContext _context;

        public ChatManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        // Chats
        public IEnumerable<Chat> GetAllChats()
        {
            return _context.Chats.ToList();
        }

        public Chat? GetChatById(int id)
        {
            return _context.Chats.FirstOrDefault(x => x.Id == id);
        }
    }
}
