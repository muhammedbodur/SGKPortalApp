using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Mesai
{
    public partial class PerAbst
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private ILogger<PerAbst> Logger { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery(Name = "tc")]
        public string? TcKimlikNo { get; set; }

        private List<DevamsizlikListDto> list = new();
        private bool isLoading = false;
        private bool isSaving = false;
        private bool showModal = false;

        private DateTime? filterBaslangic = DateTime.Now.AddDays(-30);
        private DateTime? filterBitis = DateTime.Now;
        private bool sadeceOnayBekleyenler = false;

        private DevamsizlikCreateDto newKayit = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // TODO: Get from user claims
            if (string.IsNullOrEmpty(TcKimlikNo))
                TcKimlikNo = "12345678901";

            newKayit.TcKimlikNo = TcKimlikNo;

            await LoadList();
        }

        private async Task LoadList()
        {
            try
            {
                isLoading = true;
                list.Clear();

                var filter = new DevamsizlikFilterDto
                {
                    TcKimlikNo = TcKimlikNo,
                    BaslangicTarihi = filterBaslangic,
                    BitisTarihi = filterBitis,
                    SadeceOnayBekleyenler = sadeceOnayBekleyenler
                };

                var response = await HttpClient.PostAsJsonAsync("/api/devamsizlik/liste", filter);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DevamsizlikListDto>>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        list = result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Devamsızlık listesi yüklenirken hata");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ShowCreateModal()
        {
            newKayit = new DevamsizlikCreateDto
            {
                TcKimlikNo = TcKimlikNo!,
                Turu = IzinMazeretTuru.Mazeret
            };
            showModal = true;
        }

        private void CloseModal()
        {
            showModal = false;
            newKayit = new DevamsizlikCreateDto { TcKimlikNo = TcKimlikNo! };
        }

        private async Task CreateKayit()
        {
            try
            {
                isSaving = true;

                var response = await HttpClient.PostAsJsonAsync("/api/devamsizlik", newKayit);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

                    if (result?.Success == true)
                    {
                        Logger.LogInformation("Devamsızlık kaydı oluşturuldu: {Id}", result.Data);
                        CloseModal();
                        await LoadList();
                    }
                    else
                    {
                        Logger.LogWarning("Devamsızlık oluşturulamadı: {Message}", result?.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Devamsızlık oluştururken hata");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task OnaylaKayit(int id)
        {
            try
            {
                // TODO: Get current user sicil from claims
                int currentUserSicil = 12345;

                var response = await HttpClient.PutAsJsonAsync($"/api/devamsizlik/{id}/onayla", currentUserSicil);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();

                    if (result?.Success == true)
                    {
                        Logger.LogInformation("Devamsızlık onaylandı: {Id}", id);
                        await LoadList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Devamsızlık onaylanırken hata: {Id}", id);
            }
        }

        private async Task SilKayit(int id)
        {
            try
            {
                var response = await HttpClient.DeleteAsync($"/api/devamsizlik/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();

                    if (result?.Success == true)
                    {
                        Logger.LogInformation("Devamsızlık silindi: {Id}", id);
                        await LoadList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Devamsızlık silinirken hata: {Id}", id);
            }
        }

        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Message { get; set; }
        }
    }
}
