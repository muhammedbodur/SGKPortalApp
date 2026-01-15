using System;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret talebi güncelleme DTO
    /// Sadece talep sahibi ve beklemede olan talepler güncellenebilir
    /// </summary>
    public class IzinMazeretTalepUpdateRequestDto
    {
        /// <summary>
        /// İzin/Mazeret türü
        /// </summary>
        [Required(ErrorMessage = "İzin/Mazeret türü zorunludur")]
        public IzinMazeretTuru Turu { get; set; }

        /// <summary>
        /// Açıklama/Sebep
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }

        // ═══════════════════════════════════════════════════════
        // İZİN ALANLARI
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
        // MAZERET ALANLARI
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Mazeret tarihi (Mazeret için zorunlu)
        /// </summary>
        public DateTime? MazeretTarihi { get; set; }

        /// <summary>
        /// Saat dilimi (Mazeret için)
        /// </summary>
        [StringLength(50)]
        public string? SaatDilimi { get; set; }

        /// <summary>
        /// Belge eki
        /// </summary>
        [StringLength(1000)]
        public string? BelgeEki { get; set; }
    }
}
