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
        /// Claims'teki eski resim yolu yerine, TC Kimlik No ile güncel resim yolunu kontrol eder.
        /// </summary>
        private string? UserImage
        {
            get
            {
                var tcKimlikNo = UserInfo.GetTcKimlikNo();
                if (string.IsNullOrEmpty(tcKimlikNo))
                    return null;

                // Resim dosyası var mı kontrol et
                var fileName = $"{tcKimlikNo}.jpg";
                var imagePath = Path.Combine(WebHostEnvironment.WebRootPath, "images", "avatars", fileName);
                
                if (File.Exists(imagePath))
                {
                    return $"/images/avatars/{fileName}";
                }

                // Yoksa Claims'teki değeri kullan (eski davranış)
                return UserInfo.GetResim();
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
