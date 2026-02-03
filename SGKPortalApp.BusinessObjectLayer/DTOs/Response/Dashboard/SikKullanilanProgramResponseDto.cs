using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard
{
    public class SikKullanilanProgramResponseDto
    {
        public int ProgramId { get; set; }
        public string ProgramAdi { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string IkonClass { get; set; } = "bx-desktop";
        public string RenkKodu { get; set; } = "primary";
        public int Sira { get; set; }
        public Aktiflik Aktiflik { get; set; }
    }
}
