using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Chatbot;

namespace ParkingManagement.Web.Controllers;

public class ChatbotController : Controller
{
    private readonly IChatbotService _chatbotService;
    public ChatbotController(IChatbotService chatbotService) => _chatbotService = chatbotService;

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] AskRequestViewModel model)
    {
        var result = await _chatbotService.AskAsync(model);
        var answer = result?.Data?.GetValueOrDefault("answer") ?? "Xin lỗi, chatbot đang gặp sự cố.";
        return Json(new { answer });
    }
}