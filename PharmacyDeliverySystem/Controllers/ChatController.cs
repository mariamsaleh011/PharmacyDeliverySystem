using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.ViewModels.Chat;


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
        // CHAT CRUD
        // =============================

        [HttpGet]
        public IActionResult GetAllChats() => Ok(_manager.GetAllChats());

        [HttpGet("{id}")]
        public IActionResult GetChatById(int id)
        {
            var chat = _manager.GetChatById(id);
            if (chat == null) return NotFound("Chat not found");
            return Ok(chat);
        }

        [HttpPost]
        public IActionResult CreateChat([FromBody] ChatViewModels.ChatInputViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var chat = new Chat
            {
                Status = model.Status,
                Channel = model.Channel,
                OrderId = model.OrderId,
                CustomerId = model.CustomerId,
                PharmacyId = model.PharmacyId
            };

            _manager.AddChat(chat);
            return Ok(new { Message = "Chat created successfully", chat });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateChat(int id, [FromBody] ChatViewModels.ChatInputViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingChat = _manager.GetChatById(id);
            if (existingChat == null) return NotFound("Chat not found");

            existingChat.Status = model.Status;
            existingChat.Channel = model.Channel;
            existingChat.OrderId = model.OrderId;
            existingChat.CustomerId = model.CustomerId;
            existingChat.PharmacyId = model.PharmacyId;

            _manager.UpdateChat(existingChat);
            return Ok(new { Message = "Chat updated successfully", existingChat });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteChat(int id)
        {
            var chat = _manager.GetChatById(id);
            if (chat == null) return NotFound("Chat not found");

            _manager.DeleteChat(id);
            return Ok("Chat deleted successfully");
        }

        // =============================
        // MESSAGES
        // =============================

        [HttpGet("{chatId}/messages")]
        public IActionResult GetMessages(int chatId) => Ok(_manager.GetMessages(chatId));

        [HttpPost("{chatId}/messages")]
        public IActionResult AddMessage(int chatId, [FromBody] ChatViewModels.ChatMessageInputViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var msg = new ChatMessage
            {
                ChatId = chatId,
                MessageText = model.MessageText,
                SentAt = model.SentAt ?? DateTime.Now
            };

            _manager.AddMessage(msg);
            return Ok(new { Message = "Message sent successfully", msg });
        }
    }
}
