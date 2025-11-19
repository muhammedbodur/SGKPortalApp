using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskMenuIslem
{
    public partial class Index
    {
        [Inject] private IKioskMenuApiService _kioskMenuService { get; set; } = default!;
        [Inject] private IKioskMenuIslemApiService _kioskMenuIslemService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        private bool isLoading;
        private bool isDeleting;
        private bool showDeleteModal;

        private List<KioskMenuResponseDto> kioskMenuleri = new();
        private List<KioskMenuIslemResponseDto> allIslemler = new();
        private List<KioskMenuIslemResponseDto> filteredIslemler = new();

        private int selectedKioskMenuId;
        private string selectedMenuAdi = string.Empty;
        private KioskMenuIslemResponseDto? selectedIslem;

        protected override async Task OnInitializedAsync()
        {
            await LoadKioskMenuleriAsync();
        }

        private async Task LoadKioskMenuleriAsync()
        {
            try
            {
                isLoading = true;
                var result = await _kioskMenuService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    kioskMenuleri = result.Data;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Menüler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menüler yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Menüler yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OnKioskMenuChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int menuId))
            {
                selectedKioskMenuId = menuId;

                if (menuId > 0)
                {
                    var selectedMenu = kioskMenuleri.FirstOrDefault(m => m.KioskMenuId == menuId);
                    selectedMenuAdi = selectedMenu?.MenuAdi ?? string.Empty;
                    await LoadIslemlerAsync(menuId);
                }
                else
                {
                    selectedMenuAdi = string.Empty;
                    filteredIslemler.Clear();
                }
            }
        }

        private async Task LoadIslemlerAsync(int kioskMenuId)
        {
            try
            {
                isLoading = true;
                var result = await _kioskMenuIslemService.GetByKioskMenuAsync(kioskMenuId);

                if (result.Success && result.Data != null)
                {
                    allIslemler = result.Data;
                    filteredIslemler = allIslemler;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İşlemler yüklenemedi");
                    filteredIslemler.Clear();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlemler yüklenirken hata oluştu. KioskMenuId: {KioskMenuId}", kioskMenuId);
                await _toastService.ShowErrorAsync("İşlemler yüklenirken bir hata oluştu");
                filteredIslemler.Clear();
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ShowDeleteConfirm(KioskMenuIslemResponseDto islem)
        {
            selectedIslem = islem;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            showDeleteModal = false;
            selectedIslem = null;
        }

        private async Task DeleteIslem()
        {
            if (selectedIslem == null) return;

            try
            {
                isDeleting = true;
                var result = await _kioskMenuIslemService.DeleteAsync(selectedIslem.KioskMenuIslemId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("İşlem başarıyla silindi");
                    await LoadIslemlerAsync(selectedKioskMenuId);
                    HideDeleteConfirm();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İşlem silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem silinirken hata oluştu. Id: {Id}", selectedIslem.KioskMenuIslemId);
                await _toastService.ShowErrorAsync("İşlem silinirken bir hata oluştu");
            }
            finally
            {
                isDeleting = false;
            }
        }
    }
}
