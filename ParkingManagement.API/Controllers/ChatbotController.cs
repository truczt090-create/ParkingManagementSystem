using Microsoft.AspNetCore.Mvc;
using ParkingManagement.API.DTOs.Chatbot;
using ParkingManagement.API.Helpers;
using ParkingManagement.API.Services.Interfaces;

namespace ParkingManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/chatbot")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;
        public ChatbotController(IChatbotService chatbotService) => _chatbotService = chatbotService;

        [HttpPost("ask")]
        public async Task<IActionResult> Ask(ChatRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Question))
                return BadRequest(ApiResponse<object>.Fail("Câu hỏi không được để trống"));

            var answer = await _chatbotService.AskAsync(req);
            return Ok(ApiResponse<ChatResponse>.Ok(new ChatResponse(answer)));
        }
    }
}