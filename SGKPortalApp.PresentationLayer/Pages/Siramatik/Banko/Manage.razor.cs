using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Banko
{
    public partial class Manage
    {
        [Inject] private IBankoApiService _bankoService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? BankoId { get; set; }
        
        [Parameter]
        [SupplyParameterFromQuery]
        public int? HizmetBinasiId { get; set; }

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool IsEditMode => BankoId.HasValue && BankoId.Value > 0;

        // Form Model
        private BankoFormModel model = new();

        // Lookup Data
        private List<HizmetBinasiResponseDto> allHizmetBinalari = new();
        private int selectedHizmetBinasiId = 0;
        private string hizmetBinasiAdi = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownData();

            if (IsEditMode)
            {
                await LoadBanko();
            }
            else
            {
                // Yeni kayıt için varsayılan değerler
                model = new BankoFormModel
                {
                    HizmetBinasiId = 0,
                    KatTipi = KatTipi.zemin,
                    BankoNo = 1,
                    BankoTipi = BankoTipi.Normal
                };

                // URL'den hizmet binası parametresi geldiyse otomatik seç
                if (HizmetBinasiId.HasValue && HizmetBinasiId.Value > 0)
                {
                    selectedHizmetBinasiId = HizmetBinasiId.Value;
                    model.HizmetBinasiId = HizmetBinasiId.Value;
                    _logger.LogInformation($"🔗 URL'den HizmetBinasiId alındı: {HizmetBinasiId.Value}");
                }
            }
        }

        private async Task LoadDropdownData()
        {
            isLoading = true;
            try
            {
                var result = await _hizmetBinasiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    allHizmetBinalari = result.Data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadBanko()
        {
            if (!BankoId.HasValue) return;

            isLoading = true;
            try
            {
                var result = await _bankoService.GetByIdAsync(BankoId.Value);
                if (result.Success && result.Data != null)
                {
                    var banko = result.Data;
                    model = new BankoFormModel
                    {
                        HizmetBinasiId = banko.HizmetBinasiId,
                        KatTipi = banko.KatTipi,
                        BankoNo = banko.BankoNo,
                        BankoTipi = banko.BankoTipi,
                        BankoAciklama = banko.BankoAciklama
                    };

                    selectedHizmetBinasiId = banko.HizmetBinasiId;
                    hizmetBinasiAdi = banko.HizmetBinasiAdi;
                }
                else
                {
                    await _toastService.ShowErrorAsync("Banko bulunamadı");
                    NavigateBack();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Banko yüklenirken hata oluştu");
                NavigateBack();
            }
            finally
            {
                isLoading = false;
            }
        }

        private void OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int hizmetBinasiId))
            {
                selectedHizmetBinasiId = hizmetBinasiId;
                model.HizmetBinasiId = hizmetBinasiId;
            }
        }

        private async Task HandleSubmit()
        {
            isSaving = true;
            try
            {
                if (IsEditMode)
                {
                    // Update
                    var updateDto = new BankoUpdateRequestDto
                    {
                        BankoNo = model.BankoNo,
                        KatTipi = model.KatTipi,
                        BankoTipi = model.BankoTipi,
                        BankoAciklama = model.BankoAciklama
                    };

                    var result = await _bankoService.UpdateAsync(BankoId!.Value, updateDto);
                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Banko başarıyla güncellendi");
                        NavigateBack();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message);
                    }
                }
                else
                {
                    // Create
                    var createDto = new BankoCreateRequestDto
                    {
                        HizmetBinasiId = model.HizmetBinasiId,
                        BankoNo = model.BankoNo,
                        KatTipi = model.KatTipi,
                        BankoTipi = model.BankoTipi,
                        BankoAciklama = model.BankoAciklama
                    };

                    var result = await _bankoService.CreateAsync(createDto);
                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Banko başarıyla eklendi");
                        NavigateBack();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko kaydedilirken hata oluştu");
                await _toastService.ShowErrorAsync("Banko kaydedilirken hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private void NavigateBack()
        {
            if (HizmetBinasiId.HasValue && HizmetBinasiId.Value > 0)
            {
                _navigationManager.NavigateTo($"/siramatik/banko/list?hizmetBinasiId={HizmetBinasiId.Value}");
            }
            else
            {
                _navigationManager.NavigateTo("/siramatik/banko/list");
            }
        }
    }
}
