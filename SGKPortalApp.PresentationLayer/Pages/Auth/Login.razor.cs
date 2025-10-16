using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    [AllowAnonymous]
    public partial class Login
    {
        [Inject] private IAuthApiService AuthApiService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private HttpClient HttpClient { get; set; } = default!;

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
                await JS.InvokeVoidAsync("console.log", " 1. HandleLogin başladı");
                await JS.InvokeVoidAsync("console.log", $" 2. TC: {loginModel.TcKimlikNo}, Şifre uzunluğu: {loginModel.Password?.Length}");
                
                await JS.InvokeVoidAsync("console.log", " 3. API çağrısı yapılıyor...");
                var response = await AuthApiService.LoginAsync(loginModel);
                await JS.InvokeVoidAsync("console.log", $" 4. API response alındı: {response?.Success}");

                if (response == null || !response.Success)
                {
                    errorMessage = response?.Message ?? "Giriş başarısız. Lütfen bilgilerinizi kontrol ediniz.";
                    return;
                }

                // Başarılı giriş - Cookie oluştur ve ana sayfaya yönlendir
                await JS.InvokeVoidAsync("console.log", $"🔵 5. Login başarılı: {response.AdSoyad}");
                
                // LoginHandler'a POST yaparak Cookie oluştur
                await JS.InvokeVoidAsync("console.log", "🔵 6. LoginHandler'a POST yapılıyor...");
                var loginHandlerUrl = Navigation.ToAbsoluteUri("/auth/loginhandler").ToString();
                var cookieResponse = await HttpClient.PostAsJsonAsync(loginHandlerUrl, response);
                
                if (cookieResponse.IsSuccessStatusCode)
                {
                    await JS.InvokeVoidAsync("console.log", "🔵 7. Cookie oluşturuldu, yönlendiriliyor...");
                    // Ana sayfaya yönlendir (forceLoad ile tam sayfa yenileme)
                    Navigation.NavigateTo("/", forceLoad: true);
                }
                else
                {
                    var errorContent = await cookieResponse.Content.ReadAsStringAsync();
                    await JS.InvokeVoidAsync("console.error", $"❌ LoginHandler hatası: Status={cookieResponse.StatusCode}, Content={errorContent}");
                    errorMessage = $"Oturum oluşturulamadı: {cookieResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Bir hata oluştu. Lütfen tekrar deneyin.";
                await JS.InvokeVoidAsync("console.error", ex.Message);
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
    }
}
