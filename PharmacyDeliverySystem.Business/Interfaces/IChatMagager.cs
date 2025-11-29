using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IChatManager
    {
        IEnumerable<Chat> GetAllChats();
        Chat? GetChatById(int id);
        void AddChat(Chat chat);
        void UpdateChat(Chat chat);
        void DeleteChat(int id);

        // Optional: Messages
        IEnumerable<ChatMessage> GetMessages(int chatId);
        void AddMessage(ChatMessage message);
    }
}

