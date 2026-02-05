using System;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class HaberResponseDto
    {
        public int HaberId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;

        /// <summary>
        /// Vitrin fotoğrafı (slider'da gösterilir) - tek resim
        /// </summary>
        public string? VitrinResimUrl { get; set; }

        /// <summary>
        /// Habere ait tüm resimler (detay sayfasında slider olarak gösterilir)
        /// </summary>
        public List<HaberResimResponseDto> Resimler { get; set; } = new();

        public int Sira { get; set; }
        public DateTime YayinTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        // UI badge alanları
        public string? BadgeColor { get; set; }
        public string? BadgeText { get; set; }
    }
}
