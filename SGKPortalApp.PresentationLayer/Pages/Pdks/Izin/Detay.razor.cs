using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class Detay
    {
        [Parameter] public int Id { get; set; }

        [Inject] private IIzinMazeretTalepApiService _izinMazeretTalepService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IJSRuntime _js { get; set; } = default!;

        private IzinMazeretTalepResponseDto? Talep { get; set; }
        private bool IsLoading { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadTalep();
        }

        private async Task LoadTalep()
        {
            try
            {
                IsLoading = true;

                var result = await _izinMazeretTalepService.GetByIdAsync(Id);

                if (result.Success && result.Data != null)
                {
                    Talep = result.Data;
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talep bulunamadı");
                }
            }
            catch (Exception ex)
            {
                await ShowToast("error", $"Talep yüklenirken hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void GoBack()
        {
            _navigationManager.NavigateTo("/pdks/izin/taleplerim");
        }

        private string GetOnayBadgeClass(string onayDurumu)
        {
            return onayDurumu switch
            {
                "Beklemede" => "bg-label-warning",
                "Onaylandı" => "bg-label-success",
                "Reddedildi" => "bg-label-danger",
                "İptal Edildi" => "bg-label-secondary",
                _ => "bg-label-secondary"
            };
        }

        private string GetDurumBadgeClass(string durum)
        {
            return durum switch
            {
                "Onaylandı" => "bg-label-success",
                "Reddedildi" => "bg-label-danger",
                "İptal" => "bg-label-secondary",
                _ => "bg-label-warning"
            };
        }

        private async Task ShowToast(string type, string message)
        {
            try
            {
                await _js.InvokeVoidAsync("showToast", type, message);
            }
            catch
            {
                // Toast gösterilemezse sessizce devam et
            }
        }
    }
}
