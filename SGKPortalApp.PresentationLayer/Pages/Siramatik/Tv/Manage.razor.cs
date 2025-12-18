using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Tv
{
    public partial class Manage
    {
        protected override string PagePermissionKey => "SIRA.TV.MANAGE";

        [Parameter]
        public int TvId { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? HizmetBinasiId { get; set; }

        [Inject]
        private ITvApiService _tvService { get; set; } = default!;

        [Inject]
        private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;

        [Inject]
        private IToastService _toastService { get; set; } = default!;

        [Inject]
        private NavigationManager _navigationManager { get; set; } = default!;

        [Inject]
        private ILogger<Manage> _logger { get; set; } = default!;

        private TvFormModel model = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();

        private bool isLoading = true;
        private bool isSaving = false;
        private bool isAktif = true;

        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi = null;

        private bool IsEditMode => TvId > 0;
        private int selectedHizmetBinasiId = 0;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;

            try
            {
                // Hizmet binalarını yükle
                var binaResult = await _hizmetBinasiService.GetAllAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data
                        .Where(b => b.Aktiflik == Aktiflik.Aktif)
                        .OrderBy(b => b.HizmetBinasiAdi)
                        .ToList();
                }

                // Edit mode ise TV'yi yükle
                if (IsEditMode)
                {
                    var tvResult = await _tvService.GetByIdAsync(TvId);
                    if (tvResult.Success && tvResult.Data != null)
                    {
                        var tv = tvResult.Data;
                        model = new TvFormModel
                        {
                            TvAdi = tv.TvAdi,
                            HizmetBinasiId = tv.HizmetBinasiId,
                            KatTipi = tv.KatTipi,
                            TvAciklama = tv.TvAciklama
                        };
                        isAktif = tv.Aktiflik == Aktiflik.Aktif;
                        eklenmeTarihi = tv.EklenmeTarihi;
                        duzenlenmeTarihi = tv.DuzenlenmeTarihi;
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync("TV bulunamadı");
                        NavigateBack();
                    }
                }
                else
                {
                    // ✅ Güvenlik: URL'den HizmetBinasiId parametresi geldiyse yetki kontrolü
                    if (HizmetBinasiId.HasValue && HizmetBinasiId.Value > 0)
                    {
                        if (!CanAccessHizmetBinasi(HizmetBinasiId.Value))
                        {
                            await _toastService.ShowWarningAsync("Bu Hizmet Binasına erişim yetkiniz yok!");
                            _logger.LogWarning("Yetkisiz Hizmet Binası erişim denemesi (URL): {BinaId}", HizmetBinasiId.Value);
                            _navigationManager.NavigateTo("/siramatik/tv");
                            return;
                        }

                        selectedHizmetBinasiId = HizmetBinasiId.Value;
                        model.HizmetBinasiId = HizmetBinasiId.Value;
                    }
                    else
                    {
                        // ✅ URL'den parametre gelmediyse kullanıcının kendi HizmetBinası'nı seç
                        var userHizmetBinasiId = GetCurrentUserHizmetBinasiId();
                        if (userHizmetBinasiId > 0)
                        {
                            selectedHizmetBinasiId = userHizmetBinasiId;
                            model.HizmetBinasiId = userHizmetBinasiId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veri yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async void OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int hizmetBinasiId))
            {
                // ✅ Güvenlik kontrolü
                if (hizmetBinasiId > 0 && !CanAccessHizmetBinasi(hizmetBinasiId))
                {
                    await _toastService.ShowWarningAsync("Bu Hizmet Binasını seçme yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz Hizmet Binası seçim denemesi: {BinaId}", hizmetBinasiId);
                    return;
                }

                selectedHizmetBinasiId = hizmetBinasiId;
                model.HizmetBinasiId = hizmetBinasiId;
            }
        }

        private async Task HandleSubmit()
        {
            if (model.HizmetBinasiId == 0)
            {
                await _toastService.ShowWarningAsync("Lütfen hizmet binası seçiniz");
                return;
            }

            isSaving = true;

            try
            {
                if (IsEditMode)
                {
                    var updateDto = new TvUpdateRequestDto
                    {
                        TvId = TvId,
                        TvAdi = model.TvAdi,
                        HizmetBinasiId = model.HizmetBinasiId,
                        KatTipi = model.KatTipi,
                        TvAciklama = model.TvAciklama,
                        Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _tvService.UpdateAsync(updateDto);
                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("TV başarıyla güncellendi");
                        NavigateBack();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Güncelleme başarısız");
                    }
                }
                else
                {
                    // ✅ Güvenlik: Form submit öncesi son kontrol (form manipulation önlemi)
                    if (!CanAccessHizmetBinasi(model.HizmetBinasiId))
                    {
                        await _toastService.ShowErrorAsync("Bu Hizmet Binasında kayıt oluşturma yetkiniz yok!");
                        _logger.LogWarning("Yetkisiz kayıt oluşturma denemesi: HizmetBinasiId={BinaId}", model.HizmetBinasiId);
                        return;
                    }

                    var createDto = new TvCreateRequestDto
                    {
                        TvAdi = model.TvAdi,
                        HizmetBinasiId = model.HizmetBinasiId,
                        KatTipi = model.KatTipi,
                        TvAciklama = model.TvAciklama,
                        Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _tvService.CreateAsync(createDto);
                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("TV başarıyla oluşturuldu");
                        NavigateBack();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Oluşturma başarısız");
                    }
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/tv");
        }

        // Form Model
        private class TvFormModel
        {
            public string TvAdi { get; set; } = string.Empty;
            public int HizmetBinasiId { get; set; }
            public KatTipi KatTipi { get; set; } = KatTipi.zemin;
            public string? TvAciklama { get; set; }
        }
    }
}
