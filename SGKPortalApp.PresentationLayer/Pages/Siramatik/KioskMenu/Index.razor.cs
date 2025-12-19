using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskMenu
{
    public partial class Index
    {

        [Inject] private IKioskMenuApiService _kioskMenuService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        private bool isLoading;
        private bool isDeleting;
        private bool showDeleteModal;

        private List<KioskMenuResponseDto> allMenuler = new();
        private List<KioskMenuResponseDto> filteredMenuler = new();
        private KioskMenuResponseDto? selectedMenu;

        private string searchText = string.Empty;
        private Aktiflik? selectedAktiflik;
        private string sortBy = "name-asc";

        protected override async Task OnInitializedAsync()
        {
            await LoadMenusAsync();
        }

        private async Task LoadMenusAsync()
        {
            try
            {
                isLoading = true;
                var result = await _kioskMenuService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    allMenuler = result.Data;
                    ApplyFilters();
                }
                else
                {
                    allMenuler = new List<KioskMenuResponseDto>();
                    filteredMenuler = new List<KioskMenuResponseDto>();
                    await _toastService.ShowWarningAsync(result.Message ?? "Menü bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk menüler yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Menüler yüklenirken hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ApplyFilters()
        {
            IEnumerable<KioskMenuResponseDto> query = allMenuler;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(m => m.MenuAdi.ContainsTurkish(searchText, StringComparison.OrdinalIgnoreCase)
                                       || (m.Aciklama?.ContainsTurkish(searchText, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            if (selectedAktiflik.HasValue)
            {
                query = query.Where(m => m.Aktiflik == selectedAktiflik.Value);
            }

            query = sortBy switch
            {
                "name-asc" => query.OrderBy(m => m.MenuAdi),
                "name-desc" => query.OrderByDescending(m => m.MenuAdi),
                "date-desc" => query.OrderByDescending(m => m.EklenmeTarihi),
                "date-asc" => query.OrderBy(m => m.EklenmeTarihi),
                _ => query.OrderBy(m => m.MenuAdi)
            };

            filteredMenuler = query.ToList();
        }

        private async Task RefreshAsync()
        {
            searchText = string.Empty;
            selectedAktiflik = null;
            sortBy = "name-asc";
            await LoadMenusAsync();
        }

        private void OnSearchChanged()
        {
            ApplyFilters();
        }

        private void OnAktiflikChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                selectedAktiflik = null;
            }
            else if (int.TryParse(e.Value.ToString(), out var val))
            {
                selectedAktiflik = (Aktiflik)val;
            }
            ApplyFilters();
        }

        private void OnSortChanged(ChangeEventArgs e)
        {
            sortBy = e.Value?.ToString() ?? "name-asc";
            ApplyFilters();
        }

        private async Task ToggleAktiflik(KioskMenuResponseDto menu)
        {
            try
            {
                var updateDto = new KioskMenuUpdateRequestDto
                {
                    KioskMenuId = menu.KioskMenuId,
                    MenuAdi = menu.MenuAdi,
                    Aciklama = menu.Aciklama,
                    Aktiflik = menu.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif
                };

                var result = await _kioskMenuService.UpdateAsync(updateDto);
                if (result.Success)
                {
                    menu.Aktiflik = updateDto.Aktiflik;
                    await _toastService.ShowSuccessAsync($"Menü {(menu.Aktiflik == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı");
                    ApplyFilters();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Durum değiştirilemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü aktiflik değiştirilemedi");
                await _toastService.ShowErrorAsync("Durum değiştirilirken hata oluştu");
            }
        }

        private void ShowDeleteConfirm(KioskMenuResponseDto menu)
        {
            selectedMenu = menu;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            selectedMenu = null;
            showDeleteModal = false;
        }

        private async Task DeleteMenuAsync()
        {
            if (selectedMenu == null)
            {
                return;
            }

            try
            {
                isDeleting = true;
                var result = await _kioskMenuService.DeleteAsync(selectedMenu.KioskMenuId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Kiosk menü silindi");
                    allMenuler.Remove(selectedMenu);
                    ApplyFilters();
                    HideDeleteConfirm();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Menü silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk menü silinirken hata oluştu");
                await _toastService.ShowErrorAsync("Menü silinirken hata oluştu");
            }
            finally
            {
                isDeleting = false;
            }
        }
    }
}
