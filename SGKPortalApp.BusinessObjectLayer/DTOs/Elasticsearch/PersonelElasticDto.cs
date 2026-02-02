using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Elasticsearch
{
    /// <summary>
    /// Elasticsearch'te indexlenecek denormalize personel dokümanı
    /// SQL = source of truth, Elasticsearch = sadece arama
    /// </summary>
    public class PersonelElasticDto
    {
        /// <summary>
        /// TC Kimlik No - Primary identifier
        /// </summary>
        public string TcKimlikNo { get; set; } = string.Empty;

        /// <summary>
        /// Personel adı
        /// </summary>
        public string Ad { get; set; } = string.Empty;

        /// <summary>
        /// Personel soyadı
        /// </summary>
        public string Soyad { get; set; } = string.Empty;

        /// <summary>
        /// Ad Soyad birleşik
        /// </summary>
        public string AdSoyad { get; set; } = string.Empty;

        /// <summary>
        /// Sicil numarası
        /// </summary>
        public string? SicilNo { get; set; }

        /// <summary>
        /// Departman ID (filtreleme için)
        /// </summary>
        public int DepartmanId { get; set; }

        /// <summary>
        /// Departman adı (denormalize)
        /// </summary>
        public string? DepartmanAdi { get; set; }

        /// <summary>
        /// Servis ID (filtreleme için)
        /// </summary>
        public int ServisId { get; set; }

        /// <summary>
        /// Servis adı (denormalize)
        /// </summary>
        public string? ServisAdi { get; set; }

        /// <summary>
        /// Ünvan ID
        /// </summary>
        public int? UnvanId { get; set; }

        /// <summary>
        /// Ünvan adı (denormalize)
        /// </summary>
        public string? UnvanAdi { get; set; }

        /// <summary>
        /// Personel resim URL
        /// </summary>
        public string? Resim { get; set; }

        /// <summary>
        /// Aktiflik durumu
        /// </summary>
        public PersonelAktiflikDurum PersonelAktiflikDurum { get; set; }

        /// <summary>
        /// Aktif mi? (hızlı filtreleme için)
        /// </summary>
        public bool Aktif { get; set; }

        /// <summary>
        /// Full-text arama için birleştirilmiş alan
        /// Ad + Soyad + Departman + Servis + Ünvan + SicilNo
        /// </summary>
        public string FullText { get; set; } = string.Empty;

        /// <summary>
        /// Son güncelleme tarihi
        /// </summary>
        public DateTime GuncellemeTarihi { get; set; }
    }
}
