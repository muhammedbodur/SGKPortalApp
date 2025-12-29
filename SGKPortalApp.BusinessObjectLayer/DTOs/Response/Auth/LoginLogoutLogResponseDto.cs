using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth
{
    public class LoginLogoutLogResponseDto
    {
        public int LoginLogoutLogId { get; set; }
        public string? TcKimlikNo { get; set; }
        public string? AdSoyad { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string? SessionID { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Browser { get; set; }
        public string? OperatingSystem { get; set; }
        public string? DeviceType { get; set; }
        public bool LoginSuccessful { get; set; }
        public string? FailureReason { get; set; }

        // Hesaplanan alanlar
        public TimeSpan? SessionDuration =>
            LogoutTime.HasValue ? LogoutTime.Value - LoginTime : null;

        public bool IsActiveSession => !LogoutTime.HasValue;

        public string SessionDurationText
        {
            get
            {
                if (!SessionDuration.HasValue)
                    return IsActiveSession ? "Aktif Oturum" : "-";

                var duration = SessionDuration.Value;
                if (duration.TotalMinutes < 1)
                    return $"{(int)duration.TotalSeconds} saniye";
                if (duration.TotalHours < 1)
                    return $"{(int)duration.TotalMinutes} dakika";
                if (duration.TotalDays < 1)
                    return $"{(int)duration.TotalHours} saat {duration.Minutes} dakika";

                return $"{(int)duration.TotalDays} gün {duration.Hours} saat";
            }
        }

        public string LoginTimeText => LoginTime.ToString("dd.MM.yyyy HH:mm:ss");
        public string LogoutTimeText => LogoutTime?.ToString("dd.MM.yyyy HH:mm:ss") ?? "-";
    }
}
