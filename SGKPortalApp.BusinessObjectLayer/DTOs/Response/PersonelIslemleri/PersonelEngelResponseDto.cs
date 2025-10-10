using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelEngelResponseDto
    {
        public int PersonelEngelId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public EngelDerecesi EngelDerecesi { get; set; }
        public string? EngelNedeni1 { get; set; }
        public string? EngelNedeni2 { get; set; }
        public string? EngelNedeni3 { get; set; }
        public string? Aciklama { get; set; }
    }
}
