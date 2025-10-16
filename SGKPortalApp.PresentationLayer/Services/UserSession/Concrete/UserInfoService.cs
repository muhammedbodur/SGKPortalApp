using SGKPortalApp.PresentationLayer.Services.UserSession.Interfaces;
using System.Security.Claims;

namespace SGKPortalApp.PresentationLayer.Services.UserSession.Concrete
{
    /// <summary>
    /// Oturum açmış kullanıcının bilgilerine erişim sağlar.
    /// HttpContext.User.Claims üzerinden güvenli bir şekilde bilgileri okur.
    /// Scoped service olarak kayıtlıdır - her HTTP request için yeni instance oluşturulur.
    /// </summary>
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Güvenli bir şekilde claim değerini okur
        /// </summary>
        private string GetClaimValue(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType) ?? string.Empty;
        }

        /// <summary>
        /// Güvenli bir şekilde int claim değerini okur
        /// </summary>
        private int GetClaimValueAsInt(string claimType)
        {
            var value = GetClaimValue(claimType);
            return int.TryParse(value, out int result) ? result : 0;
        }

        public string GetTcKimlikNo()
        {
            return GetClaimValue("TcKimlikNo");
        }

        public int GetSicilNo()
        {
            return GetClaimValueAsInt("SicilNo");
        }

        public string GetAdSoyad()
        {
            return GetClaimValue("AdSoyad");
        }

        public string GetEmail()
        {
            // ClaimTypes.Email standart claim type'dır
            return GetClaimValue(ClaimTypes.Email);
        }

        public int GetDepartmanId()
        {
            return GetClaimValueAsInt("DepartmanId");
        }

        public string GetDepartmanAdi()
        {
            return GetClaimValue("DepartmanAdi");
        }

        public int GetServisId()
        {
            return GetClaimValueAsInt("ServisId");
        }

        public string GetServisAdi()
        {
            return GetClaimValue("ServisAdi");
        }

        public int GetHizmetBinasiId()
        {
            return GetClaimValueAsInt("HizmetBinasiId");
        }

        public string GetHizmetBinasiAdi()
        {
            return GetClaimValue("HizmetBinasiAdi");
        }

        public string GetSessionId()
        {
            return GetClaimValue("SessionID");
        }

        public string? GetResim()
        {
            var resim = GetClaimValue("Resim");
            return string.IsNullOrEmpty(resim) ? null : resim;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public UserInfoModel GetUserInfo()
        {
            return new UserInfoModel
            {
                TcKimlikNo = GetTcKimlikNo(),
                SicilNo = GetSicilNo(),
                AdSoyad = GetAdSoyad(),
                Email = GetEmail(),
                DepartmanId = GetDepartmanId(),
                DepartmanAdi = GetDepartmanAdi(),
                ServisId = GetServisId(),
                ServisAdi = GetServisAdi(),
                HizmetBinasiId = GetHizmetBinasiId(),
                HizmetBinasiAdi = GetHizmetBinasiAdi(),
                SessionId = GetSessionId(),
                Resim = GetResim()
            };
        }
    }
}
