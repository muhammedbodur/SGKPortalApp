using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.SignalR;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.Text.Json;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.SignalRLogs
{
    public partial class Index : IDisposable
    {
        [Inject] private ISignalREventLogApiService _eventLogService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        // State
        private bool isLoading = false;
        private bool autoRefresh = false;
        private bool showDetailModal = false;
        private string quickFilter = "recent5";

        // Data
        private List<SignalREventLogResponseDto> eventLogs = new();
        private SignalREventLogStatsDto? stats;
        private SignalREventLogResponseDto? selectedLog;
        private SignalREventLogFilterDto filter = new()
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Now,
            PageNumber = 1,
            PageSize = 50
        };

        // Pagination
        private int totalCount = 0;
        private int totalPages => (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        // Auto refresh timer
        private Timer? _refreshTimer;

        protected override async Task OnInitializedAsync()
        {
            await SetQuickFilter("recent5");
        }

        private async Task LoadData()
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                // Load filtered data
                var result = await _eventLogService.GetFilteredAsync(filter);
                if (result != null)
                {
                    eventLogs = result.Items;
                    totalCount = result.TotalCount;
                }

                // Load stats
                stats = await _eventLogService.GetStatsAsync(filter.StartDate, filter.EndDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event log yükleme hatası");
                await _toastService.ShowErrorAsync("Event logları yüklenirken hata oluştu");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task ApplyFilter()
        {
            filter.PageNumber = 1;
            quickFilter = "custom";
            await LoadData();
        }

        private async Task SetQuickFilter(string filterType)
        {
            quickFilter = filterType;
            filter.PageNumber = 1;
            filter.EventType = null;
            filter.DeliveryStatus = null;
            filter.SiraNo = null;
            filter.CorrelationId = null;

            switch (filterType)
            {
                case "all":
                    filter.StartDate = DateTime.Today;
                    filter.EndDate = DateTime.Now;
                    break;
                case "recent5":
                    filter.StartDate = DateTime.Now.AddMinutes(-5);
                    filter.EndDate = DateTime.Now;
                    break;
                case "recent15":
                    filter.StartDate = DateTime.Now.AddMinutes(-15);
                    filter.EndDate = DateTime.Now;
                    break;
                case "recent60":
                    filter.StartDate = DateTime.Now.AddHours(-1);
                    filter.EndDate = DateTime.Now;
                    break;
                case "failed":
                    filter.StartDate = DateTime.Today;
                    filter.EndDate = DateTime.Now;
                    filter.DeliveryStatus = SignalRDeliveryStatus.Failed;
                    break;
                case "notarget":
                    filter.StartDate = DateTime.Today;
                    filter.EndDate = DateTime.Now;
                    filter.DeliveryStatus = SignalRDeliveryStatus.NoTarget;
                    break;
            }

            await LoadData();
        }

        private async Task GoToPage(int page)
        {
            if (page < 1 || page > totalPages) return;
            filter.PageNumber = page;
            await LoadData();
        }

        private void OnAutoRefreshChanged()
        {
            if (autoRefresh)
            {
                _refreshTimer = new Timer(async _ =>
                {
                    await InvokeAsync(async () =>
                    {
                        filter.EndDate = DateTime.Now;
                        if (quickFilter.StartsWith("recent"))
                        {
                            var minutes = quickFilter switch
                            {
                                "recent5" => 5,
                                "recent15" => 15,
                                "recent60" => 60,
                                _ => 5
                            };
                            filter.StartDate = DateTime.Now.AddMinutes(-minutes);
                        }
                        await LoadData();
                    });
                }, null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
            }
            else
            {
                _refreshTimer?.Dispose();
                _refreshTimer = null;
            }
        }

        private void ShowDetail(SignalREventLogResponseDto log)
        {
            selectedLog = log;
            showDetailModal = true;
        }

        private void HideDetail()
        {
            showDetailModal = false;
            selectedLog = null;
        }

        private async Task FilterBySira(int siraId)
        {
            HideDetail();
            filter = new SignalREventLogFilterDto
            {
                SiraId = siraId,
                PageNumber = 1,
                PageSize = 50
            };
            quickFilter = "custom";
            await LoadData();
        }

        private async Task FilterByCorrelation(Guid correlationId)
        {
            HideDetail();
            filter = new SignalREventLogFilterDto
            {
                CorrelationId = correlationId,
                PageNumber = 1,
                PageSize = 50
            };
            quickFilter = "custom";
            await LoadData();
        }

        // Helper methods
        private static string GetEventTypeName(SignalREventType eventType) => eventType switch
        {
            SignalREventType.NewSira => "Yeni Sıra",
            SignalREventType.SiraCalled => "Sıra Çağrıldı",
            SignalREventType.SiraCompleted => "Sıra Tamamlandı",
            SignalREventType.SiraCancelled => "Sıra İptal",
            SignalREventType.SiraRedirected => "Sıra Yönlendirildi",
            SignalREventType.PanelUpdate => "Panel Güncelleme",
            SignalREventType.TvUpdate => "TV Güncelleme",
            SignalREventType.BankoModeActivated => "Banko Modu Aktif",
            SignalREventType.BankoModeDeactivated => "Banko Modu Pasif",
            SignalREventType.ForceLogout => "Zorla Çıkış",
            SignalREventType.Announcement => "Duyuru",
            _ => "Diğer"
        };

        private static string GetDeliveryStatusName(SignalRDeliveryStatus status) => status switch
        {
            SignalRDeliveryStatus.Pending => "Beklemede",
            SignalRDeliveryStatus.Sent => "Gönderildi",
            SignalRDeliveryStatus.Failed => "Başarısız",
            SignalRDeliveryStatus.NoTarget => "Hedef Yok",
            SignalRDeliveryStatus.Acknowledged => "Alındı",
            _ => "Bilinmiyor"
        };

        private static string GetEventTypeBadgeClass(SignalREventType eventType) => eventType switch
        {
            SignalREventType.NewSira => "bg-success",
            SignalREventType.SiraCalled => "bg-primary",
            SignalREventType.SiraCompleted => "bg-info",
            SignalREventType.SiraCancelled => "bg-danger",
            SignalREventType.SiraRedirected => "bg-warning",
            SignalREventType.PanelUpdate => "bg-label-primary",
            SignalREventType.TvUpdate => "bg-label-info",
            SignalREventType.BankoModeActivated => "bg-label-success",
            SignalREventType.BankoModeDeactivated => "bg-label-warning",
            SignalREventType.ForceLogout => "bg-label-danger",
            SignalREventType.Announcement => "bg-label-secondary",
            _ => "bg-secondary"
        };

        private static string GetStatusBadgeClass(SignalRDeliveryStatus status) => status switch
        {
            SignalRDeliveryStatus.Pending => "bg-label-warning",
            SignalRDeliveryStatus.Sent => "bg-label-success",
            SignalRDeliveryStatus.Failed => "bg-danger",
            SignalRDeliveryStatus.NoTarget => "bg-warning",
            SignalRDeliveryStatus.Acknowledged => "bg-success",
            _ => "bg-secondary"
        };

        private static string GetRowClass(SignalREventLogResponseDto log) => log.DeliveryStatus switch
        {
            SignalRDeliveryStatus.Failed => "table-danger",
            SignalRDeliveryStatus.NoTarget => "table-warning",
            _ => ""
        };

        private static string FormatJson(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return json;
            }
        }

        public void Dispose()
        {
            _refreshTimer?.Dispose();
        }
    }
}
