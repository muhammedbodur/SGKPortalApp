using System;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret talepleri filtreleme DTO
    /// Raporlama ve listeleme için kullanılır
    /// Permission-based filtering destekler
    /// </summary>
    public class IzinMazeretTalepFilterRequestDto
    {
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string? TcKimlikNo { get; set; }

        /// <summary>
        /// Ad Soyad (partial match)
        /// </summary>
        public string? AdSoyad { get; set; }

        /// <summary>
        /// Sicil No
        /// </summary>
        public int? SicilNo { get; set; }

        /// <summary>
        /// Departman ID (permission-based filtering)
        /// </summary>
        public int? DepartmanId { get; set; }

        /// <summary>
        /// Servis ID (permission-based filtering)
        /// </summary>
        public int? ServisId { get; set; }

        /// <summary>
        /// İzin/Mazeret türü ID
        /// </summary>
        public int? IzinMazeretTuruId { get; set; }

        /// <summary>
        /// 1. Onay durumu
        /// </summary>
        public OnayDurumu? BirinciOnayDurumu { get; set; }

        /// <summary>
        /// 2. Onay durumu
        /// </summary>
        public OnayDurumu? IkinciOnayDurumu { get; set; }

        /// <summary>
        /// Başlangıç tarihi (filtre için)
        /// </summary>
        public DateTime? BaslangicTarihiMin { get; set; }

        /// <summary>
        /// Başlangıç tarihi (filtre için)
        /// </summary>
        public DateTime? BaslangicTarihiMax { get; set; }

        /// <summary>
        /// Bitiş tarihi (filtre için)
        /// </summary>
        public DateTime? BitisTarihiMin { get; set; }

        /// <summary>
        /// Bitiş tarihi (filtre için)
        /// </summary>
        public DateTime? BitisTarihiMax { get; set; }

        /// <summary>
        /// Talep tarihi (filtre için)
        /// </summary>
        public DateTime? TalepTarihiMin { get; set; }

        /// <summary>
        /// Talep tarihi (filtre için)
        /// </summary>
        public DateTime? TalepTarihiMax { get; set; }

        /// <summary>
        /// Aktif mi? (true=aktif, false=iptal edilmiş, null=hepsi)
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Sayfa numarası (pagination)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Sayfa boyutu (pagination)
        /// </summary>
        public int PageSize { get; set; } = 50;

        /// <summary>
        /// Sıralama alanı
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Azalan sıralama mı?
        /// </summary>
        public bool SortDescending { get; set; } = false;
    }
}
