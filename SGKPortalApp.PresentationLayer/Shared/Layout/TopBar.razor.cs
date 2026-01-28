using Microsoft.AspNetCore.Components;
using SGKPortalApp.Common.Helpers;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Shared.Layout
{
    public partial class TopBar
    {
        [Inject] private IUserInfoService UserInfo { get; set; } = default!;
        [Inject] private IWebHostEnvironment WebHostEnvironment { get; set; } = default!;
        [Inject] private IConfiguration Configuration { get; set; } = default!;
        [Inject] private PersonelImagePathHelper ImagePathHelper { get; set; } = default!;

        private string UserInitials => GetInitials(UserInfo.GetAdSoyad());
        private string UserFullName => UserInfo.GetAdSoyad();
        private string UserDepartment => UserInfo.GetDepartmanAdi();

        /// <summary>
        /// Kullanıcı resmini dinamik olarak oluşturur.
        /// Claims'teki resim bilgisini kullanarak fiziksel dosya kontrolü yapar.
        /// Config'den path ayarları okunur.
        /// </summary>
        private string? UserImage
        {
            get
            {
                var tcKimlikNo = UserInfo.GetTcKimlikNo();
                var resim = UserInfo.GetResim();

                if (string.IsNullOrEmpty(tcKimlikNo) || string.IsNullOrEmpty(resim))
                    return null;

                // Web path'i helper ile oluştur (config'den BasePath kullanır)
                var webPath = ImagePathHelper.GetWebPath(resim);

                if (string.IsNullOrEmpty(webPath))
                    return null;

                // Fiziksel path: WebRootPath + webPath (örn: wwwroot + /images/avatars/123.jpg)
                var physicalPath = Path.Combine(WebHostEnvironment.WebRootPath, webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(physicalPath))
                {
                    return webPath;
                }

                return null;
            }
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "??";

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();

            // İlk ismin ilk harfi + Soyadın ilk harfi
            return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
        }
    }
}
