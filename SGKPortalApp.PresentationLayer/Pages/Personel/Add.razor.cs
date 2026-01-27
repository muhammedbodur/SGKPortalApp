using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Personel;

public partial class Add
{
    [Inject] private IPersonelApiService PersonelApiService { get; set; } = default!;
    [Inject] private IToastService ToastService { get; set; } = default!;
    [Inject] private NavigationManager _navigationManager { get; set; } = default!;

    private AddPersonelFormData FormData { get; set; } = new();
    private bool IsLoading { get; set; }
    private string? ErrorMessage { get; set; }

    protected override string PagePermissionKey => "personel";

    private async Task HandleSubmit()
    {
        ErrorMessage = null;
        IsLoading = true;

        try
        {
            // TC Kimlik No validasyonu
            if (string.IsNullOrWhiteSpace(FormData.TcKimlikNo) || FormData.TcKimlikNo.Length != 11)
            {
                ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.";
                return;
            }

            if (!FormData.TcKimlikNo.All(char.IsDigit))
            {
                ErrorMessage = "TC Kimlik No sadece rakamlardan oluşmalıdır.";
                return;
            }

            // Ad Soyad validasyonu
            if (string.IsNullOrWhiteSpace(FormData.AdSoyad) || FormData.AdSoyad.Length < 3)
            {
                ErrorMessage = "Ad Soyad en az 3 karakter olmalıdır.";
                return;
            }

            // TC Kimlik No daha önce kullanılmış mı kontrol et
            var existingPersonel = await PersonelApiService.GetByTcKimlikNoAsync(FormData.TcKimlikNo);
            if (existingPersonel.Success && existingPersonel.Data != null)
            {
                ErrorMessage = $"Bu TC Kimlik No ({FormData.TcKimlikNo}) zaten kayıtlı. Mevcut personeli düzenlemek için Personel Listesini kullanın.";
                return;
            }

            // Manage sayfasına yönlendir (create mode ile)
            var encodedAdSoyad = Uri.EscapeDataString(FormData.AdSoyad.Trim());
            _navigationManager.NavigateTo($"/personel/create?tc={FormData.TcKimlikNo}&adsoyad={encodedAdSoyad}");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Bir hata oluştu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void NavigateToList()
    {
        _navigationManager.NavigateTo("/personel");
    }

    public class AddPersonelFormData
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Ad Soyad 3-200 karakter arasında olmalıdır")]
        public string AdSoyad { get; set; } = string.Empty;
    }
}
