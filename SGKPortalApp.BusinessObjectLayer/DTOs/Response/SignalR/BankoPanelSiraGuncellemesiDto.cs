using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
    /// <summary>
    /// Banko Panel Sıra Güncellemesi için DTO
    /// Kiosk'tan sıra alındığında veya yönlendirme sonrası personele bildirim için kullanılır
    /// </summary>
    public class BankoPanelSiraGuncellemesiDto
    {
        /// <summary>
        /// Güncellenen sıra ID
        /// </summary>
        public int SiraId { get; set; }

        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string PersonelTc { get; set; } = string.Empty;

        /// <summary>
        /// Güncellenen sıra bilgisi
        /// </summary>
        public object? Sira { get; set; }

        /// <summary>
        /// Sıranın personelin listesindeki pozisyonu (0-indexed)
        /// </summary>
        public int Pozisyon { get; set; }

        /// <summary>
        /// Personelin toplam sıra sayısı
        /// </summary>
        public int ToplamSiraSayisi { get; set; }

        /// <summary>
        /// İşlem zamanı
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
