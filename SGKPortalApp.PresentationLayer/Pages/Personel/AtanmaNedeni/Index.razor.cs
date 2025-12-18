using Microsoft.AspNetCore.Components;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SGKPortalApp.PresentationLayer.Components.Base;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.AtanmaNedeni
{
    public partial class Index
    {
        protected override string PagePermissionKey => "PER.ATANMANEDENI.INDEX";
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IAtanmaNedeniApiService _atanmaNedeniApiService { get; set; } = default!;

        private List<AtanmaNedeniResponseDto> AtanmaNedenleri { get; set; } = new();
        private List<AtanmaNedeniResponseDto> FilteredAtanmaNedenleri { get; set; } = new();
        
        private bool IsLoading { get; set; } = true;
        private bool IsDeleting { get; set; } = false;
        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = "";
        private bool ShowDeleteConfirmation { get; set; } = false;
        private AtanmaNedeniResponseDto? DeleteAtanmaNedeni { get; set; }
        private int DeleteAtanmaNedeniPersonelSayisi { get; set; }

        // Filtreleme
        private string searchTerm = "";
        private string sortBy = "name-asc";

        // Pagination
        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => (int)Math.Ceiling((double)FilteredAtanmaNedenleri.Count / PageSize);

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                IsLoading = true;
                var response = await _atanmaNedeniApiService.GetAllAsync();
                AtanmaNedenleri = response?.Data ?? new List<AtanmaNedeniResponseDto>();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veriler yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            FilteredAtanmaNedenleri = AtanmaNedenleri;

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                FilteredAtanmaNedenleri = FilteredAtanmaNedenleri
                    .Where(d => d.AtanmaNedeni.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sıralama
            FilteredAtanmaNedenleri = sortBy switch
            {
                "name-asc" => FilteredAtanmaNedenleri.OrderBy(d => d.AtanmaNedeni).ToList(),
                "name-desc" => FilteredAtanmaNedenleri.OrderByDescending(d => d.AtanmaNedeni).ToList(),
                "date-newest" => FilteredAtanmaNedenleri.OrderByDescending(d => d.AtanmaNedeniId).ToList(),
                "date-oldest" => FilteredAtanmaNedenleri.OrderBy(d => d.AtanmaNedeniId).ToList(),
                _ => FilteredAtanmaNedenleri.OrderBy(d => d.AtanmaNedeni).ToList()
            };

            CurrentPage = 1;
        }

        private void OnSearchChanged()
        {
            ApplyFilters();
            StateHasChanged();
        }

        private void OnSortByChanged(ChangeEventArgs e)
        {
            sortBy = e.Value?.ToString() ?? "name-asc";
            ApplyFilters();
        }

        private void ClearFilters()
        {
            searchTerm = "";
            sortBy = "name-asc";
            ApplyFilters();
        }

        private void ChangePage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
                StateHasChanged();
            }
        }

        private async Task ExportToExcel()
        {
            try
            {
                IsExporting = true;
                ExportType = "excel";
                await Task.Delay(1000); // Simüle edilmiş export
                await _toastService.ShowSuccessAsync("Excel dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Excel export hatası: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = "";
            }
        }

        private async Task ExportToPdf()
        {
            try
            {
                IsExporting = true;
                ExportType = "pdf";
                await Task.Delay(1000); // Simüle edilmiş export
                await _toastService.ShowSuccessAsync("PDF dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"PDF export hatası: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = "";
            }
        }

        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/personel/atanma-nedeni/manage");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/personel/atanma-nedeni/manage/{id}");
        }

        private async Task ShowDeleteModal(AtanmaNedeniResponseDto atanmaNedeni)
        {
            DeleteAtanmaNedeni = atanmaNedeni;

            // Personel sayısını al
            var personelCountResponse = await _atanmaNedeniApiService.GetPersonelCountAsync(atanmaNedeni.AtanmaNedeniId);
            DeleteAtanmaNedeniPersonelSayisi = personelCountResponse?.Data ?? 0;

            ShowDeleteConfirmation = true;
        }

        private void HideDeleteModal()
        {
            ShowDeleteConfirmation = false;
            DeleteAtanmaNedeni = null;
            DeleteAtanmaNedeniPersonelSayisi = 0;
        }

        private async Task ConfirmDelete()
        {
            if (DeleteAtanmaNedeni == null) return;

            try
            {
                IsDeleting = true;
                var response = await _atanmaNedeniApiService.DeleteAsync(DeleteAtanmaNedeni.AtanmaNedeniId);

                if (response?.Success == true)
                {
                    await _toastService.ShowSuccessAsync($"{DeleteAtanmaNedeni.AtanmaNedeni} başarıyla silindi!");
                    await LoadData();
                    HideDeleteModal();
                }
                else
                {
                    await _toastService.ShowErrorAsync(response?.Message ?? "Silme işlemi başarısız oldu!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsDeleting = false;
            }
        }
    }
}
