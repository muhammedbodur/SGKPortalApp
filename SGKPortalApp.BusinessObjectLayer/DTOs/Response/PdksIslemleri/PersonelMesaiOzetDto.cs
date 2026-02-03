namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// Personel Mesai Özet DTO - Departman raporunda kullanılır
    /// </summary>
    public class PersonelMesaiOzetDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public string? ServisAdi { get; set; }
        public int ToplamGun { get; set; }
        public int CalistigiGun { get; set; }
        public int IzinliGun { get; set; }
        public int MazeretliGun { get; set; }
        public int DevamsizGun { get; set; }
        public int HaftaSonuCalisma { get; set; }
        public int GecKalma { get; set; }
        public string ToplamMesaiSuresi { get; set; } = "00:00"; // HH:MM format
        public int ToplamMesaiDakika { get; set; }
        public List<PersonelMesaiGunlukDto> GunlukDetay { get; set; } = new();
    }
}
