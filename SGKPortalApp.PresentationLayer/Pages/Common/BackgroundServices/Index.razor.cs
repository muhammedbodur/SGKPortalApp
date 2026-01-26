using Microsoft.AspNetCore.Components;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Pages.Common.BackgroundServices
{
    public partial class Index
    {
        [Inject] private IBackgroundServiceApiService BackgroundServiceApi { get; set; } = default!;
        [Inject] private ILogger<Index> Logger { get; set; } = default!;

        private List<BackgroundServiceStatusDto> Services { get; set; } = new();
        private bool IsLoading { get; set; }
        private bool IsProcessing { get; set; }
        private BackgroundServiceStatusDto? SelectedService { get; set; }
        private int NewIntervalMinutes { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadServices();
        }

        private async Task LoadServices()
        {
            IsLoading = true;
            try
            {
                var result = await BackgroundServiceApi.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    Services = result.Data;
                }
                else
                {
                    Logger.LogWarning("Background servisler yüklenemedi: {Message}", result.Message);
                    Services = new List<BackgroundServiceStatusDto>();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Background servisler yüklenirken hata oluştu");
                Services = new List<BackgroundServiceStatusDto>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RefreshServices()
        {
            await LoadServices();
        }

        private async Task TriggerService(string serviceName)
        {
            IsProcessing = true;
            try
            {
                var result = await BackgroundServiceApi.TriggerAsync(serviceName);
                if (result.Success)
                {
                    Logger.LogInformation("Servis tetiklendi: {ServiceName}", serviceName);
                    await Task.Delay(1000);
                    await LoadServices();
                }
                else
                {
                    Logger.LogWarning("Servis tetiklenemedi: {ServiceName} - {Message}", serviceName, result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Servis tetiklenirken hata oluştu: {ServiceName}", serviceName);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task PauseService(string serviceName)
        {
            IsProcessing = true;
            try
            {
                var result = await BackgroundServiceApi.PauseAsync(serviceName);
                if (result.Success)
                {
                    Logger.LogInformation("Servis duraklatıldı: {ServiceName}", serviceName);
                    await LoadServices();
                }
                else
                {
                    Logger.LogWarning("Servis duraklatılamadı: {ServiceName} - {Message}", serviceName, result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Servis duraklatılırken hata oluştu: {ServiceName}", serviceName);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task ResumeService(string serviceName)
        {
            IsProcessing = true;
            try
            {
                var result = await BackgroundServiceApi.ResumeAsync(serviceName);
                if (result.Success)
                {
                    Logger.LogInformation("Servis devam ettiriliyor: {ServiceName}", serviceName);
                    await LoadServices();
                }
                else
                {
                    Logger.LogWarning("Servis devam ettirilemedi: {ServiceName} - {Message}", serviceName, result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Servis devam ettirilirken hata oluştu: {ServiceName}", serviceName);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void ShowIntervalModal(BackgroundServiceStatusDto service)
        {
            SelectedService = service;
            NewIntervalMinutes = (int)service.Interval.TotalMinutes;
        }

        private void CloseIntervalModal()
        {
            SelectedService = null;
        }

        private async Task UpdateInterval()
        {
            if (SelectedService == null || NewIntervalMinutes < 1)
                return;

            IsProcessing = true;
            try
            {
                var result = await BackgroundServiceApi.SetIntervalAsync(SelectedService.ServiceName, NewIntervalMinutes);
                
                if (result.Success)
                {
                    Logger.LogInformation("Servis aralığı değiştirildi: {ServiceName} -> {Minutes} dakika", 
                        SelectedService.ServiceName, NewIntervalMinutes);
                    CloseIntervalModal();
                    await LoadServices();
                }
                else
                {
                    Logger.LogWarning("Servis aralığı değiştirilemedi: {ServiceName} - {Message}", 
                        SelectedService.ServiceName, result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Servis aralığı değiştirilirken hata oluştu: {ServiceName}", SelectedService.ServiceName);
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
