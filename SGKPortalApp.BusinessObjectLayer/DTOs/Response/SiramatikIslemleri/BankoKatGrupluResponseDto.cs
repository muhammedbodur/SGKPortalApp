using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Kat bazlı gruplu banko listesi için kullanılan DTO
    /// </summary>
    public class BankoKatGrupluResponseDto
    {
        public KatTipi KatTipi { get; set; }
        public string KatTipiAdi { get; set; } = string.Empty;
        public List<BankoResponseDto> Bankolar { get; set; } = new List<BankoResponseDto>();
    }
}
