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
    public partial class MesaiListele
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private ILogger<MesaiListele> Logger { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery(Name = "tc")]
        public string? TcKimlikNo { get; set; }

        private PersonelMesaiBaslikDto? baslikBilgi;
        private List<PersonelMesaiListResponseDto> mesaiList = new();
        private bool isLoading = false;

        private DateTime baslangicTarihi = DateTime.Now.AddDays(-30);
        private DateTime bitisTarihi = DateTime.Now;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // TODO: User claim'den TC al
            if (string.IsNullOrEmpty(TcKimlikNo))
                TcKimlikNo = "12345678901";

            await LoadBaslikBilgi();
            await LoadMesaiList();
        }

        private async Task LoadBaslikBilgi()
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<ApiResponse<PersonelMesaiBaslikDto>>(
                    $"/api/personel-mesai/baslik/{TcKimlikNo}");

                if (response?.Success == true && response.Data != null)
                {
                    baslikBilgi = response.Data;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Başlık bilgisi yüklenirken hata");
            }
        }

        private async Task LoadMesaiList()
        {
            try
            {
                isLoading = true;
                mesaiList.Clear();

                var request = new PersonelMesaiFilterRequestDto
                {
                    TcKimlikNo = TcKimlikNo!,
                    BaslangicTarihi = baslangicTarihi,
                    BitisTarihi = bitisTarihi
                };

                var response = await HttpClient.PostAsJsonAsync("/api/personel-mesai/liste", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PersonelMesaiListResponseDto>>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        mesaiList = result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Mesai listesi yüklenirken hata");
            }
            finally
            {
                isLoading = false;
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
