using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;

namespace SGKPortalApp.PresentationLayer.Pages.Personel
{
    public partial class Detail : ComponentBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IPersonelApiService _personelApiService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter] public string? TcKimlikNo { get; set; }

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private PersonelResponseDto? Personel { get; set; }

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = true;
        private bool NotFound { get; set; } = false;
        private string ActiveTab { get; set; } = "personel";

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await LoadPersonelDetail();
        }

        private async Task LoadPersonelDetail()
        {
            IsLoading = true;
            try
            {
                // TC Kimlik No kontrolü
                if (string.IsNullOrEmpty(TcKimlikNo))
                {
                    NotFound = true;
                    IsLoading = false;
                    return;
                }

                // API'den personel detayını getir
                Personel = await _personelApiService.GetByTcKimlikNoAsync(TcKimlikNo);

                if (Personel == null)
                {
                    NotFound = true;
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Personel yüklenirken hata oluştu: {ex.Message}");
                NotFound = true;
            }
            finally
            {
                IsLoading = false;
            }
        }


        // ═══════════════════════════════════════════════════════
        // TAB METHODS
        // ═══════════════════════════════════════════════════════

        private void SetActiveTab(string tabName)
        {
            ActiveTab = tabName;
            StateHasChanged();
        }

        // ═══════════════════════════════════════════════════════
        // NAVIGATION METHODS
        // ═══════════════════════════════════════════════════════

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        private void NavigateToPersonelList()
        {
            _navigationManager.NavigateTo("/personel");
        }

        private void NavigateToEdit()
        {
            _navigationManager.NavigateTo($"/personel/manage/{TcKimlikNo}");
        }
    }
}
