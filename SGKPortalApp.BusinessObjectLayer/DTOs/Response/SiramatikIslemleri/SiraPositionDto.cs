namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Sıranın personelin panelindeki pozisyon bilgisi
    /// </summary>
    public class SiraPositionDto
    {
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string TcKimlikNo { get; set; } = string.Empty;

        /// <summary>
        /// Sıranın personelin panelindeki pozisyonu (0-based index)
        /// </summary>
        public int Position { get; set; }
    }
}
