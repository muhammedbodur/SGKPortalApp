namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Sıra numarası bilgisi DTO'su
    /// Eski proje mantığı ile uyumlu - KanalIslem bazında sıra numarası hesaplama
    /// </summary>
    public class SiraNoBilgisiDto
    {
        public int SiraNo { get; set; }
        public int DepartmanHizmetBinasiId { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
        public string KanalAltAdi { get; set; } = string.Empty;
        public int KanalAltIslemId { get; set; }
        public int KanalIslemId { get; set; }
        public int BaslangicNumara { get; set; }
        public int BitisNumara { get; set; }
    }
}
