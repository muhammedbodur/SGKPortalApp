using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System.Text.Json.Serialization;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Elasticsearch
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
        /// Ad Soyad
        /// </summary>
        public string AdSoyad { get; set; } = string.Empty;

        /// <summary>
        /// Sicil numarası (string olarak - arama için)
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
        /// Aktiflik durumu (integer olarak serialize edilir)
        /// </summary>
        [JsonConverter(typeof(JsonNumberEnumConverter<PersonelAktiflikDurum>))]
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
