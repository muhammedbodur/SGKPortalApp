using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth;
using SGKPortalApp.PresentationLayer.Services.AuthenticationServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    [AllowAnonymous]
    public partial class Login
    {
        [Inject] private IAuthApiService AuthApiService { get; set; } = default!;
        [Inject] private ICookieAuthService CookieAuthService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery(Name = "sessionExpired")]
        public bool SessionExpired { get; set; }

        private LoginRequestDto loginModel = new();
        private bool showPassword = false;
        private bool isLoading = false;
        private string? errorMessage;

        protected override void OnParametersSet()
        {
            if (SessionExpired)
            {
                errorMessage = "⚠️ Oturumunuz sonlandırıldı. Hesabınıza başka bir cihaz veya tarayıcıdan giriş yapıldı.";
            }
        }

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }

        private async Task HandleLogin()
        {
            errorMessage = null;
            isLoading = true;

            try
            {
                await JS.InvokeVoidAsync("console.log", "🔵 1. HandleLogin başladı");
                await JS.InvokeVoidAsync("console.log", $"🔵 2. TC: {loginModel.TcKimlikNo}, Şifre uzunluğu: {loginModel.Password?.Length}");

                await JS.InvokeVoidAsync("console.log", "🔵 3. API çağrısı yapılıyor...");
                var response = await AuthApiService.LoginAsync(loginModel);
                await JS.InvokeVoidAsync("console.log", $"🔵 4. API response alındı: {response?.Success}");

                if (response == null || !response.Success)
                {
                    errorMessage = response?.Message ?? "Giriş başarısız. Lütfen bilgilerinizi kontrol ediniz.";
                    return;
                }

                // Başarılı giriş - LoginHandler Razor Page'e POST yap
                await JS.InvokeVoidAsync("console.log", $"🔵 5. Login başarılı: {response.AdSoyad}");

                await JS.InvokeVoidAsync("console.log", "🔵 6. LoginHandler'a form submit yapılıyor...");

                // ✅ FİKS: JavaScript ile form submit kullan
                // Bu sayede browser cookie'leri otomatik olarak alır ve gönderir
                var loginDataJson = System.Text.Json.JsonSerializer.Serialize(response);
                await JS.InvokeVoidAsync("submitLoginForm", loginDataJson);
            }
            catch (Exception ex)
            {
                errorMessage = "Bir hata oluştu. Lütfen tekrar deneyin.";
                await JS.InvokeVoidAsync("console.error", $"❌ Hata: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
    }
}