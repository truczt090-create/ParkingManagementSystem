using Microsoft.EntityFrameworkCore;
using ParkingManagement.API.Data;
using ParkingManagement.API.Models;

namespace ParkingManagement.API.Services.BackgroundJobs
{
    public class OvertimeReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OvertimeReminderService> _logger;

        // Quét mỗi 5 phút — có thể rút ngắn xuống 30 giây khi test cho nhanh thấy kết quả
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public OvertimeReminderService(IServiceScopeFactory scopeFactory, ILogger<OvertimeReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndNotifyAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi trong OvertimeReminderService");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task CheckAndNotifyAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ParkingDbContext>();

            var now = DateTime.UtcNow;
            var soonThreshold = now.AddMinutes(15);

            // Lấy các Session đang gửi, thuộc Booking đã xác nhận, sắp hoặc đã quá EndTime
            var activeSessions = await db.ParkingSessions
                .Include(s => s.Booking)
                .Where(s => s.Status == "DangGui" && s.BookingId != null &&
                            s.Booking!.Status == "DaXacNhan" &&
                            s.Booking.EndTime <= soonThreshold)
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                var booking = session.Booking!;
                var isOvertime = now > booking.EndTime;
                var marker = $"[Booking#{booking.BookingId}]";
                var type = isOvertime ? "DaQuaGio" : "NhacNhoSapHetGio";

                // Tránh gửi trùng: kiểm tra đã có thông báo cùng loại cho booking này chưa
                var alreadyNotified = await db.Notifications.AnyAsync(n =>
                    n.UserId == booking.UserId && n.Type == type && n.Content.Contains(marker));

                if (alreadyNotified) continue;

                var title = isOvertime ? "Đã quá giờ gửi xe" : "Sắp hết giờ gửi xe";
                var content = isOvertime
                    ? $"{marker} Booking của bạn đã quá giờ. Phụ phí đang được tính, gia hạn ngay để tránh phát sinh thêm."
                    : $"{marker} Booking của bạn sắp hết giờ (còn dưới 15 phút). Gia hạn ngay trên app nếu cần thêm thời gian.";

                db.Notifications.Add(new Notification
                {
                    UserId = booking.UserId,
                    Title = title,
                    Content = content,
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (activeSessions.Any())
                await db.SaveChangesAsync();
        }
    }
}