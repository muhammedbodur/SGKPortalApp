using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    [AllowAnonymous]
    public partial class ForgotPassword
    {
        [Inject] private IAuthApiService AuthApiService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private VerifyIdentityRequestDto verifyModel = new() { DogumTarihi = DateTime.Now.AddYears(-30) };
        private bool isLoading = false;
        private string? errorMessage;
        private string? successMessage;

        private async Task HandleVerify()
        {
            try
            {
                isLoading = true;
                errorMessage = null;
                successMessage = null;
                StateHasChanged();

                var response = await AuthApiService.VerifyIdentityAsync(verifyModel);

                if (response == null || !response.Success)
                {
                    errorMessage = response?.Message ?? "Kimlik doğrulama başarısız. Lütfen bilgilerinizi kontrol ediniz.";
                    return;
                }

                // Başarılı doğrulama - Reset Password sayfasına yönlendir
                successMessage = $"Kimlik doğrulandı: {response.AdSoyad}";
                await Task.Delay(1500); // Mesajı göster
                Navigation.NavigateTo($"/auth/reset-password?tc={response.TcKimlikNo}");
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
