using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Sıra yönlendirme seçenekleri response DTO
    /// Kullanıcıya sadece geçerli yönlendirme tiplerini ve bankoları gösterir
    /// </summary>
    public class YonlendirmeSecenekleriResponseDto
    {
        /// <summary>
        /// Kullanılabilir yönlendirme tipleri
        /// Örnek: ["BaskaBanko", "Sef", "UzmanPersonel"]
        /// </summary>
        public List<YonlendirmeTipi> AvailableTypes { get; set; } = new();

        /// <summary>
        /// Yönlendirilecek aktif bankolar listesi (BaskaBanko için)
        /// </summary>
        public List<BankoOptionDto> Bankolar { get; set; } = new();

        /// <summary>
        /// Şef personeli sayısı (bilgilendirme için)
        /// </summary>
        public int SefPersonelCount { get; set; }

        /// <summary>
        /// Uzman personel sayısı (bilgilendirme için)
        /// </summary>
        public int UzmanPersonelCount { get; set; }
    }

    /// <summary>
    /// Hedef banko seçeneği DTO
    /// </summary>
    public class BankoOptionDto
    {
        public int BankoId { get; set; }
        public int BankoNo { get; set; }
        public string PersonelAdi { get; set; } = string.Empty;
        public string KatAdi { get; set; } = string.Empty;

        /// <summary>
        /// Display için formatlanmış text
        /// Örnek: "Banko 1 (Zemin Kat) - Ahmet Yılmaz"
        /// </summary>
        public string DisplayText => $"Banko {BankoNo} ({KatAdi}) - {PersonelAdi}";
    }
}
