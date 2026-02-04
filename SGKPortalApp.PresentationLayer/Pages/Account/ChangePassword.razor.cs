using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Account;

namespace SGKPortalApp.PresentationLayer.Pages.Account
{
    public partial class ChangePassword
    {
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private ChangePasswordRequestDto Model = new();
        private bool IsSubmitting = false;
        private bool ShowSuccessMessage = false;
        private string? ErrorMessage;
        private bool ShowCurrentPassword = false;
        private bool ShowNewPassword = false;
        private bool ShowConfirmPassword = false;

        // Password strength indicators
        private bool HasMinLength => !string.IsNullOrEmpty(Model.NewPassword) && Model.NewPassword.Length >= 8;
        private bool HasUpperCase => !string.IsNullOrEmpty(Model.NewPassword) && Model.NewPassword.Any(char.IsUpper);
        private bool HasLowerCase => !string.IsNullOrEmpty(Model.NewPassword) && Model.NewPassword.Any(char.IsLower);
        private bool HasDigit => !string.IsNullOrEmpty(Model.NewPassword) && Model.NewPassword.Any(char.IsDigit);
        private bool HasSpecialChar => !string.IsNullOrEmpty(Model.NewPassword) && Model.NewPassword.Any(c => "!@#$%^&*()_+-=[]{}|;':\",./<>?".Contains(c));

        private int PasswordStrength
        {
            get
            {
                int strength = 0;
                if (HasMinLength) strength++;
                if (HasUpperCase) strength++;
                if (HasLowerCase) strength++;
                if (HasDigit) strength++;
                if (HasSpecialChar) strength++;
                return strength;
            }
        }

        private int PasswordStrengthPercent => PasswordStrength * 20;

        private string PasswordStrengthColor => PasswordStrength switch
        {
            0 or 1 => "danger",
            2 => "warning",
            3 => "info",
            4 or 5 => "success",
            _ => "secondary"
        };

        private string PasswordStrengthText => PasswordStrength switch
        {
            0 => "Çok Zayıf",
            1 => "Zayıf",
            2 => "Orta",
            3 => "İyi",
            4 => "Güçlü",
            5 => "Çok Güçlü",
            _ => ""
        };

        private bool IsFormValid =>
            !string.IsNullOrEmpty(Model.CurrentPassword) &&
            !string.IsNullOrEmpty(Model.NewPassword) &&
            !string.IsNullOrEmpty(Model.ConfirmPassword) &&
            Model.NewPassword == Model.ConfirmPassword &&
            HasMinLength && HasUpperCase && HasLowerCase && HasDigit && HasSpecialChar;

        private void CheckPasswordStrength(ChangeEventArgs e)
        {
            Model.NewPassword = e.Value?.ToString() ?? "";
            StateHasChanged();
        }

        private async Task HandleSubmit()
        {
            if (!IsFormValid)
            {
                ErrorMessage = "Lütfen tüm alanları doğru şekilde doldurun.";
                return;
            }

            if (Model.NewPassword != Model.ConfirmPassword)
            {
                ErrorMessage = "Şifreler eşleşmiyor.";
                return;
            }

            IsSubmitting = true;
            ErrorMessage = null;
            ShowSuccessMessage = false;

            try
            {
                // API çağrısı burada yapılacak
                await Task.Delay(1500); // Simülasyon

                // Başarılı
                ShowSuccessMessage = true;
                Model = new ChangePasswordRequestDto();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Bir hata oluştu: {ex.Message}";
            }
            finally
            {
                IsSubmitting = false;
            }
        }
    }
}
