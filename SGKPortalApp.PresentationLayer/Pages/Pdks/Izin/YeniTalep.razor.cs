using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.Common.Extensions;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class YeniTalep
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILogger<YeniTalep> Logger { get; set; } = default!;

        private IzinMazeretTalepCreateRequestDto request = new();
        private int? selectedTuru = null;
        private bool showTurError = false;
        private bool isSaving = false;
        private bool isChecking = false;
        private int toplamGun = 0;
        private string overlapWarning = string.Empty;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Kullanıcı TC'sini al (User claim'den gelecek)
            request.TcKimlikNo = "12345678901"; // TODO: User claim'den al
        }

        private void OnTuruChanged()
        {
            if (selectedTuru.HasValue)
            {
                request.Turu = (IzinMazeretTuru)selectedTuru.Value;
                showTurError = false;
                overlapWarning = string.Empty;

                // Alanları temizle
                request.BaslangicTarihi = null;
                request.BitisTarihi = null;
                request.MazeretTarihi = null;
                request.SaatDilimi = null;
                toplamGun = 0;
            }
        }

        private void CalculateDays()
        {
            if (request.BaslangicTarihi.HasValue && request.BitisTarihi.HasValue)
            {
                if (request.BitisTarihi.Value >= request.BaslangicTarihi.Value)
                {
                    toplamGun = (request.BitisTarihi.Value - request.BaslangicTarihi.Value).Days + 1;
                }
                else
                {
                    toplamGun = 0;
                }
            }
            else
            {
                toplamGun = 0;
            }
        }

        private async Task CheckOverlap()
        {
            if (!selectedTuru.HasValue)
            {
                showTurError = true;
                return;
            }

            // Validasyon
            if (request.Turu == IzinMazeretTuru.Mazeret)
            {
                if (!request.MazeretTarihi.HasValue || string.IsNullOrWhiteSpace(request.SaatDilimi))
                {
                    await ShowToast("warning", "Mazeret tarihi ve saat dilimi zorunludur");
                    return;
                }
            }
            else
            {
                if (!request.BaslangicTarihi.HasValue || !request.BitisTarihi.HasValue)
                {
                    await ShowToast("warning", "Başlangıç ve bitiş tarihi zorunludur");
                    return;
                }
            }

            try
            {
                isChecking = true;
                overlapWarning = string.Empty;

                var response = await HttpClient.PostAsJsonAsync("/api/izin-mazeret-talep/check-overlap", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<OverlapCheckResponse>();

                    if (result?.HasOverlap == true)
                    {
                        overlapWarning = result.Message ?? "Bu tarih aralığında çakışma var!";
                        await ShowToast("warning", "⚠️ Çakışma tespit edildi");
                    }
                    else
                    {
                        await ShowToast("success", "✅ Çakışma yok, talep oluşturabilirsiniz");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Çakışma kontrolü sırasında hata oluştu");
                await ShowToast("error", "Çakışma kontrolü yapılamadı");
            }
            finally
            {
                isChecking = false;
            }
        }

        private async Task HandleSubmit()
        {
            if (!selectedTuru.HasValue)
            {
                showTurError = true;
                return;
            }

            try
            {
                isSaving = true;

                var response = await HttpClient.PostAsJsonAsync("/api/izin-mazeret-talep", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                    if (result?.Success == true)
                    {
                        await ShowToast("success", "✅ Talep başarıyla oluşturuldu");
                        await Task.Delay(500);
                        NavigationManager.NavigateTo("/pdks/izin/taleplerim");
                    }
                    else
                    {
                        await ShowToast("error", result?.Message ?? "Talep oluşturulamadı");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Logger.LogWarning("Talep oluşturulamadı: {Error}", errorContent);
                    await ShowToast("error", "Talep oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Talep oluşturulurken hata oluştu");
                await ShowToast("error", "Talep oluşturulurken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
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
                request.BelgeEki = $"data:{file.ContentType};base64,{base64}";

                await ShowToast("success", "Belge yüklendi");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Dosya yüklenirken hata oluştu");
                await ShowToast("error", "Dosya yüklenemedi");
            }
        }

        private async Task ShowToast(string type, string message)
        {
            try
            {
                await JS.InvokeVoidAsync("showToast", type, message);
            }
            catch
            {
                // Toast gösterilemezse sessizce devam et
            }
        }

        // Helper classes
        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Message { get; set; }
        }

        private class OverlapCheckResponse
        {
            public bool HasOverlap { get; set; }
            public string? Message { get; set; }
        }
    }
}
