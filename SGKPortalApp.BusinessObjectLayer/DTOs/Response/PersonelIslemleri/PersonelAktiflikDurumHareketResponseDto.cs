using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelAktiflikDurumHareketResponseDto
    {
        public int PersonelAktiflikDurumHareketId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public PersonelAktiflikDurum? OncekiDurum { get; set; }
        public string? OncekiDurumAdi { get; set; }
        public PersonelAktiflikDurum YeniDurum { get; set; }
        public string? YeniDurumAdi { get; set; }
        public DateTime DegisiklikTarihi { get; set; }
        public DateTime? GecerlilikBaslangicTarihi { get; set; }
        public DateTime? GecerlilikBitisTarihi { get; set; }
        public string? Aciklama { get; set; }
        public string? BelgeNo { get; set; }
        public string? EkleyenKullanici { get; set; }
        public DateTime? EklenmeTarihi { get; set; }
    }
}
