using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.DepartmanHizmetBinasi
{
    public partial class Index
    {
        [Inject] private IDepartmanHizmetBinasiApiService _service { get; set; } = default!;
        [Inject] private IDepartmanApiService _departmanService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        private List<DepartmanHizmetBinasiResponseDto> assignedBinalar = new();
        private List<DepartmanResponseDto> departmanlar = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();

        private int selectedDepartmanId = 0;
        private int draggedBinaId = 0;

        private bool isLoading = true;
        private bool isLoadingBinalar = false;
        private bool isSaving = false;
        private bool showDeleteModal = false;
        private bool isDeleting = false;
        private DepartmanHizmetBinasiResponseDto? selectedItem = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;

            try
            {
                // Departmanları yükle
                var departmanResult = await _departmanService.GetAllAsync();
                if (departmanResult.Success && departmanResult.Data != null)
                {
                    departmanlar = departmanResult.Data.OrderBy(d => d.DepartmanAdi).ToList();
                }

                // Hizmet binalarını yükle (sadece aktif olanlar)
                var binaResult = await _hizmetBinasiService.GetActiveAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data.OrderBy(b => b.HizmetBinasiAdi).ToList();
                }

                // Eşleştirmeleri yükle
                var result = await _service.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    assignedBinalar = result.Data
                        .OrderBy(e => e.DepartmanAdi)
                        .ThenBy(e => e.HizmetBinasiAdi)
                        .ToList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Eşleştirmeler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veri yüklenirken hata oluştu: {ex.Message}");
                _logger.LogError(ex, "LoadDataAsync hatası");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void SelectDepartman(int departmanId)
        {
            selectedDepartmanId = departmanId;
        }

        private void OnDragStart(int binaId)
        {
            draggedBinaId = binaId;
        }

        private async Task OnDropToAssigned()
        {
            if (draggedBinaId == 0 || selectedDepartmanId == 0) return;

            await AddBina(draggedBinaId);
            draggedBinaId = 0;
        }

        private async Task AddBina(int binaId)
        {
            if (selectedDepartmanId == 0) return;

            isSaving = true;

            try
            {
                var createDto = new DepartmanHizmetBinasiCreateRequestDto
                {
                    DepartmanId = selectedDepartmanId,
                    HizmetBinasiId = binaId,
                    AnaBina = false
                };

                var result = await _service.CreateAsync(createDto);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Eşleştirme başarıyla eklendi");
                    await LoadDataAsync();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Eşleştirme eklenemedi");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
                _logger.LogError(ex, "AddBina hatası");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task RemoveBina(int departmanHizmetBinasiId)
        {
            isSaving = true;

            try
            {
                var result = await _service.DeleteAsync(departmanHizmetBinasiId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Eşleştirme kaldırıldı");
                    await LoadDataAsync();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Eşleştirme kaldırılamadı");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
                _logger.LogError(ex, "RemoveBina hatası");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task RefreshAsync()
        {
            await LoadDataAsync();
            await _toastService.ShowSuccessAsync("Liste yenilendi");
        }

        private async Task ToggleAnaBina(DepartmanHizmetBinasiResponseDto item)
        {
            isSaving = true;

            try
            {
                var updateDto = new DepartmanHizmetBinasiUpdateRequestDto
                {
                    AnaBina = !item.AnaBina
                };

                var result = await _service.UpdateAsync(item.DepartmanHizmetBinasiId, updateDto);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"{(updateDto.AnaBina ? "Ana Bina" : "Şube")} olarak işaretlendi");
                    await LoadDataAsync();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İşlem başarısız");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
                _logger.LogError(ex, "ToggleAnaBina hatası");
            }
            finally
            {
                isSaving = false;
            }
        }

        private void ShowDeleteConfirm(DepartmanHizmetBinasiResponseDto item)
        {
            selectedItem = item;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            showDeleteModal = false;
            selectedItem = null;
        }

        private async Task DeleteItem()
        {
            if (selectedItem == null) return;

            isDeleting = true;

            try
            {
                var result = await _service.DeleteAsync(selectedItem.DepartmanHizmetBinasiId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Eşleştirme başarıyla silindi");
                    HideDeleteConfirm();
                    await LoadDataAsync();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Silme işlemi başarısız");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
                _logger.LogError(ex, "DeleteItem hatası");
            }
            finally
            {
                isDeleting = false;
            }
        }
    }
}
