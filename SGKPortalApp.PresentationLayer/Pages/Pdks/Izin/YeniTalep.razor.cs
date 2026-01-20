using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SGKPortalApp.Common.Extensions;
using Microsoft.Extensions.Logging;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class YeniTalep
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IIzinMazeretTalepApiService _izinMazeretTalepService { get; set; } = default!;
        [Inject] private IIzinSorumluApiService _izinSorumluApiService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private ILogger<YeniTalep> Logger { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private IzinMazeretTalepCreateRequestDto Request { get; set; } = new();
        private int? SelectedTuru { get; set; } = null;
        private int ToplamGun { get; set; } = 0;
        private string OverlapWarning { get; set; } = string.Empty;

        // TimeSpan için string conversion (HTML time input için)
        private string BaslangicSaatiStr
        {
            get => Request.BaslangicSaati?.ToString(@"hh\:mm") ?? "";
            set
            {
                if (TimeSpan.TryParse(value, out var time))
                    Request.BaslangicSaati = time;
            }
        }

        private string BitisSaatiStr
        {
            get => Request.BitisSaati?.ToString(@"hh\:mm") ?? "";
            set
            {
                if (TimeSpan.TryParse(value, out var time))
                    Request.BitisSaati = time;
            }
        }

        // İzin türü listesi
        private List<IzinMazeretTuruResponseDto> IzinTurleri { get; set; } = new();
        private IzinMazeretTuruResponseDto? SelectedIzinTuru { get; set; }

        // Onaycı listeleri
        private List<PersonelResponseDto> AvailableApprovers { get; set; } = new();
        private string? SelectedBirinciOnayci { get; set; }
        private string? SelectedIkinciOnayci { get; set; }

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool ShowTurError { get; set; } = false;
        private bool IsSaving { get; set; } = false;
        private bool IsChecking { get; set; } = false;
        private bool IsLoadingApprovers { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var tcKimlikNo = await GetCurrentUserTcKimlikNoAsync();
            Request.TcKimlikNo = tcKimlikNo ?? string.Empty;

            // İzin türlerini yükle
            await LoadLeaveTypesAsync();

            // Onaycı listelerini yükle
            await LoadApproversAsync();
        }

        // ═══════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════════════════════════════

        private void OnTuruChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var turValue))
            {
                SelectedTuru = turValue;
                SelectedIzinTuru = IzinTurleri.FirstOrDefault(t => t.IzinMazeretTuruId == turValue);
                Request.IzinMazeretTuruId = turValue;
                ShowTurError = false;
                OverlapWarning = string.Empty;

                // Form alanlarını temizle
                Request.BaslangicTarihi = null;
                Request.BitisTarihi = null;
                Request.MazeretTarihi = null;
                Request.BaslangicSaati = null;
                Request.BitisSaati = null;
                ToplamGun = 0;
            }
            else
            {
                SelectedTuru = null;
                SelectedIzinTuru = null;
            }
        }

        private void CalculateDays()
        {
            if (Request.BaslangicTarihi.HasValue && Request.BitisTarihi.HasValue)
            {
                if (Request.BitisTarihi.Value >= Request.BaslangicTarihi.Value)
                {
                    ToplamGun = (Request.BitisTarihi.Value - Request.BaslangicTarihi.Value).Days + 1;
                }
                else
                {
                    ToplamGun = 0;
                }
            }
            else
            {
                ToplamGun = 0;
            }
        }

        private async Task CheckOverlap()
        {
            if (!SelectedTuru.HasValue)
            {
                ShowTurError = true;
                return;
            }

            // Tür seçilmiş mi kontrol et
            if (!SelectedTuru.HasValue)
            {
                ShowTurError = true;
                return;
            }

            // Validasyon
            if (SelectedIzinTuru != null && !SelectedIzinTuru.PlanliIzinMi)
            {
                if (!Request.MazeretTarihi.HasValue || !Request.BaslangicSaati.HasValue || !Request.BitisSaati.HasValue)
                {
                    await ToastService.ShowWarningAsync("Mazeret için tarih ve saat dilimi zorunludur");
                    return;
                }
            }
            else
            {
                if (!Request.BaslangicTarihi.HasValue || !Request.BitisTarihi.HasValue)
                {
                    await ToastService.ShowWarningAsync("Başlangıç ve bitiş tarihi zorunludur");
                    return;
                }
            }

            try
            {
                IsChecking = true;
                OverlapWarning = string.Empty;

                var result = await _izinMazeretTalepService.CheckOverlapAsync(Request);

                if (result.Success && result.Data != null)
                {
                    if (result.Data.HasOverlap)
                    {
                        OverlapWarning = result.Data.Message ?? "Bu tarih aralığında çakışma var!";
                        await ToastService.ShowWarningAsync("⚠️ Çakışma tespit edildi");
                    }
                    else
                    {
                        await ToastService.ShowSuccessAsync("✅ Çakışma yok, talep oluşturabilirsiniz");
                    }
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Çakışma kontrolü yapılamadı");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Çakışma kontrolü sırasında hata oluştu");
                await ToastService.ShowErrorAsync("Çakışma kontrolü yapılamadı");
            }
            finally
            {
                IsChecking = false;
            }
        }

        private async Task HandleSubmit()
        {
            if (!SelectedTuru.HasValue)
            {
                ShowTurError = true;
                return;
            }

            // Aynı kişi kontrolü
            if (!ValidateApprovers())
            {
                await ToastService.ShowErrorAsync("1. ve 2. onaycı aynı kişi olamaz!");
                return;
            }

            try
            {
                IsSaving = true;

                // Seçilen onaycıları request'e ata
                Request.BirinciOnayciTcKimlikNo = SelectedBirinciOnayci;
                Request.IkinciOnayciTcKimlikNo = SelectedIkinciOnayci;

                var result = await _izinMazeretTalepService.CreateAsync(Request);

                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync("✅ Talep başarıyla oluşturuldu");
                    await Task.Delay(500);
                    _navigationManager.NavigateTo("/pdks/izin/taleplerim");
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Talep oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync("Talep oluşturulurken bir hata oluştu");
            }
            finally
            {
                IsSaving = false;
            }
        }



        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private async Task<string?> GetCurrentUserTcKimlikNoAsync()
        {
            try
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                return authState.User.FindFirst("TcKimlikNo")?.Value;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Kullanıcının seçebileceği onaycıları yükler
        /// Kurallar:
        /// - Unvan ID = 7: Tüm departmanlarda
        /// - Servis ID = 33: Sadece kendi departmanında
        /// - Unvan ID IN (5,87): Aynı departman VE aynı servis
        /// </summary>
        private async Task LoadApproversAsync()
        {
            try
            {
                IsLoadingApprovers = true;
                AvailableApprovers.Clear();

                var tcKimlikNo = await GetCurrentUserTcKimlikNoAsync();
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    Logger.LogWarning("Kullanıcı TC Kimlik No alınamadı");
                    return;
                }

                var result = await _izinMazeretTalepService.GetAvailableApproversAsync(tcKimlikNo);

                if (result.Success && result.Data != null)
                {
                    AvailableApprovers = result.Data;
                    Logger.LogInformation("{Count} adet onaycı bulundu", AvailableApprovers.Count);
                }
                else
                {
                    Logger.LogWarning("Onaycı listesi alınamadı: {Message}", result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Onaycı listesi yüklenirken hata oluştu");
            }
            finally
            {
                IsLoadingApprovers = false;
            }
        }

        /// <summary>
        /// İzin türlerini yükler
        /// </summary>
        private async Task LoadLeaveTypesAsync()
        {
            try
            {
                var result = await _izinMazeretTalepService.GetAvailableLeaveTypesAsync();

                if (result.Success && result.Data != null)
                {
                    IzinTurleri = result.Data;
                    Logger.LogInformation("{Count} adet izin türü yüklendi", IzinTurleri.Count);
                }
                else
                {
                    Logger.LogWarning("İzin türleri alınamadı: {Message}", result.Message);
                    await ToastService.ShowWarningAsync("İzin türleri yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "İzin türleri yüklenirken hata oluştu");
                await ToastService.ShowErrorAsync("İzin türleri yüklenemedi");
            }
        }

        /// <summary>
        /// Seçili izin türü için 2. onaycı gerekli mi?
        /// </summary>
        private bool RequiresSecondApprover()
        {
            return SelectedIzinTuru?.IkinciOnayciGerekli ?? true;
        }

        /// <summary>
        /// 1. ve 2. onaycı için aynı kişi seçilip seçilmediğini kontrol eder
        /// </summary>
        private bool ValidateApprovers()
        {
            if (!string.IsNullOrEmpty(SelectedBirinciOnayci) && 
                !string.IsNullOrEmpty(SelectedIkinciOnayci) &&
                SelectedBirinciOnayci == SelectedIkinciOnayci)
            {
                return false;
            }
            return true;
        }
    }
}
