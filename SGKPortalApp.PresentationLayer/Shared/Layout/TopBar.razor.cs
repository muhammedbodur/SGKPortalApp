using Microsoft.AspNetCore.Components;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Shared.Layout
{
    public partial class TopBar
    {
        [Inject] private IUserInfoService UserInfo { get; set; } = default!;

        private string UserInitials => GetInitials(UserInfo.GetAdSoyad());
        private string UserFullName => UserInfo.GetAdSoyad();
        private string UserDepartment => UserInfo.GetDepartmanAdi();
        private string? UserImage => UserInfo.GetResim();

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
