using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Elasticsearch;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Elasticsearch;

namespace SGKPortalApp.PresentationLayer.Pages.Yetki.ElasticSearch
{
    public partial class ElasticsearchAdmin
    {
        [Inject] private IElasticsearchAdminApiService ApiService { get; set; } = default!;
        [Inject] private ILogger<ElasticsearchAdmin> Logger { get; set; } = default!;

        private IndexStatusInfo? Status { get; set; }
        private bool IsLoadingStatus { get; set; } = false;
        private bool IsProcessing { get; set; } = false;
        private string? CurrentOperation { get; set; }
        private string? OperationMessage { get; set; }
        private bool OperationSuccess { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await RefreshStatus();
        }

        private async Task RefreshStatus()
        {
            IsLoadingStatus = true;
            OperationMessage = null;
            StateHasChanged();

            try
            {
                var result = await ApiService.GetStatusAsync();
                if (result.Success && result.Data != null)
                {
                    Status = result.Data;
                }
                else
                {
                    Logger.LogError("Status alınamadı: {Message}", result.Message);
                    OperationSuccess = false;
                    OperationMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Status yenileme hatası");
                OperationSuccess = false;
                OperationMessage = $"Hata: {ex.Message}";
            }
            finally
            {
                IsLoadingStatus = false;
                StateHasChanged();
            }
        }

        private async Task CreateIndex()
        {
            IsProcessing = true;
            CurrentOperation = "create";
            OperationMessage = null;
            StateHasChanged();

            try
            {
                var result = await ApiService.CreateIndexAsync();

                if (result.Success)
                {
                    OperationSuccess = true;
                    OperationMessage = "✅ Index başarıyla oluşturuldu!";
                    await RefreshStatus();
                }
                else
                {
                    OperationSuccess = false;
                    OperationMessage = $"❌ {result.Message}";
                }
            }
            catch (Exception ex)
            {
                OperationSuccess = false;
                OperationMessage = $"❌ Hata: {ex.Message}";
                Logger.LogError(ex, "Index oluşturma hatası");
            }
            finally
            {
                IsProcessing = false;
                CurrentOperation = null;
                StateHasChanged();
            }
        }

        private async Task FullReindex()
        {
            IsProcessing = true;
            CurrentOperation = "reindex";
            OperationMessage = "⏳ Tam reindex başlatılıyor... Bu işlem uzun sürebilir.";
            StateHasChanged();

            try
            {
                var result = await ApiService.FullReindexAsync();

                if (result.Success)
                {
                    OperationSuccess = true;
                    OperationMessage = $"✅ Tam reindex tamamlandı! {result.Data} personel indexlendi.";
                    await RefreshStatus();
                }
                else
                {
                    OperationSuccess = false;
                    OperationMessage = $"❌ {result.Message}";
                }
            }
            catch (Exception ex)
            {
                OperationSuccess = false;
                OperationMessage = $"❌ Hata: {ex.Message}";
                Logger.LogError(ex, "Full reindex hatası");
            }
            finally
            {
                IsProcessing = false;
                CurrentOperation = null;
                StateHasChanged();
            }
        }

        private async Task IncrementalSync()
        {
            IsProcessing = true;
            CurrentOperation = "sync";
            OperationMessage = "⏳ Senkronizasyon başlatılıyor...";
            StateHasChanged();

            try
            {
                var result = await ApiService.IncrementalSyncAsync(24);

                if (result.Success)
                {
                    OperationSuccess = true;
                    OperationMessage = $"✅ Senkronizasyon tamamlandı! {result.Data} personel güncellendi.";
                    await RefreshStatus();
                }
                else
                {
                    OperationSuccess = false;
                    OperationMessage = $"❌ {result.Message}";
                }
            }
            catch (Exception ex)
            {
                OperationSuccess = false;
                OperationMessage = $"❌ Hata: {ex.Message}";
                Logger.LogError(ex, "Incremental sync hatası");
            }
            finally
            {
                IsProcessing = false;
                CurrentOperation = null;
                StateHasChanged();
            }
        }
    }
}
