using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SGKPortalApp.PresentationLayer.Components.Base;

namespace SGKPortalApp.PresentationLayer.Pages.Personel
{
    public partial class Detail
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
        private List<PersonelCocukResponseDto> Cocuklar { get; set; } = new();
        private List<PersonelHizmetResponseDto> Hizmetler { get; set; } = new();
        private List<PersonelEgitimResponseDto> Egitimler { get; set; } = new();
        private List<PersonelImzaYetkisiResponseDto> ImzaYetkileri { get; set; } = new();
        private List<PersonelCezaResponseDto> Cezalar { get; set; } = new();
        private List<PersonelEngelResponseDto> Engeller { get; set; } = new();
        private List<PersonelAktiflikDurumHareketResponseDto> AktiflikDurumHareketleri { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = true;
        private bool NotFound { get; set; } = false;
        private string ActiveTab { get; set; } = "personel";
        private bool IsSendingToDevices { get; set; } = false;

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
                var result = await _personelApiService.GetByTcKimlikNoAsync(TcKimlikNo);

                if (result.Success && result.Data != null)
                {
                    Personel = result.Data;
                    
                    // Alt tablo bilgilerini yükle
                    Cocuklar = result.Data.Cocuklar ?? new List<PersonelCocukResponseDto>();
                    Hizmetler = result.Data.Hizmetler ?? new List<PersonelHizmetResponseDto>();
                    Egitimler = result.Data.Egitimler ?? new List<PersonelEgitimResponseDto>();
                    ImzaYetkileri = result.Data.ImzaYetkileriDetay ?? new List<PersonelImzaYetkisiResponseDto>();
                    Cezalar = result.Data.Cezalar ?? new List<PersonelCezaResponseDto>();
                    Engeller = result.Data.Engeller ?? new List<PersonelEngelResponseDto>();
                    AktiflikDurumHareketleri = result.Data.AktiflikDurumHareketleri?.OrderByDescending(h => h.DegisiklikTarihi).ToList()
                        ?? new List<PersonelAktiflikDurumHareketResponseDto>();
                }
                else
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
        // TAB HELPER METHODS (Blazor Server Compatible)
        // ═══════════════════════════════════════════════════════

        private void SetPersonelTab() => SetActiveTab("personel");
        private void SetIletisimTab() => SetActiveTab("iletisim");
        private void SetKisiselTab() => SetActiveTab("kisisel");
        private void SetOzlukTab() => SetActiveTab("ozluk");
        private void SetEsCocukTab() => SetActiveTab("es-cocuk");
        private void SetHizmetTab() => SetActiveTab("hizmet");
        private void SetEgitimTab() => SetActiveTab("egitim");
        private void SetYetkiTab() => SetActiveTab("yetki");
        private void SetCezaTab() => SetActiveTab("ceza");
        private void SetEngelTab() => SetActiveTab("engel");
        private void SetFotografTab() => SetActiveTab("fotograf");

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

        // ═══════════════════════════════════════════════════════
        // PDKS OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task SendCardToAllDevices()
        {
            if (Personel == null || string.IsNullOrEmpty(TcKimlikNo)) return;

            IsSendingToDevices = true;
            StateHasChanged();

            try
            {
                var result = await _personelApiService.SendCardToAllDevicesAsync(TcKimlikNo);

                if (result.Success && result.Data != null)
                {
                    var data = result.Data;
                    if (data.SuccessCount > 0)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? 
                            $"Kart bilgisi {data.SuccessCount} cihaza başarıyla gönderildi!" + 
                            (data.FailCount > 0 ? $" ({data.FailCount} cihaza gönderilemedi)" : ""));
                        
                        // Personel bilgilerini yenile
                        await LoadPersonelDetail();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync("Kart bilgisi hiçbir cihaza gönderilemedi!");
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Kart gönderilirken bir hata oluştu");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Kart gönderilirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsSendingToDevices = false;
                StateHasChanged();
            }
        }

        private async Task DeleteCardFromAllDevices()
        {
            if (Personel == null || string.IsNullOrEmpty(TcKimlikNo)) return;

            IsSendingToDevices = true;
            StateHasChanged();

            try
            {
                var result = await _personelApiService.DeleteCardFromAllDevicesAsync(TcKimlikNo);

                if (result.Success && result.Data != null)
                {
                    var data = result.Data;
                    if (data.SuccessCount > 0)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? 
                            $"Kart bilgisi {data.SuccessCount} cihazdan başarıyla silindi!" + 
                            (data.FailCount > 0 ? $" ({data.FailCount} cihazdan silinemedi)" : ""));
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync("Kart bilgisi hiçbir cihazdan silinemedi!");
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Kart silinirken bir hata oluştu");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Kart silinirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsSendingToDevices = false;
                StateHasChanged();
            }
        }
    }
}
