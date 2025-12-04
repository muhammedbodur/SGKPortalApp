namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Kiosk Menü DTO (Vatandaşın ilk gördüğü ekran)
    /// </summary>
    public class KioskMenuDto
    {
        public int KioskMenuId { get; set; }
        public string MenuAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public int MenuSira { get; set; }
        
        /// <summary>
        /// Bu menüdeki toplam bekleyen sıra sayısı
        /// </summary>
        public int ToplamBekleyenSiraSayisi { get; set; }
        
        /// <summary>
        /// Bu menüde aktif personeli olan alt işlem sayısı
        /// </summary>
        public int AktifAltIslemSayisi { get; set; }
    }
}
