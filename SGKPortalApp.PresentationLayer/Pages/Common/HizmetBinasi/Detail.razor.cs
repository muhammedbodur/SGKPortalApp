using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.HizmetBinasi
{
    public partial class Detail
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter] public int Id { get; set; }

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private HizmetBinasiDetailResponseDto? HizmetBinasi { get; set; }

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = true;
        private bool NotFound { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        protected override async Task OnParametersSetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            IsLoading = true;
            NotFound = false;

            try
            {
                var result = await _hizmetBinasiService.GetDetailByIdAsync(Id);

                if (!result.Success || result.Data == null)
                {
                    NotFound = true;
                    await _toastService.ShowErrorAsync(result.Message ?? "Hizmet binası bulunamadı!");
                }
                else
                {
                    HizmetBinasi = result.Data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Veri yüklenirken bir hata oluştu!");
                NotFound = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // NAVIGATION
        // ═══════════════════════════════════════════════════════

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        private void NavigateToList()
        {
            _navigationManager.NavigateTo("/common/hizmet-binasi");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/common/hizmet-binasi/manage/{id}");
        }

        private void NavigateToPersonelDetail(string TcKimlikNo)
        {
            _navigationManager.NavigateTo($"/personel/detail/{TcKimlikNo}");
        }
    }
}