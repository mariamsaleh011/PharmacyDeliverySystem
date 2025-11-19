using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace PharmacyDeliverySystem.Business
{
    public class ChatManager : IChatManager
    {
        private readonly PharmacyDeliveryContext context;

        public ChatManager(PharmacyDeliveryContext context)
        {
            this.context = context;
        }

        // Chats
        public IEnumerable<Chat> GetAllChats() => context.Chats.ToList();
        public Chat? GetChatById(int id) => context.Chats.Find(id);
        public void AddChat(Chat chat) { context.Chats.Add(chat); context.SaveChanges(); }
        public void UpdateChat(Chat chat) { context.Chats.Update(chat); context.SaveChanges(); }
        public void DeleteChat(int id)
        {
            var chat = context.Chats.Find(id);
            if (chat != null) { context.Chats.Remove(chat); context.SaveChanges(); }
        }

        // Messages
        public IEnumerable<ChatMessage> GetMessages(int chatId)
        {
            return context.ChatMessages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToList();
        }

        public void AddMessage(ChatMessage message)
        {
            context.ChatMessages.Add(message);
            context.SaveChanges();
        }
    }
}
