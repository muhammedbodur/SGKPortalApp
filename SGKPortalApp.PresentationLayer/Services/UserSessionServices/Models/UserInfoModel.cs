namespace SGKPortalApp.PresentationLayer.Services.UserSessionServices.Models
{
    /// <summary>
    /// Kullanıcı bilgilerini taşıyan model
    /// </summary>
    public class UserInfoModel
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int ServisId { get; set; }
        public string ServisAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string? Resim { get; set; }
        public string? ResimRoute { get; set; }
    }
}
