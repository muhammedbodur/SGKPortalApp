namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// TV ekranında gösterilecek sıra bilgisi
    /// </summary>
    public class TvSiraDto
    {
        public int BankoId { get; set; }
        public int BankoNo { get; set; }
        public string KatTipi { get; set; } = string.Empty;
        public int SiraNo { get; set; }
    }
}
