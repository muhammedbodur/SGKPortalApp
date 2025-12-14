using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Yetki
{
    public partial class Index : ComponentBase
    {
        [Inject] private IYetkiApiService _yetkiService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private List<YetkiResponseDto> Yetkiler { get; set; } = new();
        private List<YetkiResponseDto> FilteredYetkiler { get; set; } = new();

        private bool IsLoading { get; set; } = true;

        private string searchTerm = string.Empty;
        private string sortBy = "name-asc";

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => (int)Math.Ceiling(FilteredYetkiler.Count / (double)PageSize);

        private IEnumerable<YetkiResponseDto> PagedItems =>
            FilteredYetkiler
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize);

        protected override async Task OnInitializedAsync()
        {
            await LoadYetkiler();
        }

        private async Task LoadYetkiler()
        {
            IsLoading = true;
            try
            {
                var result = await _yetkiService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    Yetkiler = result.Data;
                    ApplyFiltersAndSort();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Yetkiler yüklenemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFiltersAndSort()
        {
            var query = Yetkiler.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(y => y.YetkiAdi.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            query = sortBy switch
            {
                "name-asc" => query.OrderBy(y => y.YetkiAdi),
                "name-desc" => query.OrderByDescending(y => y.YetkiAdi),
                "date-newest" => query.OrderByDescending(y => y.EklenmeTarihi),
                "date-oldest" => query.OrderBy(y => y.EklenmeTarihi),
                _ => query.OrderBy(y => y.YetkiAdi)
            };

            FilteredYetkiler = query.ToList();
            CurrentPage = 1;
        }

        private void OnSearchChanged()
        {
            CurrentPage = 1;
            ApplyFiltersAndSort();
        }

        private void OnSortByChanged(ChangeEventArgs e)
        {
            sortBy = e.Value?.ToString() ?? "name-asc";
            CurrentPage = 1;
            ApplyFiltersAndSort();
        }

        private void ClearFilters()
        {
            searchTerm = string.Empty;
            sortBy = "name-asc";
            CurrentPage = 1;
            ApplyFiltersAndSort();
        }

        private void ChangePage(int page)
        {
            if (page < 1 || page > TotalPages) return;
            CurrentPage = page;
        }

        private void NavigateToCreate() => _navigationManager.NavigateTo("/personel/yetki/manage");
        private void NavigateToEdit(int id) => _navigationManager.NavigateTo($"/personel/yetki/manage/{id}");

        private string GetParentName(int? ustYetkiId)
        {
            if (!ustYetkiId.HasValue)
                return "-";

            var parent = Yetkiler.FirstOrDefault(x => x.YetkiId == ustYetkiId.Value);
            return parent?.YetkiAdi ?? "-";
        }
    }
}
