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

        private LoginRequestDto loginModel = new();
        private bool showPassword = false;
        private bool isLoading = false;
        private string? errorMessage;

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

                await JS.InvokeVoidAsync("console.log", "🔵 6. LoginHandler'a POST yapılıyor...");

                // HttpClient kullanarak LoginHandler'a POST yap
                var httpClient = new HttpClient { BaseAddress = new Uri(Navigation.BaseUri) };
                var loginHandlerResponse = await httpClient.PostAsJsonAsync("/auth/loginhandler", response);

                if (loginHandlerResponse.IsSuccessStatusCode)
                {
                    await JS.InvokeVoidAsync("console.log", "🔵 7. Cookie başarıyla oluşturuldu!");

                    // Sayfayı tamamen yenile - window.location kullan
                    await JS.InvokeVoidAsync("console.log", "🔵 8. Ana sayfaya yönlendiriliyor...");

                    // ✅ FİKS: window.location.href ile tam browser refresh
                    // Bu şekilde:
                    // 1. Browser yeni HTTP request yapar
                    // 2. Cookie'ler yeniden gönderilir
                    // 3. ServerAuthenticationStateProvider'a erişilerek yeni user döndürülür
                    // 4. AuthorizeRouteView yeni state'i görür
                    await JS.InvokeVoidAsync("eval", "window.location.href = '/'");
                }
                else
                {
                    var errorContent = await loginHandlerResponse.Content.ReadAsStringAsync();
                    await JS.InvokeVoidAsync("console.error", $"❌ LoginHandler hatası: Status={loginHandlerResponse.StatusCode}, Content={errorContent}");
                    errorMessage = $"Oturum oluşturulamadı (Status: {loginHandlerResponse.StatusCode}). Lütfen tekrar deneyin.";
                }
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