namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelCezaResponseDto
    {
        public int PersonelCezaId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public string CezaSebebi { get; set; } = string.Empty;
        public string? AltBendi { get; set; }
        public DateTime CezaTarihi { get; set; }
        public string? Aciklama { get; set; }
    }
}
