using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.GununMenusu
{
    public partial class Index
    {
        [Inject] private IGununMenusuApiService _gununMenusuService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private List<GununMenusuResponseDto> Items { get; set; } = new();
        private List<GununMenusuResponseDto> FilteredItems { get; set; } = new();

        private DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7);
        private DateTime EndDate { get; set; } = DateTime.Today;
        private string SearchTerm { get; set; } = string.Empty;
        private Aktiflik? AktiflikFilter { get; set; }

        private bool IsLoading { get; set; } = true;

        private bool ShowDeleteModal { get; set; } = false;
        private bool IsDeleting { get; set; } = false;
        private int DeleteId { get; set; }
        private DateTime DeleteDate { get; set; }
        private string DeleteDateText => DeleteDate.ToString("dd.MM.yyyy");

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            IsLoading = true;
            try
            {
                var result = await _gununMenusuService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    Items = result.Data;
                    ApplyFilters();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Menüler yüklenemedi!");
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

        private void ApplyFilters()
        {
            var query = Items.AsEnumerable();

            query = query.Where(x => x.Tarih.Date >= StartDate.Date && x.Tarih.Date <= EndDate.Date);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(x => x.Icerik.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (AktiflikFilter.HasValue)
            {
                query = query.Where(x => x.Aktiflik == AktiflikFilter.Value);
            }

            FilteredItems = query
                .OrderByDescending(x => x.Tarih)
                .ThenByDescending(x => x.EklenmeTarihi)
                .ToList();
        }

        private void ClearFilters()
        {
            StartDate = DateTime.Today.AddDays(-7);
            EndDate = DateTime.Today;
            SearchTerm = string.Empty;
            AktiflikFilter = null;
            ApplyFilters();
        }

        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/common/gunun-menusu/add");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/common/gunun-menusu/manage/{id}");
        }

        private void ShowDeleteConfirmation(int id, DateTime tarih)
        {
            DeleteId = id;
            DeleteDate = tarih;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeleteId = 0;
            DeleteDate = default;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _gununMenusuService.DeleteAsync(DeleteId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Menü başarıyla silindi!");
                    CloseDeleteModal();
                    await LoadData();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Menü silinemedi!");
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
