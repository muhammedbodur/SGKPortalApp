using System;

namespace SGKPortalApp.Common.Helpers
{
    /// <summary>
    /// Türkiye saat dilimi (UTC+3) için DateTime yardımcı sınıfı
    /// Tüm DateTime işlemlerinde bu sınıf kullanılmalıdır
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Türkiye saat dilimi bilgisi (Turkey Standard Time - UTC+3)
        /// </summary>
        private static readonly TimeZoneInfo TurkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

        /// <summary>
        /// Türkiye saat dilimine göre şu anki tarih ve saat
        /// DateTime.Now yerine bu property kullanılmalıdır
        /// </summary>
        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TurkeyTimeZone);

        /// <summary>
        /// Türkiye saat dilimine göre bugünün tarihi (saat 00:00:00)
        /// </summary>
        public static DateTime Today => Now.Date;

        /// <summary>
        /// UTC tarihini Türkiye saat dilimine çevirir
        /// </summary>
        /// <param name="utcDateTime">UTC tarih</param>
        /// <returns>Türkiye saat diliminde tarih</returns>
        public static DateTime ConvertFromUtc(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TurkeyTimeZone);
        }

        /// <summary>
        /// Türkiye saat dilimindeki tarihi UTC'ye çevirir
        /// </summary>
        /// <param name="turkeyDateTime">Türkiye saat diliminde tarih</param>
        /// <returns>UTC tarih</returns>
        public static DateTime ConvertToUtc(DateTime turkeyDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(turkeyDateTime, TurkeyTimeZone);
        }

        /// <summary>
        /// Nullable UTC tarihini Türkiye saat dilimine çevirir
        /// </summary>
        public static DateTime? ConvertFromUtc(DateTime? utcDateTime)
        {
            return utcDateTime.HasValue ? ConvertFromUtc(utcDateTime.Value) : null;
        }

        /// <summary>
        /// Nullable Türkiye saat dilimindeki tarihi UTC'ye çevirir
        /// </summary>
        public static DateTime? ConvertToUtc(DateTime? turkeyDateTime)
        {
            return turkeyDateTime.HasValue ? ConvertToUtc(turkeyDateTime.Value) : null;
        }

        /// <summary>
        /// Türk tarih formatında string döndürür (dd.MM.yyyy)
        /// </summary>
        public static string ToTurkishDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Türk tarih-saat formatında string döndürür (dd.MM.yyyy HH:mm)
        /// </summary>
        public static string ToTurkishDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy HH:mm");
        }

        /// <summary>
        /// Türk tarih-saat formatında string döndürür (saniye ile - dd.MM.yyyy HH:mm:ss)
        /// </summary>
        public static string ToTurkishDateTimeStringWithSeconds(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
        }

        /// <summary>
        /// Nullable DateTime için Türk tarih formatında string döndürür
        /// </summary>
        public static string ToTurkishDateString(this DateTime? dateTime)
        {
            return dateTime?.ToString("dd.MM.yyyy") ?? "-";
        }

        /// <summary>
        /// Nullable DateTime için Türk tarih-saat formatında string döndürür
        /// </summary>
        public static string ToTurkishDateTimeString(this DateTime? dateTime)
        {
            return dateTime?.ToString("dd.MM.yyyy HH:mm") ?? "-";
        }
    }
}
