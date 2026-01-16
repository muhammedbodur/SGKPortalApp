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
    public partial class PersonelMesaiList
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private ILogger<PersonelMesaiList> Logger { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private List<PersonelListResponseDto> personelList = new();
        private List<DepartmanDto> departmanList = new();

        private bool isLoading = false;
        private int? filterDepartmanId = null;
        private bool sadeceAktifler = true;
        private string? aramaMetni = null;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFilterData();
            await LoadPersonelList();
        }

        private async Task LoadFilterData()
        {
            try
            {
                // Load Departman list
                var deptResponse = await HttpClient.GetFromJsonAsync<ApiResponse<List<DepartmanDto>>>("/api/departman/liste");
                if (deptResponse?.Success == true && deptResponse.Data != null)
                {
                    departmanList = deptResponse.Data;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Filtre verileri yüklenirken hata");
            }
        }

        private async Task LoadPersonelList()
        {
            try
            {
                isLoading = true;
                personelList.Clear();

                var request = new PersonelListFilterRequestDto
                {
                    DepartmanId = filterDepartmanId,
                    SadeceAktifler = sadeceAktifler,
                    AramaMetni = aramaMetni
                };

                var response = await HttpClient.PostAsJsonAsync("/api/personel-list/liste", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PersonelListResponseDto>>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        personelList = result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Personel listesi yüklenirken hata");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task ToggleAktifDurum(PersonelListResponseDto personel)
        {
            try
            {
                var request = new PersonelAktifDurumUpdateDto
                {
                    TcKimlikNo = personel.TcKimlikNo,
                    Aktif = !personel.Aktif
                };

                var response = await HttpClient.PutAsJsonAsync("/api/personel-list/toggle-aktif", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();

                    if (result?.Success == true)
                    {
                        // Reload list to reflect changes
                        await LoadPersonelList();
                    }
                    else
                    {
                        Logger.LogWarning("Aktif durum güncelleme başarısız: {Message}", result?.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Aktif durum güncellenirken hata: {TcKimlikNo}", personel.TcKimlikNo);
            }
        }

        // Helper DTOs for filter dropdowns
        private class DepartmanDto
        {
            public int DepartmanId { get; set; }
            public string DepartmanAdi { get; set; } = string.Empty;
        }

        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Message { get; set; }
        }
    }
}
