using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Account;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Account;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Pages.Account
{
    public partial class Profile
    {
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private IUserApiService UserApiService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private string UserName = "";
        private string UserFullName = "";
        private string UserEmail = "";
        private string UserPhone = "";
        private string UserDepartment = "";
        private string UserRole = "Kullanıcı";
        private string ProfileImage = "";
        private string RegistrationDate = "";
        private bool TwoFactorEnabled = false;
        private bool EmailNotifications = true;
        private bool SmsNotifications = false;
        private bool AnnouncementNotifications = true;
        private bool CalendarReminders = true;
        private bool IsSaving = false;
        private bool ShowSuccessMessage = false;

        private UpdateProfileRequestDto ProfileModel = new();
        private List<SessionInfoDto> RecentSessions = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadUserProfile();
            LoadRecentSessions();
        }

        private async Task LoadUserProfile()
        {
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity?.IsAuthenticated == true)
                {
                    UserName = user.Identity.Name ?? "kullanici";
                    UserFullName = user.FindFirst("name")?.Value ?? user.Identity.Name ?? "Kullanıcı";
                    UserEmail = user.FindFirst("email")?.Value ?? "email@sgk.gov.tr";
                    UserPhone = user.FindFirst("phone")?.Value ?? "-";
                    UserDepartment = user.FindFirst("department")?.Value ?? "İzmir SGK";
                    UserRole = user.FindFirst("role")?.Value ?? "Kullanıcı";
                    ProfileImage = user.FindFirst("picture")?.Value ?? "";
                    RegistrationDate = DateTime.Now.AddYears(-2).ToString("dd.MM.yyyy");

                    // Model'i doldur
                    var nameParts = UserFullName.Split(' ');
                    ProfileModel.FirstName = nameParts.Length > 0 ? nameParts[0] : "";
                    ProfileModel.LastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
                    ProfileModel.Email = UserEmail;
                    ProfileModel.Phone = UserPhone;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profil yükleme hatası: {ex.Message}");
            }
        }

        private void LoadRecentSessions()
        {
            RecentSessions = new List<SessionInfoDto>
            {
                new SessionInfoDto { Browser = "Chrome", BrowserIcon = "bxl-chrome", Device = "Windows 11", Location = "İzmir, TR", Date = DateTime.Now, IsActive = true },
                new SessionInfoDto { Browser = "Firefox", BrowserIcon = "bxl-firefox", Device = "MacOS", Location = "İzmir, TR", Date = DateTime.Now.AddDays(-1), IsActive = false },
                new SessionInfoDto { Browser = "Safari", BrowserIcon = "bxl-apple", Device = "iPhone 14", Location = "İzmir, TR", Date = DateTime.Now.AddDays(-3), IsActive = false },
            };
        }

        private async Task SaveProfile()
        {
            IsSaving = true;
            try
            {
                // API çağrısı yapılacak
                await Task.Delay(1000); // Simülasyon
                ShowSuccessMessage = true;
                UserFullName = $"{ProfileModel.FirstName} {ProfileModel.LastName}";
                UserEmail = ProfileModel.Email ?? "";
                UserPhone = ProfileModel.Phone ?? "";
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async Task ResetForm()
        {
            await LoadUserProfile();
        }

        private async Task OpenImageUpload()
        {
            await JSRuntime.InvokeVoidAsync("alert", "Fotoğraf yükleme özelliği yakında eklenecek.");
        }
    }
}
