using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices;
using Microsoft.JSInterop;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class Taleplerim
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILogger<Taleplerim> Logger { get; set; } = default!;

        private List<IzinMazeretTalepListResponseDto> talepler = new();
        private List<IzinMazeretTalepListResponseDto> filteredTalepler = new();
        private bool isLoading = false;
        private string searchTerm = string.Empty;
        private string durumFilter = string.Empty;
        private bool showInactive = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadTalepler();
        }

        private async Task LoadTalepler()
        {
            try
            {
                isLoading = true;

                // Kullanıcı TC'sini al (User claim'den gelecek)
                var userTc = "12345678901"; // TODO: User claim'den al

                var response = await HttpClient.GetFromJsonAsync<ApiResponse<List<IzinMazeretTalepListResponseDto>>>(
                    $"/api/izin-mazeret-talep/personel/{userTc}?includeInactive={showInactive}");

                if (response?.Success == true && response.Data != null)
                {
                    talepler = response.Data;
                    FilterTalepler();
                    await ShowToast("success", "Talepler yüklendi");
                }
                else
                {
                    await ShowToast("error", response?.Message ?? "Talepler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Talepler yüklenirken hata oluştu");
                await ShowToast("error", "Talepler yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void FilterTalepler()
        {
            var query = talepler.AsEnumerable();

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t =>
                    t.TuruAdi.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.AdSoyad.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.IzinMazeretTalepId.ToString().Contains(searchTerm));
            }

            // Durum filtresi
            if (!string.IsNullOrWhiteSpace(durumFilter))
            {
                query = query.Where(t => t.GenelDurum == durumFilter);
            }

            filteredTalepler = query.ToList();
        }

        private void ViewDetails(int id)
        {
            NavigationManager.NavigateTo($"/pdks/izin/detay/{id}");
        }

        private void EditTalep(int id)
        {
            NavigationManager.NavigateTo($"/pdks/izin/duzenle/{id}");
        }

        private async Task CancelTalep(int id)
        {
            var confirmed = await JS.InvokeAsync<bool>("confirm", "Bu talebi iptal etmek istediğinizden emin misiniz?");
            if (!confirmed) return;

            try
            {
                var iptalNedeni = await JS.InvokeAsync<string>("prompt", "İptal nedeni:");
                if (string.IsNullOrWhiteSpace(iptalNedeni)) return;

                var response = await HttpClient.PostAsJsonAsync(
                    $"/api/izin-mazeret-talep/{id}/cancel",
                    iptalNedeni);

                if (response.IsSuccessStatusCode)
                {
                    await ShowToast("success", "Talep iptal edildi");
                    await LoadTalepler();
                }
                else
                {
                    await ShowToast("error", "Talep iptal edilemedi");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Talep iptal edilirken hata oluştu");
                await ShowToast("error", "Bir hata oluştu");
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
                _ => "bg-label-warning" // Beklemede
            };
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

        // API Response wrapper
        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Message { get; set; }
        }
    }
}
