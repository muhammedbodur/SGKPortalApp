using System;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    public class SpecialCardResponseDto
    {
        public int Id { get; set; }
        public CardType CardType { get; set; }
        public string CardTypeName { get; set; } = string.Empty;
        public long CardNumber { get; set; }
        public string CardName { get; set; } = string.Empty;
        public string EnrollNumber { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
