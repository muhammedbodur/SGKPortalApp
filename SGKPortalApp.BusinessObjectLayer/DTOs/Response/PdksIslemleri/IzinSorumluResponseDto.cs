using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// İzin Sorumlusu Response DTO
    /// </summary>
    public class IzinSorumluResponseDto
    {
        public int IzinSorumluId { get; set; }

        /// <summary>
        /// Departman ID (Null ise tüm departmanlar)
        /// </summary>
        public int? DepartmanId { get; set; }

        /// <summary>
        /// Departman Adı
        /// </summary>
        public string DepartmanAdi { get; set; } = string.Empty;

        /// <summary>
        /// Servis ID (Null ise tüm servisler)
        /// </summary>
        public int? ServisId { get; set; }

        /// <summary>
        /// Servis Adı
        /// </summary>
        public string? ServisAdi { get; set; }

        /// <summary>
        /// Sorumlu Personel TC Kimlik No
        /// </summary>
        public string SorumluPersonelTcKimlikNo { get; set; } = string.Empty;

        /// <summary>
        /// Sorumlu Personel Adı Soyadı
        /// </summary>
        public string SorumluPersonelAdSoyad { get; set; } = string.Empty;

        /// <summary>
        /// Sorumlu Personel Sicil No
        /// </summary>
        public int SorumluPersonelSicilNo { get; set; }

        /// <summary>
        /// Onay seviyesi: 1 = Birinci Onayci, 2 = İkinci Onayci
        /// </summary>
        public int OnaySeviyesi { get; set; }

        /// <summary>
        /// Onay seviyesi açıklaması
        /// </summary>
        public string OnaySeviyesiAciklama => OnaySeviyesi == 1 ? "Birinci Onaycı" : "İkinci Onaycı";

        /// <summary>
        /// Aktif/Pasif durum
        /// </summary>
        public bool Aktif { get; set; }

        /// <summary>
        /// Açıklama/Not
        /// </summary>
        public string? Aciklama { get; set; }

        /// <summary>
        /// Eklenme tarihi
        /// </summary>
        public DateTime EklenmeTarihi { get; set; }

        /// <summary>
        /// Düzenlenme tarihi
        /// </summary>
        public DateTime? DuzenlenmeTarihi { get; set; }
    }
}
