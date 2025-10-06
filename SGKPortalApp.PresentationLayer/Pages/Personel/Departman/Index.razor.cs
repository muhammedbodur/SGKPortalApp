using Microsoft.AspNetCore.Components;
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

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<DepartmanResponseDto> Departmanlar { get; set; } = new();
        private List<DepartmanResponseDto> FilteredDepartmanlar { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string searchTerm = string.Empty;
        private string filterStatus = "all";
        private string sortBy = "name-asc";

        // ═══════════════════════════════════════════════════════
        // PAGINATION PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => (int)Math.Ceiling(FilteredDepartmanlar.Count / (double)PageSize);

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = true;

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowDeleteModal { get; set; } = false;
        private int DeleteDepartmanId { get; set; }
        private string DeleteDepartmanAdi { get; set; } = string.Empty;
        private int DeleteDepartmanPersonelSayisi { get; set; }
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

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
                ApplyFiltersAndSort();
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

        // ═══════════════════════════════════════════════════════
        // FİLTRELEME VE SIRALAMA
        // ═══════════════════════════════════════════════════════

        private void OnSearchChanged()
        {
            CurrentPage = 1;
            ApplyFiltersAndSort();
        }

        private void OnFilterStatusChanged(ChangeEventArgs e)
        {
            filterStatus = e.Value?.ToString() ?? "all";
            CurrentPage = 1;
            ApplyFiltersAndSort();
        }

        private void OnSortByChanged(ChangeEventArgs e)
        {
            sortBy = e.Value?.ToString() ?? "name-asc";
            ApplyFiltersAndSort();
        }

        private void ClearFilters()
        {
            searchTerm = string.Empty;
            filterStatus = "all";
            sortBy = "name-asc";
            CurrentPage = 1;
            ApplyFiltersAndSort();
        }

        private void ApplyFiltersAndSort()
        {
            var query = Departmanlar.AsEnumerable();

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.DepartmanAdi
                    .Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Durum filtresi
            query = filterStatus switch
            {
                "active" => query.Where(d => d.DepartmanAktiflik == Aktiflik.Aktif),
                "passive" => query.Where(d => d.DepartmanAktiflik == Aktiflik.Pasif),
                _ => query
            };

            // Sıralama
            query = sortBy switch
            {
                "name-asc" => query.OrderBy(d => d.DepartmanAdi),
                "name-desc" => query.OrderByDescending(d => d.DepartmanAdi),
                "date-newest" => query.OrderByDescending(d => d.EklenmeTarihi),
                "date-oldest" => query.OrderBy(d => d.EklenmeTarihi),
                "personel-most" => query.OrderByDescending(d => d.PersonelSayisi),
                "personel-least" => query.OrderBy(d => d.PersonelSayisi),
                _ => query.OrderBy(d => d.DepartmanAdi)
            };

            FilteredDepartmanlar = query.ToList();
        }

        // ═══════════════════════════════════════════════════════
        // PAGİNATİON
        // ═══════════════════════════════════════════════════════

        private void ChangePage(int page)
        {
            if (page < 1 || page > TotalPages) return;
            CurrentPage = page;
            StateHasChanged();
        }

        // ═══════════════════════════════════════════════════════
        // NAVİGASYON
        // ═══════════════════════════════════════════════════════

        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/personel/departman/manage");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/personel/departman/manage/{id}");
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/personel/departman");
        }

        // ═══════════════════════════════════════════════════════
        // CRUD İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        private async Task ToggleStatus(int departmanId)
        {
            try
            {
                var departman = Departmanlar.FirstOrDefault(d => d.DepartmanId == departmanId);
                if (departman == null)
                {
                    await _toastService.ShowErrorAsync("Departman bulunamadı!");
                    return;
                }

                // Mock implementation - Gerçek API gelince değişecek
                var yeniDurum = departman.DepartmanAktiflik == Aktiflik.Aktif
                    ? Aktiflik.Pasif
                    : Aktiflik.Aktif;

                departman.DepartmanAktiflik = yeniDurum;
                departman.DuzenlenmeTarihi = DateTime.Now;

                var statusText = yeniDurum == Aktiflik.Aktif ? "aktif" : "pasif";
                await _toastService.ShowSuccessAsync($"Departman {statusText} yapıldı.");

                ApplyFiltersAndSort();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Durum değiştirme işlemi başarısız!");
            }
        }

        private void ShowDeleteConfirmation(int departmanId, string departmanAdi)
        {
            var departman = Departmanlar.FirstOrDefault(d => d.DepartmanId == departmanId);
            if (departman == null) return;

            DeleteDepartmanId = departmanId;
            DeleteDepartmanAdi = departmanAdi;
            DeleteDepartmanPersonelSayisi = departman.PersonelSayisi;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeleteDepartmanId = 0;
            DeleteDepartmanAdi = string.Empty;
            DeleteDepartmanPersonelSayisi = 0;
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
                    CloseDeleteModal();
                }
                else
                {
                    await _toastService.ShowErrorAsync("Departman silinemedi!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Silme işlemi başarısız!");
            }
            finally
            {
                IsDeleting = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // EXPORT İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        private async Task ExportToExcel()
        {
            await _toastService.ShowInfoAsync("Excel export özelliği yakında eklenecek...");
            // TODO: Excel export implementasyonu
            // Bu özellik için ClosedXML veya EPPlus kullanılabilir
        }

        private async Task ExportToPdf()
        {
            await _toastService.ShowInfoAsync("PDF export özelliği yakında eklenecek...");
            // TODO: PDF export implementasyonu
            // Bu özellik için QuestPDF veya iTextSharp kullanılabilir
        }
    }
}