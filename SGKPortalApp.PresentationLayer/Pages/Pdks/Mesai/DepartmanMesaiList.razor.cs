using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Mesai
{
    public partial class DepartmanMesaiList
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private ILogger<DepartmanMesaiList> Logger { get; set; } = default!;

        private DepartmanMesaiReportDto? report;
        private List<DepartmanDto> departmanList = new();
        private List<ServisDto> servisList = new();
        private HashSet<string> expandedRows = new();

        private bool isLoading = false;
        private int? filterDepartmanId = null;
        private int? filterServisId = null;
        private DateTime baslangicTarihi = DateTime.Now.AddDays(-30);
        private DateTime bitisTarihi = DateTime.Now;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFilterData();
        }

        private async Task LoadFilterData()
        {
            try
            {
                // Load Departman list
                var departmanResponse = await HttpClient.GetFromJsonAsync<ApiResponse<List<DepartmanDto>>>("/api/departman/liste");
                if (departmanResponse?.Success == true && departmanResponse.Data != null)
                {
                    departmanList = departmanResponse.Data;
                }

                // Load Servis list
                var servisResponse = await HttpClient.GetFromJsonAsync<ApiResponse<List<ServisDto>>>("/api/servis/liste");
                if (servisResponse?.Success == true && servisResponse.Data != null)
                {
                    servisList = servisResponse.Data;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Filtre verileri yüklenirken hata");
            }
        }

        private async Task LoadReport()
        {
            try
            {
                if (!filterDepartmanId.HasValue)
                {
                    Logger.LogWarning("Departman seçilmeden rapor oluşturulamaz");
                    return;
                }

                isLoading = true;
                report = null;
                expandedRows.Clear();

                var request = new DepartmanMesaiFilterRequestDto
                {
                    DepartmanId = filterDepartmanId.Value,
                    ServisId = filterServisId,
                    BaslangicTarihi = baslangicTarihi,
                    BitisTarihi = bitisTarihi
                };

                var response = await HttpClient.PostAsJsonAsync("/api/departman-mesai/rapor", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<DepartmanMesaiReportDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        report = result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Departman mesai raporu yüklenirken hata");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ToggleRow(string tcKimlikNo)
        {
            if (expandedRows.Contains(tcKimlikNo))
            {
                expandedRows.Remove(tcKimlikNo);
            }
            else
            {
                expandedRows.Add(tcKimlikNo);
            }
        }

        // Helper DTOs for filter dropdowns
        private class DepartmanDto
        {
            public int DepartmanId { get; set; }
            public string DepartmanAdi { get; set; } = string.Empty;
        }

        private class ServisDto
        {
            public int ServisId { get; set; }
            public string ServisAdi { get; set; } = string.Empty;
        }

        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Message { get; set; }
        }
    }
}
