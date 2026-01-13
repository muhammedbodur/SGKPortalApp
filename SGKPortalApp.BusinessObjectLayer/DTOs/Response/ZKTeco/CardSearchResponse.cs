using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco
{
    /// <summary>
    /// Kart sorgulama yanıtı
    /// </summary>
    public class CardSearchResponse
    {
        /// <summary>
        /// Sorgulama başarılı mı?
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mesaj
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Cihaz sonuçları
        /// </summary>
        public List<DeviceUserMatch> DeviceResults { get; set; } = new List<DeviceUserMatch>();

        /// <summary>
        /// Toplam bulunan kullanıcı sayısı
        /// </summary>
        public int TotalMatches { get; set; }

        /// <summary>
        /// Uyuşmazlık sayısı
        /// </summary>
        public int MismatchCount { get; set; }

        /// <summary>
        /// Aranan cihaz sayısı
        /// </summary>
        public int SearchedDeviceCount { get; set; }
    }
}
