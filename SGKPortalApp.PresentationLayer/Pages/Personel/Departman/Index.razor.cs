using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices;
using SGKPortalApp.PresentationLayer.Services.UIServices;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Departman
{
    public partial class Index : ComponentBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IDepartmanApiService _departmanService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        // Properties
        private List<DepartmanResponseDto> Departmanlar { get; set; } = new();
        private List<DepartmanResponseDto> FilteredDepartmanlar { get; set; } = new();
        private string searchTerm = string.Empty;
        private bool IsLoading { get; set; } = true;

        // Delete Modal
        private bool ShowDeleteModal { get; set; } = false;
        private int DeleteDepartmanId { get; set; }
        private string DeleteDepartmanAdi { get; set; } = string.Empty;
        private bool IsDeleting { get; set; } = false;

        // Lifecycle
        protected override async Task OnInitializedAsync()
        {
            await LoadDepartmanlar();
        }

        private async Task LoadDepartmanlar()
        {
            IsLoading = true;
            try
            {
                Departmanlar = await _departmanService.GetAllAsync();
                FilteredDepartmanlar = Departmanlar;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Departmanlar yüklenirken bir hata oluştu!");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnSearchChanged()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                FilteredDepartmanlar = Departmanlar;
            }
            else
            {
                FilteredDepartmanlar = Departmanlar
                    .Where(d => d.DepartmanAdi.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            StateHasChanged();
        }

        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/personel/departman/manage");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/personel/departman/manage/{id}");
        }

        private async Task ToggleAktiflik(int id)
        {
            try
            {
                var departman = Departmanlar.FirstOrDefault(d => d.DepartmanId == id);
                if (departman == null)
                {
                    await _toastService.ShowErrorAsync("Departman bulunamadı!");
                    return;
                }

                var yeniDurum = departman.DepartmanAktiflik == Aktiflik.Aktif
                    ? Aktiflik.Pasif
                    : Aktiflik.Aktif;

                var updateDto = new DepartmanUpdateRequestDto
                {
                    DepartmanAdi = departman.DepartmanAdi,
                    DepartmanAktiflik = yeniDurum
                };

                await _departmanService.UpdateAsync(id, updateDto);

                await _toastService.ShowSuccessAsync(
                    $"Departman {(yeniDurum == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı."
                );

                await LoadDepartmanlar();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Durum değiştirilirken bir hata oluştu!");
            }
        }

        private void ShowDeleteConfirmation(int id, string departmanAdi)
        {
            DeleteDepartmanId = id;
            DeleteDepartmanAdi = departmanAdi;
            ShowDeleteModal = true;
            StateHasChanged();
        }

        private void HideDeleteConfirmation()
        {
            ShowDeleteModal = false;
            DeleteDepartmanId = 0;
            DeleteDepartmanAdi = string.Empty;
            StateHasChanged();
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var success = await _departmanService.DeleteAsync(DeleteDepartmanId);

                if (success)
                {
                    await _toastService.ShowSuccessAsync("Departman başarıyla silindi.");
                    await LoadDepartmanlar();
                    HideDeleteConfirmation();
                }
                else
                {
                    await _toastService.ShowErrorAsync("Departman silinemedi!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Departman silinirken bir hata oluştu!");
            }
            finally
            {
                IsDeleting = false;
            }
        }
    }
}