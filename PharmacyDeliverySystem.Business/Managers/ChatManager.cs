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
            this._context = context;
        }

        // Chats
        public IEnumerable<Chat> GetAllChats() => _context.Chats.ToList();
        public Chat? GetChatById(int id) => _context.Chats.Find(id);
        public void AddChat(Chat chat) { _context.Chats.Add(chat); _context.SaveChanges(); }
        public void UpdateChat(Chat chat) { _context.Chats.Update(chat); _context.SaveChanges(); }
        public void DeleteChat(int id)
        {
            var chat = _context.Chats.Find(id);
            if (chat != null) { _context.Chats.Remove(chat); _context.SaveChanges(); }
        }

        // Messages
        public IEnumerable<ChatMessage> GetMessages(int chatId)
        {
            return _context.ChatMessages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToList();
        }

        public void AddMessage(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            _context.SaveChanges();
        }
    }
}
