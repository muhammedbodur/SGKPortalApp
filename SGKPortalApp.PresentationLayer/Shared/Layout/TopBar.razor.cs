using Microsoft.AspNetCore.Components;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Shared.Layout
{
    public partial class TopBar
    {
        [Inject] private IUserInfoService UserInfo { get; set; } = default!;
        [Inject] private IWebHostEnvironment WebHostEnvironment { get; set; } = default!;

        private string UserInitials => GetInitials(UserInfo.GetAdSoyad());
        private string UserFullName => UserInfo.GetAdSoyad();
        private string UserDepartment => UserInfo.GetDepartmanAdi();
        
        /// <summary>
        /// Kullanıcı resmini dinamik olarak oluşturur.
        /// Claims'teki resim bilgisini kullanarak fiziksel dosya kontrolü yapar.
        /// </summary>
        private string? UserImage
        {
            get
            {
                var tcKimlikNo = UserInfo.GetTcKimlikNo();
                var resim = UserInfo.GetResim();

                if (string.IsNullOrEmpty(tcKimlikNo) || string.IsNullOrEmpty(resim))
                    return null;

                // Fiziksel path: wwwroot/images/avatars/{resim}.jpg
                var physicalPath = Path.Combine(WebHostEnvironment.WebRootPath, "images", "avatars", $"{resim}.jpg");

                // URL path: /images/avatars/{resim}.jpg
                var urlPath = $"/images/avatars/{resim}.jpg";

                if (File.Exists(physicalPath))
                {
                    return urlPath;
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
