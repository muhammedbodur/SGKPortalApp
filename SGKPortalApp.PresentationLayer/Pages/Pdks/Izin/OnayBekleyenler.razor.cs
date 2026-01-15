using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class OnayBekleyenler
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ILogger<OnayBekleyenler> Logger { get; set; } = default!;

        private List<IzinMazeretTalepListResponseDto> talepler = new();
        private IzinMazeretTalepListResponseDto? selectedTalep;
        private bool isLoading = false;
        private bool showApprovalModal = false;
        private bool showDetailModal = false;
        private bool isApproving = false;
        private string onayNotu = string.Empty;

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
                talepler.Clear();

                // TODO: Kullanıcı TC'sini claim'den al
                var onayciTcKimlikNo = "12345678901";

                // Hem 1. onay hem 2. onay için bekleyen talepleri getir
                var firstApproverResponse = await HttpClient.GetFromJsonAsync<ApiResponse<List<IzinMazeretTalepListResponseDto>>>(
                    $"/api/izin-mazeret-talep/pending/first-approver/{onayciTcKimlikNo}");

                var secondApproverResponse = await HttpClient.GetFromJsonAsync<ApiResponse<List<IzinMazeretTalepListResponseDto>>>(
                    $"/api/izin-mazeret-talep/pending/second-approver/{onayciTcKimlikNo}");

                // Her iki listeden gelen talepleri birleştir
                var allTalepler = new List<IzinMazeretTalepListResponseDto>();

                if (firstApproverResponse?.Success == true && firstApproverResponse.Data != null)
                {
                    allTalepler.AddRange(firstApproverResponse.Data);
                }

                if (secondApproverResponse?.Success == true && secondApproverResponse.Data != null)
                {
                    allTalepler.AddRange(secondApproverResponse.Data);
                }

                // Talep tarihine göre sırala (en yeniden en eskiye)
                talepler = allTalepler.OrderByDescending(t => t.TalepTarihi).ToList();
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

        private void ShowApprovalModal(IzinMazeretTalepListResponseDto talep, bool approve)
        {
            selectedTalep = talep;
            isApproving = approve;
            onayNotu = string.Empty;
            showApprovalModal = true;
        }

        private void CloseApprovalModal()
        {
            showApprovalModal = false;
            selectedTalep = null;
            onayNotu = string.Empty;
        }

        private void ShowDetailModal(IzinMazeretTalepListResponseDto talep)
        {
            selectedTalep = talep;
            showDetailModal = true;
        }

        private void CloseDetailModal()
        {
            showDetailModal = false;
            selectedTalep = null;
        }

        private async Task SubmitApproval()
        {
            if (selectedTalep == null) return;

            // Red durumunda not zorunlu
            if (!isApproving && string.IsNullOrWhiteSpace(onayNotu))
            {
                await ShowToast("warning", "Red nedeni belirtmeniz zorunludur");
                return;
            }

            try
            {
                var request = new IzinMazeretTalepOnayRequestDto
                {
                    Onaylandi = isApproving,
                    OnayNotu = onayNotu
                };

                var response = await HttpClient.PostAsJsonAsync(
                    $"/api/izin-mazeret-talep/{selectedTalep.IzinMazeretTalepId}/approve",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                    if (result?.Success == true)
                    {
                        await ShowToast("success", isApproving
                            ? "✅ Talep başarıyla onaylandı"
                            : "❌ Talep reddedildi");

                        CloseApprovalModal();
                        await LoadTalepler();
                    }
                    else
                    {
                        await ShowToast("error", result?.Message ?? "İşlem başarısız");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Logger.LogWarning("Onay işlemi başarısız: {Error}", errorContent);
                    await ShowToast("error", "İşlem başarısız oldu");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Onay işlemi sırasında hata oluştu");
                await ShowToast("error", "İşlem sırasında bir hata oluştu");
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
    }
}
