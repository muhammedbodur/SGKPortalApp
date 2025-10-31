using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Personel Atama Matrix için kullanılan DTO
    /// Her personel bir satır olarak gelir ve KanalAtamalari collection içerir
    /// </summary>
    public class PersonelAtamaMatrixDto
    {
        // Personel Bilgileri
        public string TcKimlikNo { get; set; } = string.Empty;
        public int SicilNo { get; set; } = 0;
        public string PersonelAdSoyad { get; set; } = string.Empty;
        public string Resim { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; }

        // Kanal Atama Bilgileri (Collection)
        public List<PersonelKanalAtamaDto> KanalAtamalari { get; set; } = new();
    }
}