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

    /// <summary>
    /// TV ekranına basit sıra bildirimi (eski yapı için - receiveSiraUpdate)
    /// </summary>
    public class TvSiraUpdateDto
    {
        public int SiraNo { get; set; }
        public string BankoNo { get; set; } = string.Empty;
        public string KanalAltAdi { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// TV ekranına sıra çağırma bildirimi için kullanılan DTO
    /// Request/Command Pattern ile clean architecture
    /// </summary>
    public class TvSiraCalledDto
    {
        /// <summary>
        /// Çağrılan sıra numarası
        /// </summary>
        public int SiraNo { get; set; }

        /// <summary>
        /// Banko numarası (string format: "1", "2", "A1" vb.)
        /// </summary>
        public string BankoNo { get; set; } = string.Empty;

        /// <summary>
        /// Banko ID
        /// </summary>
        public int BankoId { get; set; }

        /// <summary>
        /// Banko tipi (BANKO, ÖNCELİKLİ BANKO, ENGELLİ BANKO, ŞEF MASASI)
        /// </summary>
        public string BankoTipi { get; set; } = string.Empty;

        /// <summary>
        /// Kat tipi (Zemin Kat, 1. Kat vb.)
        /// </summary>
        public string KatTipi { get; set; } = string.Empty;

        /// <summary>
        /// Kanal alt işlem adı (hizmet türü)
        /// </summary>
        public string KanalAltAdi { get; set; } = string.Empty;

        /// <summary>
        /// Güncelleme tipi (SiraCalled, SiraCompleted vb.)
        /// </summary>
        public string UpdateType { get; set; } = "SiraCalled";

        /// <summary>
        /// TV tarafında çağrı overlayi gösterilsin mi?
        /// </summary>
        public bool ShowOverlay { get; set; } = true;

        /// <summary>
        /// TV ekranında gösterilecek tüm aktif sıralar listesi
        /// </summary>
        public List<TvSiraItemDto> Siralar { get; set; } = new();

        /// <summary>
        /// İşlem zamanı
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// TV ekranındaki sıra listesi için item DTO
    /// </summary>
    public class TvSiraItemDto
    {
        public int BankoId { get; set; }
        public int BankoNo { get; set; }
        public string KatTipi { get; set; } = string.Empty;
        public int SiraNo { get; set; }
    }
}
