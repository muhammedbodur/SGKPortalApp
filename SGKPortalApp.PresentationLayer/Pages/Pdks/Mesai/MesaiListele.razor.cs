using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Mesai
{
    public partial class MesaiListele
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private ILogger<MesaiListele> Logger { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

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

            // Get TC from user claims
            if (string.IsNullOrEmpty(TcKimlikNo))
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                TcKimlikNo = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            if (string.IsNullOrEmpty(TcKimlikNo))
            {
                Logger.LogWarning("Kullanıcı TC kimlik no bulunamadı");
                return;
            }

            await LoadBaslikBilgi();
            await LoadMesaiList();
        }

        private async Task LoadBaslikBilgi()
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<ApiResponseDto<PersonelMesaiBaslikDto>>(
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
                    var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelMesaiListResponseDto>>>();

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
    }
}
