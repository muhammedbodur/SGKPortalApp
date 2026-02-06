using Microsoft.AspNetCore.Components;

namespace SGKPortalApp.PresentationLayer.Pages.Account
{
    public partial class ForgotPassword
    {
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private Step CurrentStep = Step.VerifyIdentity;
        private bool IsSubmitting = false;
        private string? ErrorMessage;
        private bool ShowNewPassword = false;
        private bool ShowConfirmPassword = false;

        private IdentityVerificationModel IdentityModel = new();

        private SecurityQuestionModel SecurityModel = new();
        private string SecurityQuestion = "İlk evcil hayvanınızın adı nedir?";
        private string MaskedUserName = "M***** K*****";
        private string MaskedDepartment = "S***** H******* Daire Başkanlığı";

        private NewPasswordModel PasswordModel = new();

        private bool HasMinLength => !string.IsNullOrEmpty(PasswordModel.NewPassword) && PasswordModel.NewPassword.Length >= 8;
        private bool HasUpperCase => !string.IsNullOrEmpty(PasswordModel.NewPassword) && PasswordModel.NewPassword.Any(char.IsUpper);
        private bool HasLowerCase => !string.IsNullOrEmpty(PasswordModel.NewPassword) && PasswordModel.NewPassword.Any(char.IsLower);
        private bool HasDigit => !string.IsNullOrEmpty(PasswordModel.NewPassword) && PasswordModel.NewPassword.Any(char.IsDigit);

        private bool IsPasswordValid =>
            HasMinLength && HasUpperCase && HasLowerCase && HasDigit &&
            PasswordModel.NewPassword == PasswordModel.ConfirmPassword;

        private int PasswordStrength
        {
            get
            {
                int strength = 0;
                if (HasMinLength) strength++;
                if (HasUpperCase) strength++;
                if (HasLowerCase) strength++;
                if (HasDigit) strength++;
                return strength;
            }
        }

        private int PasswordStrengthPercent => PasswordStrength * 25;

        private string PasswordStrengthColor => PasswordStrength switch
        {
            0 or 1 => "danger",
            2 => "warning",
            3 => "info",
            4 => "success",
            _ => "secondary"
        };

        private string PasswordStrengthText => PasswordStrength switch
        {
            0 => "Çok Zayıf",
            1 => "Zayıf",
            2 => "Orta",
            3 => "İyi",
            4 => "Güçlü",
            _ => ""
        };

        private async Task VerifyIdentity()
        {
            if (string.IsNullOrWhiteSpace(IdentityModel.TcKimlikNo) || IdentityModel.TcKimlikNo.Length != 11)
            {
                ErrorMessage = "Geçerli bir T.C. Kimlik Numarası giriniz.";
                return;
            }

            if (string.IsNullOrWhiteSpace(IdentityModel.SicilNo))
            {
                ErrorMessage = "Sicil numaranızı giriniz.";
                return;
            }

            if (IdentityModel.DogumTarihi == default)
            {
                ErrorMessage = "Doğum tarihinizi seçiniz.";
                return;
            }

            if (string.IsNullOrWhiteSpace(IdentityModel.TelefonSon4Hane) || IdentityModel.TelefonSon4Hane.Length != 4)
            {
                ErrorMessage = "Telefon numaranızın son 4 hanesini giriniz.";
                return;
            }

            IsSubmitting = true;
            ErrorMessage = null;

            try
            {
                await Task.Delay(1500);

                MaskedUserName = "M***** K*****";
                MaskedDepartment = "S***** H******* Daire Başkanlığı";

                CurrentStep = Step.SecurityQuestion;
            }
            catch
            {
                ErrorMessage = "Kimlik doğrulama başarısız. Bilgilerinizi kontrol ediniz.";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private async Task VerifySecurityAnswer()
        {
            if (string.IsNullOrWhiteSpace(SecurityModel.Answer))
            {
                ErrorMessage = "Güvenlik sorusunu yanıtlayınız.";
                return;
            }

            if (string.IsNullOrWhiteSpace(SecurityModel.AnneKizlikSoyadi) || SecurityModel.AnneKizlikSoyadi.Length < 3)
            {
                ErrorMessage = "Anne kızlık soyadının ilk 3 harfini giriniz.";
                return;
            }

            IsSubmitting = true;
            ErrorMessage = null;

            try
            {
                await Task.Delay(1000);
                CurrentStep = Step.NewPassword;
            }
            catch
            {
                ErrorMessage = "Güvenlik yanıtları doğrulanamadı.";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private async Task SetNewPassword()
        {
            if (!IsPasswordValid)
            {
                ErrorMessage = "Şifre gereksinimlerini karşılamıyor.";
                return;
            }

            IsSubmitting = true;
            ErrorMessage = null;

            try
            {
                await Task.Delay(1500);
                CurrentStep = Step.Success;
            }
            catch
            {
                ErrorMessage = "Şifre kaydedilemedi. Lütfen tekrar deneyiniz.";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private void GoBack()
        {
            ErrorMessage = null;
            if (CurrentStep == Step.SecurityQuestion)
            {
                CurrentStep = Step.VerifyIdentity;
            }
            else if (CurrentStep == Step.NewPassword)
            {
                CurrentStep = Step.SecurityQuestion;
            }
        }

        private enum Step
        {
            VerifyIdentity,
            SecurityQuestion,
            NewPassword,
            Success
        }

        private class IdentityVerificationModel
        {
            public string? TcKimlikNo { get; set; }
            public string? SicilNo { get; set; }
            public DateTime DogumTarihi { get; set; }
            public string? TelefonSon4Hane { get; set; }
        }

        private class SecurityQuestionModel
        {
            public string? Answer { get; set; }
            public string? AnneKizlikSoyadi { get; set; }
        }

        private class NewPasswordModel
        {
            public string? NewPassword { get; set; }
            public string? ConfirmPassword { get; set; }
        }
    }
}
