using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    /// <summary>
    /// Personel filtreleme ve arama kriterleri - UI search functionality için
    /// </summary>
    public class PersonelFilterRequestDto
    {
        #region Temel Arama Kriterleri

        /// <summary>
        /// Ad Soyad arama (contains search)
        /// </summary>
        [StringLength(200)]
        public string? AdSoyad { get; set; }

        /// <summary>
        /// TC Kimlik No arama
        /// </summary>
        [StringLength(11, MinimumLength = 11)]
        public string? TcKimlikNo { get; set; }

        /// <summary>
        /// Sicil No arama
        /// </summary>
        public int? SicilNo { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        #endregion

        #region Organizasyon Filtreleri

        /// <summary>
        /// Departman filtresi
        /// </summary>
        public int? DepartmanId { get; set; }

        /// <summary>
        /// Servis filtresi
        /// </summary>
        public int? ServisId { get; set; }

        /// <summary>
        /// Unvan filtresi
        /// </summary>
        public int? UnvanId { get; set; }

        /// <summary>
        /// Hizmet binası filtresi
        /// </summary>
        public int? HizmetBinasiId { get; set; }

        /// <summary>
        /// Birden fazla departman seçimi
        /// </summary>
        public List<int>? DepartmanIds { get; set; }

        #endregion

        #region Durum Filtreleri

        /// <summary>
        /// Personel aktiflik durumu (Aktif/Pasif/Emekli)
        /// </summary>
        public PersonelAktiflikDurum? AktiflikDurum { get; set; }

        /// <summary>
        /// Personel tipi (Memur/İşçi/Taşeron)
        /// </summary>
        public PersonelTipi? PersonelTipi { get; set; }

        /// <summary>
        /// Cinsiyet filtresi
        /// </summary>
        public Cinsiyet? Cinsiyet { get; set; }

        #endregion

        #region Demografik Filtreler

        /// <summary>
        /// Minimum yaş
        /// </summary>
        [Range(18, 65)]
        public int? MinAge { get; set; }

        /// <summary>
        /// Maksimum yaş
        /// </summary>
        [Range(18, 65)]
        public int? MaxAge { get; set; }

        /// <summary>
        /// Medeni durumu
        /// </summary>
        public MedeniDurumu? MedeniDurumu { get; set; }

        /// <summary>
        /// Öğrenim durumu
        /// </summary>
        public OgrenimDurumu? OgrenimDurumu { get; set; }

        #endregion

        #region İlişkili Veri Filtreleri

        /// <summary>
        /// Çocuğu olan personeller
        /// </summary>
        public bool? HasChildren { get; set; }

        /// <summary>
        /// Sendika üyesi personeller
        /// </summary>
        public bool? IsSendikaUyesi { get; set; }

        /// <summary>
        /// Belirli sendika üyeleri
        /// </summary>
        public int? SendikaId { get; set; }

        /// <summary>
        /// Şehit yakınlığı durumu
        /// </summary>
        public SehitYakinligi? SehitYakinligi { get; set; }

        /// <summary>
        /// Yetki sahibi personeller
        /// </summary>
        public bool? HasAuthorizations { get; set; }

        #endregion

        #region Tarih Filtreleri

        /// <summary>
        /// İşe giriş başlangıç tarihi
        /// </summary>
        public DateTime? IseGirisBaslangicTarihi { get; set; }

        /// <summary>
        /// İşe giriş bitiş tarihi
        /// </summary>
        public DateTime? IseGirisBitisTarihi { get; set; }

        /// <summary>
        /// Doğum tarihi başlangıç
        /// </summary>
        public DateTime? DogumTarihiBaslangic { get; set; }

        /// <summary>
        /// Doğum tarihi bitiş
        /// </summary>
        public DateTime? DogumTarihiBitis { get; set; }

        #endregion

        #region Arama ve Sayfalama

        /// <summary>
        /// Genel arama terimi (AdSoyad, TC, Sicil, Email'de arar)
        /// </summary>
        [StringLength(100)]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Sayfa numarası
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Sayfa boyutu
        /// </summary>
        [Range(5, 100)]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Sıralama alanı (AdSoyad, SicilNo, DepartmanAdi, vb.)
        /// </summary>
        [StringLength(50)]
        public string SortBy { get; set; } = "AdSoyad";

        /// <summary>
        /// Azalan sıralama
        /// </summary>
        public bool SortDescending { get; set; } = false;

        #endregion

        #region Advanced Filters

        /// <summary>
        /// Sadece aktif olan kayıtlar (soft delete kontrolü)
        /// </summary>
        public bool IncludeDeleted { get; set; } = false;

        /// <summary>
        /// Navigation property'lerin yüklenmesi
        /// </summary>
        public bool IncludeRelatedData { get; set; } = true;

        /// <summary>
        /// Email adresi olan personeller
        /// </summary>
        public bool? HasEmail { get; set; }

        /// <summary>
        /// Fotoğrafı olan personeller
        /// </summary>
        public bool? HasPhoto { get; set; }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Filtrenin boş olup olmadığını kontrol eder
        /// </summary>
        /// <returns>True ise hiç filtre uygulanmamış</returns>
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(AdSoyad) &&
                   string.IsNullOrWhiteSpace(TcKimlikNo) &&
                   string.IsNullOrWhiteSpace(SearchTerm) &&
                   !SicilNo.HasValue &&
                   !DepartmanId.HasValue &&
                   !ServisId.HasValue &&
                   !UnvanId.HasValue &&
                   !AktiflikDurum.HasValue;
        }

        /// <summary>
        /// Sayfalama bilgilerini validation eder
        /// </summary>
        public void ValidatePagination()
        {
            if (PageNumber < 1) PageNumber = 1;
            if (PageSize < 5) PageSize = 5;
            if (PageSize > 100) PageSize = 100;
        }

        #endregion
    }
}