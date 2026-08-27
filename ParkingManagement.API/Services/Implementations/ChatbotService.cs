using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.API.Data;
using ParkingManagement.API.DTOs.Chatbot;
using ParkingManagement.API.Services.Interfaces;

namespace ParkingManagement.API.Services.Implementations
{
    public class ChatbotService : IChatbotService
    {
        private readonly ParkingDbContext _db;
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;

        public ChatbotService(ParkingDbContext db, IHttpClientFactory httpFactory, IConfiguration config)
        {
            _db = db;
            _httpFactory = httpFactory;
            _config = config;
        }

        public async Task<string> AskAsync(ChatRequest request)
        {
            // Bước 1: Lấy dữ liệu thật từ Database làm "ngữ cảnh" cho Gemini
            var context = await BuildContextAsync(request.ParkingLotId);

            // Bước 2: Ghép prompt — ép Gemini chỉ trả lời dựa trên dữ liệu thật, tránh bịa
            var prompt = $"""
                Bạn là trợ lý ảo của hệ thống bãi đỗ xe thông minh. Chỉ trả lời dựa trên DỮ LIỆU dưới đây,
                không tự bịa thông tin. Nếu không đủ dữ liệu để trả lời, hãy nói rõ là chưa có thông tin đó.
                Trả lời ngắn gọn, thân thiện, bằng tiếng Việt.

                DỮ LIỆU:
                {context}

                CÂU HỎI CỦA KHÁCH: {request.Question}
                """;

            return await CallGeminiAsync(prompt);
        }

        private async Task<string> BuildContextAsync(int? parkingLotId)
        {
            if (parkingLotId == null)
            {
                var lots = await _db.ParkingLots
                    .Where(l => l.Status == "Approved")
                    .Select(l => $"- {l.Name} ({l.Address}), giờ mở cửa {l.OpenTime}-{l.CloseTime}")
                    .Take(10)
                    .ToListAsync();

                return "Danh sách bãi xe đang hoạt động:\n" + string.Join("\n", lots);
            }

            var lot = await _db.ParkingLots.FirstOrDefaultAsync(l => l.ParkingLotId == parkingLotId);
            if (lot == null) return "Không tìm thấy bãi xe này.";

            var prices = await _db.Prices
                .Include(p => p.VehicleType)
                .Where(p => p.ParkingLotId == parkingLotId)
                .Select(p => $"- {p.VehicleType.TypeName}, loại {p.PriceType}: {p.UnitPrice:N0}đ/giờ")
                .ToListAsync();

            var slotStats = await _db.ParkingSlots
                .Where(s => s.ParkingArea.ParkingLotId == parkingLotId)
                .GroupBy(s => s.VehicleType.TypeName)
                .Select(g => new
                {
                    VehicleType = g.Key,
                    Total = g.Count(),
                    Available = g.Count(s => s.Status == "Trống")
                })
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine($"Bãi xe: {lot.Name}");
            sb.AppendLine($"Địa chỉ: {lot.Address}");
            sb.AppendLine($"Giờ mở cửa: {lot.OpenTime} - {lot.CloseTime}");
            sb.AppendLine("Bảng giá:");
            sb.AppendLine(string.Join("\n", prices));
            sb.AppendLine("Số chỗ trống hiện tại:");
            foreach (var s in slotStats)
                sb.AppendLine($"- {s.VehicleType}: còn {s.Available}/{s.Total} chỗ");

            return sb.ToString();
        }

        private async Task<string> CallGeminiAsync(string prompt)
        {
            var apiKey = _config["Gemini:ApiKey"];
            var model = _config["Gemini:Model"] ?? "gemini-2.0-flash";

            var client = _httpFactory.CreateClient("GeminiAPI");

            var body = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var response = await client.PostAsync(
                $"models/{model}:generateContent?key={apiKey}",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
                return "Xin lỗi, chatbot đang gặp sự cố, vui lòng thử lại sau.";

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "Xin lỗi, tôi chưa có câu trả lời cho câu hỏi này.";
        }
    }
}