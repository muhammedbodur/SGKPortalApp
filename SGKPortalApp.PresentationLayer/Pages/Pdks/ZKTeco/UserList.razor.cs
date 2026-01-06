using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class UserList
    {
        [Inject] private IPersonelApiService PersonelApiService { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        private List<PersonelResponseDto> Personeller { get; set; } = new();
        private bool IsLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadPersoneller();
        }

        private async Task LoadPersoneller()
        {
            IsLoading = true;
            try
            {
                var result = await PersonelApiService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    Personeller = result.Data;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Personeller yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
