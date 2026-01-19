using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
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
        [Inject] private IJSRuntime _js { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<YeniTalep> Logger { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private IzinMazeretTalepCreateRequestDto Request { get; set; } = new();
        private int? SelectedTuru { get; set; } = null;
        private int ToplamGun { get; set; } = 0;
        private string OverlapWarning { get; set; } = string.Empty;

        // Onaycı listeleri
        private List<IzinSorumluResponseDto> BirinciOnaycılar { get; set; } = new();
        private List<IzinSorumluResponseDto> IkinciOnaycılar { get; set; } = new();

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
                Request.Turu = (IzinMazeretTuru)turValue;
                ShowTurError = false;
                OverlapWarning = string.Empty;

                Request.BaslangicTarihi = null;
                Request.BitisTarihi = null;
                Request.MazeretTarihi = null;
                Request.SaatDilimi = null;
                ToplamGun = 0;
            }
            else
            {
                SelectedTuru = null;
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
            if (Request.Turu == IzinMazeretTuru.Mazeret)
            {
                if (!Request.MazeretTarihi.HasValue || string.IsNullOrWhiteSpace(Request.SaatDilimi))
                {
                    await ShowToast("warning", "Mazeret tarihi ve saat dilimi zorunludur");
                    return;
                }
            }
            else
            {
                if (!Request.BaslangicTarihi.HasValue || !Request.BitisTarihi.HasValue)
                {
                    await ShowToast("warning", "Başlangıç ve bitiş tarihi zorunludur");
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
                        await ShowToast("warning", "⚠️ Çakışma tespit edildi");
                    }
                    else
                    {
                        await ShowToast("success", "✅ Çakışma yok, talep oluşturabilirsiniz");
                    }
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Çakışma kontrolü yapılamadı");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Çakışma kontrolü sırasında hata oluştu");
                await ShowToast("error", "Çakışma kontrolü yapılamadı");
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

            try
            {
                IsSaving = true;

                var result = await _izinMazeretTalepService.CreateAsync(Request);

                if (result.Success)
                {
                    await ShowToast("success", "✅ Talep başarıyla oluşturuldu");
                    await Task.Delay(500);
                    _navigationManager.NavigateTo("/pdks/izin/taleplerim");
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talep oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                await ShowToast("error", "Talep oluşturulurken bir hata oluştu");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            try
            {
                var file = e.File;
                if (file == null) return;

                // Max 5MB
                const long maxFileSize = 5 * 1024 * 1024;
                if (file.Size > maxFileSize)
                {
                    await ShowToast("warning", "Dosya boyutu 5MB'dan küçük olmalıdır");
                    return;
                }

                // Dosyayı base64'e çevir ve request'e ekle
                using var stream = file.OpenReadStream(maxFileSize);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var base64 = Convert.ToBase64String(ms.ToArray());
                Request.BelgeEki = $"data:{file.ContentType};base64,{base64}";

                await ShowToast("success", "Belge yüklendi");
            }
            catch
            {
                await ShowToast("error", "Dosya yüklenemedi");
            }
        }

        private async Task ShowToast(string type, string message)
        {
            try
            {
                await _js.InvokeVoidAsync("showToast", type, message);
            }
            catch
            {
                // Toast gösterilemezse sessizce devam et
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
        /// Onaycı listelerini yükler (aktif olanlar)
        /// </summary>
        private async Task LoadApproversAsync()
        {
            try
            {
                IsLoadingApprovers = true;
                BirinciOnaycılar.Clear();
                IkinciOnaycılar.Clear();

                var result = await _izinSorumluApiService.GetActiveAsync();

                if (result.Success && result.Data != null)
                {
                    // 1. ve 2. Onaycıları ayır
                    BirinciOnaycılar = result.Data.Where(s => s.OnaySeviyesi == 1).ToList();
                    IkinciOnaycılar = result.Data.Where(s => s.OnaySeviyesi == 2).ToList();
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
    }
}
