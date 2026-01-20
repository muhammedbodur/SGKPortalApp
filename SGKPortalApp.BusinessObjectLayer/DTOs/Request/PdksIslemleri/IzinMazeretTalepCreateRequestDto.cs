using System;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret talebi oluşturma DTO
    /// İzin veya Mazeret türüne göre farklı alanlar zorunlu olur
    /// </summary>
    public class IzinMazeretTalepCreateRequestDto
    {
        /// <summary>
        /// Talep sahibi personel TC Kimlik No
        /// </summary>
        [Required(ErrorMessage = "Personel TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        /// <summary>
        /// İzin/Mazeret türü ID (IzinMazeretTuruTanim tablosundan)
        /// </summary>
        [Required(ErrorMessage = "İzin/Mazeret türü zorunludur")]
        public int IzinMazeretTuruId { get; set; }

        /// <summary>
        /// Açıklama/Sebep
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }

        // ═══════════════════════════════════════════════════════
        // İZİN ALANLARI (Planlı izinler için - Turu != Mazeret ise zorunlu)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Başlangıç tarihi (İzin için zorunlu)
        /// </summary>
        public DateTime? BaslangicTarihi { get; set; }

        /// <summary>
        /// Bitiş tarihi (İzin için zorunlu)
        /// </summary>
        public DateTime? BitisTarihi { get; set; }

        // ═══════════════════════════════════════════════════════
        // MAZERET ALANLARI (Geriye dönük mazeretler için - Turu == Mazeret ise zorunlu)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Mazeret tarihi (Mazeret için zorunlu)
        /// </summary>
        public DateTime? MazeretTarihi { get; set; }

        /// <summary>
        /// Başlangıç saati (Mazeret için zorunlu)
        /// </summary>
        public TimeSpan? BaslangicSaati { get; set; }

        /// <summary>
        /// Bitiş saati (Mazeret için zorunlu)
        /// </summary>
        public TimeSpan? BitisSaati { get; set; }

        // ═══════════════════════════════════════════════════════
        // ONAY BİLGİLERİ (Opsiyonel - sistem tarafından atanabilir)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// 1. Onayci TC Kimlik No (opsiyonel, sistem otomatik atayabilir)
        /// </summary>
        [StringLength(11)]
        public string? BirinciOnayciTcKimlikNo { get; set; }

        /// <summary>
        /// 2. Onayci TC Kimlik No (opsiyonel, sistem otomatik atayabilir)
        /// </summary>
        [StringLength(11)]
        public string? IkinciOnayciTcKimlikNo { get; set; }

        /// <summary>
        /// Belge eki (ör: Rapor, Evlilik cüzdanı vb.)
        /// </summary>
        [StringLength(1000)]
        public string? BelgeEki { get; set; }
    }
}
