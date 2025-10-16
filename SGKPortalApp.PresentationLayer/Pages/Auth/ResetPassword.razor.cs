using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    [AllowAnonymous]
    public partial class ResetPassword
    {
        [Inject] private IAuthApiService AuthApiService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "tc")]
        public string? TcKimlikNo { get; set; }

        private ResetPasswordRequestDto resetModel = new();
        private bool showNewPassword = false;
        private bool showConfirmPassword = false;
        private bool isLoading = false;
        private string? errorMessage;
        private string? successMessage;

        protected override void OnInitialized()
        {
            if (string.IsNullOrEmpty(TcKimlikNo))
            {
                // TC Kimlik No yoksa forgot password sayfasına yönlendir
                Navigation.NavigateTo("/auth/forgot-password");
                return;
            }

            resetModel.TcKimlikNo = TcKimlikNo;
        }

        private async Task HandleResetPassword()
        {
            try
            {
                isLoading = true;
                errorMessage = null;
                successMessage = null;
                StateHasChanged();

                var success = await AuthApiService.ResetPasswordAsync(resetModel);

                if (!success)
                {
                    errorMessage = "Şifre sıfırlama başarısız. Lütfen tekrar deneyin.";
                    return;
                }

                // Başarılı - Login sayfasına yönlendir
                successMessage = "Şifreniz başarıyla değiştirildi! Giriş sayfasına yönlendiriliyorsunuz...";
                StateHasChanged();
                await Task.Delay(2000);
                Navigation.NavigateTo("/auth/login");
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
