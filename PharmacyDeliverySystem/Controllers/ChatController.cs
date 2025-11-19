using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatManager _manager;

        public ChatController(IChatManager manager)
        {
            _manager = manager;
        }

        // =============================
        //          CHAT CRUD
        // =============================

        // Get all chats
        [HttpGet]
        public IActionResult GetAllChats()
        {
            var chats = _manager.GetAllChats();
            return Ok(chats);
        }

        // Get chat by ID
        [HttpGet("{id}")]
        public IActionResult GetChatById(int id)
        {
            var chat = _manager.GetChatById(id);
            if (chat == null) return NotFound("Chat not found");
            return Ok(chat);
        }

        // Create new chat
        [HttpPost]
        public IActionResult CreateChat([FromBody] Chat chat)
        {
            if (chat == null) return BadRequest("Chat data is required");

            _manager.AddChat(chat);
            return Ok(new { Message = "Chat created successfully", chat });
        }

        // Update Chat
        [HttpPut("{id}")]
        public IActionResult UpdateChat(int id, [FromBody] Chat updatedChat)
        {
            var existingChat = _manager.GetChatById(id);
            if (existingChat == null) return NotFound("Chat not found");

            updatedChat.ChatId = id;
            _manager.UpdateChat(updatedChat);

            return Ok(new { Message = "Chat updated successfully", updatedChat });
        }

        // Delete Chat
        [HttpDelete("{id}")]
        public IActionResult DeleteChat(int id)
        {
            var chat = _manager.GetChatById(id);
            if (chat == null) return NotFound("Chat not found");

            _manager.DeleteChat(id);
            return Ok("Chat deleted successfully");
        }


        // =============================
        //          MESSAGES
        // =============================

        // Get messages for chat
        [HttpGet("{chatId}/messages")]
        public IActionResult GetMessages(int chatId)
        {
            var messages = _manager.GetMessages(chatId);
            return Ok(messages);
        }

        // Add message to chat
        [HttpPost("{chatId}/messages")]
        public IActionResult AddMessage(int chatId, [FromBody] ChatMessage msg)
        {
            if (msg == null || string.IsNullOrEmpty(msg.MessageText))
                return BadRequest("Message text is required.");

            msg.ChatId = chatId;
            msg.SentAt = DateTime.Now;

            _manager.AddMessage(msg);

            return Ok(new { Message = "Message sent successfully", msg });
        }
    }
}
