using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Departman
{
    public partial class Index : ComponentBase
    {
        private readonly IDepartmanApiService _departmanService;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;

        public Index(
            IDepartmanApiService departmanService,
            NavigationManager navigationManager,
            IJSRuntime jsRuntime)
        {
            _departmanService = departmanService;
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
        }

        private List<DepartmanResponseDto> Departmanlar { get; set; } = new();
        private List<DepartmanResponseDto> FilteredDepartmanlar { get; set; } = new();

        private string searchTerm = string.Empty;
        private bool IsLoading { get; set; } = true;

        // Delete Modal
        private bool ShowDeleteModal { get; set; } = false;
        private int DeleteDepartmanId { get; set; }
        private string DeleteDepartmanAdi { get; set; } = string.Empty;
        private bool IsDeleting { get; set; } = false;

        // Lifecycle Methods
        protected override async Task OnInitializedAsync()
        {
            await LoadDepartmanlar();
        }

        // Data Loading
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
                await ShowToast("Departmanlar yüklenirken bir hata oluştu!", "error");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Search
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
        }

        // Navigation
        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/personel/departman/yeni");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/personel/departman/duzenle/{id}");
        }

        // Toggle Aktiflik
        private async Task ToggleAktiflik(int id)
        {
            try
            {
                var departman = Departmanlar.FirstOrDefault(d => d.DepartmanId == id);
                if (departman == null) return;

                var updateDto = new DepartmanUpdateRequestDto
                {
                    DepartmanAdi = departman.DepartmanAdi,
                    DepartmanAktiflik = departman.DepartmanAktiflik == Aktiflik.Aktif
                        ? Aktiflik.Pasif
                        : Aktiflik.Aktif
                };

                await _departmanService.UpdateAsync(id, updateDto);

                await ShowToast(
                    $"Departman {(updateDto.DepartmanAktiflik == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı.",
                    "success"
                );

                await LoadDepartmanlar();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await ShowToast("Durum değiştirilirken bir hata oluştu!", "error");
            }
        }

        // Delete Operations
        private void ShowDeleteConfirmation(int id, string departmanAdi)
        {
            DeleteDepartmanId = id;
            DeleteDepartmanAdi = departmanAdi;
            ShowDeleteModal = true;
        }

        private void HideDeleteConfirmation()
        {
            ShowDeleteModal = false;
            DeleteDepartmanId = 0;
            DeleteDepartmanAdi = string.Empty;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var success = await _departmanService.DeleteAsync(DeleteDepartmanId);

                if (success)
                {
                    await ShowToast("Departman başarıyla silindi.", "success");
                    await LoadDepartmanlar();
                    HideDeleteConfirmation();
                }
                else
                {
                    await ShowToast("Departman silinemedi!", "error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await ShowToast("Departman silinirken bir hata oluştu!", "error");
            }
            finally
            {
                IsDeleting = false;
            }
        }

        // Toast Notification (Geçici - ToastService eklenince değişecek)
        private async Task ShowToast(string message, string type)
        {
            await _jsRuntime.InvokeVoidAsync("console.log", $"[{type.ToUpper()}] {message}");
            // TODO: ToastService.Show(message, type) şeklinde değişecek
        }
    }
}