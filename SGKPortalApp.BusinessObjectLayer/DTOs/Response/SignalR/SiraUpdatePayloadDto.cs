using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
    /// <summary>
    /// SignalR üzerinden gönderilecek sıra güncelleme payload'ı
    /// </summary>
    public class SiraUpdatePayloadDto
    {
        /// <summary>
        /// Güncelleme tipi: APPEND, REMOVE, INSERT, REFRESH
        /// </summary>
        public SiraUpdateType UpdateType { get; set; }

        /// <summary>
        /// İşlem yapılan sıra bilgisi
        /// </summary>
        public SiraCagirmaResponseDto? Sira { get; set; }

        /// <summary>
        /// Sıranın listede olması gereken pozisyon (INSERT için)
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// İşlemi yapan personel TC
        /// </summary>
        public string? PersonelTc { get; set; }

        /// <summary>
        /// İşlemi yapan banko ID
        /// </summary>
        public int? BankoId { get; set; }

        /// <summary>
        /// Ek açıklama (yönlendirme nedeni vb.)
        /// </summary>
        public string? Aciklama { get; set; }

        /// <summary>
        /// Bu sıranın üstünde olması gereken sıra ID (INSERT için konum belirleme)
        /// </summary>
        public int? PreviousSiraId { get; set; }

        /// <summary>
        /// Bu sıranın altında olması gereken sıra ID (INSERT için konum belirleme)
        /// </summary>
        public int? NextSiraId { get; set; }

        /// <summary>
        /// İşlem zamanı
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Sıra güncelleme tipleri
    /// </summary>
    public enum SiraUpdateType
    {
        /// <summary>
        /// Listeye yeni sıra eklendi (sona)
        /// </summary>
        Append = 1,

        /// <summary>
        /// Listeden sıra kaldırıldı
        /// </summary>
        Remove = 2,

        /// <summary>
        /// Belirli pozisyona sıra eklendi
        /// </summary>
        Insert = 3,

        /// <summary>
        /// Tüm liste yenilenmeli
        /// </summary>
        Refresh = 4,

        /// <summary>
        /// Mevcut sıra güncellendi (durum değişikliği)
        /// </summary>
        Update = 5
    }
}
