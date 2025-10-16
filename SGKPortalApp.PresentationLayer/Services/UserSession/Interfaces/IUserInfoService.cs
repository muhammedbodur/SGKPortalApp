namespace SGKPortalApp.PresentationLayer.Services.UserSession.Interfaces
{
    /// <summary>
    /// Oturum açmış kullanıcının bilgilerine her yerden erişim sağlar.
    /// Claims-based authentication üzerinden güvenli bir şekilde bilgileri okur.
    /// </summary>
    public interface IUserInfoService
    {
        /// <summary>
        /// Kullanıcının TC Kimlik Numarası (11 haneli)
        /// </summary>
        string GetTcKimlikNo();

        /// <summary>
        /// Kullanıcının Sicil Numarası
        /// </summary>
        int GetSicilNo();

        /// <summary>
        /// Kullanıcının Adı Soyadı
        /// </summary>
        string GetAdSoyad();

        /// <summary>
        /// Kullanıcının Email Adresi
        /// </summary>
        string GetEmail();

        /// <summary>
        /// Kullanıcının Departman ID'si
        /// </summary>
        int GetDepartmanId();

        /// <summary>
        /// Kullanıcının Departman Adı
        /// </summary>
        string GetDepartmanAdi();

        /// <summary>
        /// Kullanıcının Servis ID'si
        /// </summary>
        int GetServisId();

        /// <summary>
        /// Kullanıcının Servis Adı
        /// </summary>
        string GetServisAdi();

        /// <summary>
        /// Kullanıcının Hizmet Binası ID'si
        /// </summary>
        int GetHizmetBinasiId();

        /// <summary>
        /// Kullanıcının Hizmet Binası Adı
        /// </summary>
        string GetHizmetBinasiAdi();

        /// <summary>
        /// Login Session ID - Logout olana kadar değişmez
        /// Aynı kullanıcının farklı cihazlarını ayırt etmek için kullanılır
        /// </summary>
        string GetSessionId();

        /// <summary>
        /// Kullanıcının profil resmi yolu (varsa)
        /// </summary>
        string? GetResim();

        /// <summary>
        /// Kullanıcının oturum açıp açmadığını kontrol eder
        /// </summary>
        bool IsAuthenticated();

        /// <summary>
        /// Kullanıcının tüm bilgilerini içeren özet model döner
        /// </summary>
        UserInfoModel GetUserInfo();
    }

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
    }
}
