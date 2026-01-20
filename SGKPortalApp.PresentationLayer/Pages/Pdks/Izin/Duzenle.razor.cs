using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class Duzenle
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IIzinMazeretTalepApiService _izinMazeretTalepService { get; set; } = default!;
        [Inject] private IJSRuntime _js { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter] public int Id { get; set; }

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private IzinMazeretTalepUpdateRequestDto Request { get; set; } = new();
        private string SelectedTuruText { get; set; } = string.Empty;
        private bool IsMazeretTalep { get; set; } = false;
        private int ToplamGun { get; set; } = 0;

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = false;
        private bool IsSubmitting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadTalep();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING METHODS
        // ═══════════════════════════════════════════════════════

        private async Task LoadTalep()
        {
            try
            {
                IsLoading = true;

                var result = await _izinMazeretTalepService.GetByIdAsync(Id);

                if (result.Success && result.Data != null)
                {
                    var talep = result.Data;

                    // 🔒 FRONTEND OWNERSHIP KONTROLÜ (Defense in Depth)
                    // Backend'de de kontrol var, ama frontend'de de kontrol ederek daha iyi UX sağlıyoruz
                    var currentUserTc = await GetCurrentUserTcKimlikNoAsync();
                    if (!string.IsNullOrEmpty(currentUserTc) && talep.TcKimlikNo != currentUserTc)
                    {
                        await ShowToast("error", "Bu kaydı düzenleme yetkiniz yok");
                        _navigationManager.NavigateTo("/pdks/izin/taleplerim");
                        return;
                    }

                    // Sadece "Beklemede" olan talepler düzenlenebilir
                    if (!CanEdit(talep))
                    {
                        await ShowToast("warning", "Bu talep düzenlenemez. Sadece onaylanmamış talepler düzenlenebilir.");
                        Request = null;
                        return;
                    }

                    // Response DTO'dan Update DTO'ya dönüştür
                    Request = MapToUpdateRequest(talep);
                    SelectedTuruText = talep.TuruAdi;
                    IsMazeretTalep = talep.Turu == IzinMazeretTuru.Mazeret;

                    if (!IsMazeretTalep)
                    {
                        CalculateDays();
                    }
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talep bilgileri yüklenemedi");
                    Request = null;
                }
            }
            catch (Exception ex)
            {
                await ShowToast("error", $"Talep yüklenirken hata oluştu: {ex.Message}");
                Request = null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private bool CanEdit(IzinMazeretTalepResponseDto talep)
        {
            // Eğer birinci onay onaylandı veya reddedildi ise düzenlenemez
            if (talep.BirinciOnayDurumu != OnayDurumu.Beklemede)
                return false;

            // Eğer ikinci onay var ve onaylandı/reddedildi ise düzenlenemez
            if (talep.IkinciOnayDurumu != OnayDurumu.Beklemede)
                return false;

            return true;
        }

        private IzinMazeretTalepUpdateRequestDto MapToUpdateRequest(IzinMazeretTalepResponseDto source)
        {
            return new IzinMazeretTalepUpdateRequestDto
            {
                Turu = source.Turu,
                BaslangicTarihi = source.BaslangicTarihi,
                BitisTarihi = source.BitisTarihi,
                MazeretTarihi = source.MazeretTarihi,
                SaatDilimi = source.SaatDilimi,
                Aciklama = source.Aciklama,
                BelgeEki = source.BelgeEki
            };
        }

        private void CalculateDays()
        {
            if (Request.BaslangicTarihi.HasValue && Request.BitisTarihi.HasValue)
            {
                var days = (Request.BitisTarihi.Value - Request.BaslangicTarihi.Value).Days + 1;
                ToplamGun = days > 0 ? days : 0;
                // Not: ToplamGun backend'de hesaplanıyor, UpdateRequestDto'da yok
            }
            else
            {
                ToplamGun = 0;
            }
        }

        // ═══════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════════════════════════════

        private async Task HandleSubmit()
        {
            try
            {
                IsSubmitting = true;

                var result = await _izinMazeretTalepService.UpdateAsync(Id, Request);

                if (result.Success)
                {
                    await ShowToast("success", result.Message ?? "Talep başarıyla güncellendi");
                    await Task.Delay(1500);
                    GoBack();
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talep güncellenemedi");
                }
            }
            catch (Exception ex)
            {
                await ShowToast("error", $"Talep güncellenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private void GoBack()
        {
            _navigationManager.NavigateTo("/pdks/izin/taleplerim");
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

        /// <summary>
        /// Giriş yapmış kullanıcının TC Kimlik Numarasını alır
        /// </summary>
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
    }
}
