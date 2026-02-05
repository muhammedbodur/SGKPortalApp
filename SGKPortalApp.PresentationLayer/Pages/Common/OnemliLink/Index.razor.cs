using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using Microsoft.AspNetCore.Components.Web;

namespace SGKPortalApp.PresentationLayer.Pages.Common.OnemliLink
{
    public partial class Index
    {
        [Inject] private IOnemliLinkApiService _onemliLinkService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private List<OnemliLinkResponseDto> Items { get; set; } = new();
        private List<OnemliLinkResponseDto> FilteredItems { get; set; } = new();

        private string SearchTerm { get; set; } = string.Empty;
        private Aktiflik? AktiflikFilter { get; set; }

        private bool IsLoading { get; set; } = true;

        private bool ShowDeleteModal { get; set; } = false;
        private bool IsDeleting { get; set; } = false;
        private int DeleteId { get; set; }
        private string DeleteName { get; set; } = string.Empty;

        private int ActiveCount => Items.Count(x => x.Aktiflik == Aktiflik.Aktif);
        private int PassiveCount => Items.Count(x => x.Aktiflik == Aktiflik.Pasif);

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            IsLoading = true;
            try
            {
                var result = await _onemliLinkService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    Items = result.Data;
                    ApplyFilters();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Linkler yüklenemedi!");
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

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(x => x.LinkAdi.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (AktiflikFilter.HasValue)
            {
                query = query.Where(x => x.Aktiflik == AktiflikFilter.Value);
            }

            FilteredItems = query
                .OrderBy(x => x.Sira)
                .ThenBy(x => x.LinkAdi)
                .ToList();
        }

        private void OnSearchChanged(KeyboardEventArgs _)
        {
            ApplyFilters();
        }

        private void OnAktiflikChanged(ChangeEventArgs e)
        {
            if (e?.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                AktiflikFilter = null;
            }
            else if (Enum.TryParse<Aktiflik>(e.Value.ToString(), out var parsed))
            {
                AktiflikFilter = parsed;
            }

            ApplyFilters();
        }

        private void ClearFilters()
        {
            SearchTerm = string.Empty;
            AktiflikFilter = null;
            ApplyFilters();
        }

        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/common/onemli-link/manage");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/common/onemli-link/manage/{id}");
        }

        private void ShowDeleteConfirmation(int id, string name)
        {
            DeleteId = id;
            DeleteName = name;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeleteId = 0;
            DeleteName = string.Empty;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _onemliLinkService.DeleteAsync(DeleteId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Link başarıyla silindi!");
                    CloseDeleteModal();
                    await LoadData();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Link silinemedi!");
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
