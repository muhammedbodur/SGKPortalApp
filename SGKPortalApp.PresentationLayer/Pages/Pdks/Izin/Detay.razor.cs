using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class Detay
    {
        [Parameter] public int Id { get; set; }

        [Inject] private IIzinMazeretTalepApiService _izinMazeretTalepService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IJSRuntime _js { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;

        private IzinMazeretTalepResponseDto? Talep { get; set; }
        private bool IsLoading { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadTalep();
        }

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
                        await ShowToast("error", "Bu kaydı görüntüleme yetkiniz yok");
                        _navigationManager.NavigateTo("/pdks/izin/taleplerim");
                        return;
                    }

                    Talep = talep;
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talep bulunamadı");
                    Talep = null;
                }
            }
            catch (Exception ex)
            {
                await ShowToast("error", $"Talep yüklenirken hata: {ex.Message}");
                Talep = null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void GoBack()
        {
            _navigationManager.NavigateTo("/pdks/izin/taleplerim");
        }

        /// <summary>
        /// Talebin düzenlenebilir olup olmadığını kontrol eder
        /// Sadece tüm onaylar "Beklemede" olan talepler düzenlenebilir
        /// </summary>
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

        private void EditTalep()
        {
            if (Talep == null) return;
            _navigationManager.NavigateTo($"/pdks/izin/duzenle/{Talep.IzinMazeretTalepId}");
        }

        private async Task CancelTalep()
        {
            if (Talep == null) return;

            var iptalNedeni = await _js.InvokeAsync<string>("prompt",
                "Lütfen iptal nedenini giriniz:");

            if (string.IsNullOrWhiteSpace(iptalNedeni))
                return;

            try
            {
                var result = await _izinMazeretTalepService.CancelAsync(Talep.IzinMazeretTalepId, iptalNedeni);

                if (result.Success)
                {
                    await ShowToast("success", result.Message ?? "Talep başarıyla iptal edildi");
                    await Task.Delay(1500);
                    _navigationManager.NavigateTo("/pdks/izin/taleplerim");
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talep iptal edilemedi");
                }
            }
            catch (Exception ex)
            {
                await ShowToast("error", $"Hata: {ex.Message}");
            }
        }

        private async Task DeleteTalep()
        {
            if (Talep == null) return;

            var confirmed = await _js.InvokeAsync<bool>("confirm",
                "Bu talebi kalıcı olarak silmek istediğinizden emin misiniz?\n\nBu işlem geri alınamaz!");

            if (!confirmed)
                return;

            try
            {
                var result = await _izinMazeretTalepService.DeleteAsync(Talep.IzinMazeretTalepId);

                if (result.Success)
                {
                    await ShowToast("success", result.Message ?? "Talep başarıyla silindi");
                    await Task.Delay(1500);
                    _navigationManager.NavigateTo("/pdks/izin/taleplerim");
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talep silinemedi");
                }
            }
            catch (Exception ex)
            {
                await ShowToast("error", $"Hata: {ex.Message}");
            }
        }

        private string GetOnayBadgeClass(string onayDurumu)
        {
            return onayDurumu switch
            {
                "Beklemede" => "bg-label-warning",
                "Onaylandı" => "bg-label-success",
                "Reddedildi" => "bg-label-danger",
                "İptal Edildi" => "bg-label-secondary",
                _ => "bg-label-secondary"
            };
        }

        private string GetDurumBadgeClass(string durum)
        {
            return durum switch
            {
                "Onaylandı" => "bg-label-success",
                "Reddedildi" => "bg-label-danger",
                "İptal" => "bg-label-secondary",
                _ => "bg-label-warning"
            };
        }

        /// <summary>
        /// Talebin genel durumunu hesaplar (IsActive + onay durumları)
        /// </summary>
        private string GetGenelDurum()
        {
            if (Talep == null)
                return "Bilinmiyor";

            if (!Talep.IsActive)
                return "İptal";

            if (Talep.BirinciOnayDurumu == OnayDurumu.Reddedildi ||
                Talep.IkinciOnayDurumu == OnayDurumu.Reddedildi)
                return "Reddedildi";

            if (Talep.BirinciOnayDurumu == OnayDurumu.Onaylandi &&
                Talep.IkinciOnayDurumu == OnayDurumu.Onaylandi)
                return "Onaylandı";

            if (Talep.BirinciOnayDurumu == OnayDurumu.Beklemede)
                return "1. Onay Bekliyor";

            if (Talep.IkinciOnayDurumu == OnayDurumu.Beklemede)
                return "2. Onay Bekliyor";

            return "Beklemede";
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
