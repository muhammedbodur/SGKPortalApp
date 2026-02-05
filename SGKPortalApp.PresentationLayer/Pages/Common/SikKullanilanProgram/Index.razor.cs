using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.SikKullanilanProgram
{
    public partial class Index
    {
        [Inject] private ISikKullanilanProgramApiService _programService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private List<SikKullanilanProgramResponseDto> Items { get; set; } = new();
        private List<SikKullanilanProgramResponseDto> FilteredItems { get; set; } = new();

        private string SearchTerm { get; set; } = string.Empty;
        private Aktiflik? AktiflikFilter { get; set; }

        private bool IsLoading { get; set; } = true;

        private bool ShowDeleteModal { get; set; }
        private bool IsDeleting { get; set; }
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
                var result = await _programService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    Items = result.Data;
                    ApplyFilters();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Programlar yüklenemedi!");
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
                query = query.Where(x => x.ProgramAdi.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (AktiflikFilter.HasValue)
            {
                query = query.Where(x => x.Aktiflik == AktiflikFilter.Value);
            }

            FilteredItems = query
                .OrderBy(x => x.Sira)
                .ThenBy(x => x.ProgramAdi)
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
            _navigationManager.NavigateTo("/common/sik-kullanilan-program/manage");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/common/sik-kullanilan-program/manage/{id}");
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
                var result = await _programService.DeleteAsync(DeleteId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Program başarıyla silindi!");
                    CloseDeleteModal();
                    await LoadData();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Program silinemedi!");
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
