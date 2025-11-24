using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Tv
{
    public partial class Display
    {
        [Parameter]
        public int TvId { get; set; }

        [Inject]
        private ITvApiService _tvService { get; set; } = default!;

        private List<TvSiraDto> siralar = new();
        private string hizmetBinasiAdi = "Yükleniyor...";
        private string katTipi = "";
        private string tvAciklama = "SGK'ya hoş geldiniz. Lütfen sıranızı bekleyiniz.";
        private bool isLoading = true;
        private string errorMessage = "";

        protected override async Task OnInitializedAsync()
        {
            await LoadTvDataAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // İlk render'dan sonra periyodik yenileme başlat
                await Task.Delay(30000); // 30 saniye
                await LoadTvDataAsync();
                StateHasChanged();
            }
        }

        private async Task LoadTvDataAsync()
        {
            try
            {
                isLoading = true;

                // TV bilgilerini getir
                var tvResult = await _tvService.GetByIdAsync(TvId);
                if (tvResult.Success && tvResult.Data != null)
                {
                    hizmetBinasiAdi = tvResult.Data.HizmetBinasiAdi ?? "";
                    katTipi = tvResult.Data.KatTipi.ToString();
                    tvAciklama = tvResult.Data.TvAciklama ?? "SGK Portal'a hoş geldiniz.";
                }

                // TV'ye ait sıraları getir
                // TODO: Sıra sistemi implement edildiğinde aktif edilecek
                // Şimdilik test verisi
                siralar = new List<TvSiraDto>
                {
                    new TvSiraDto { BankoId = 1, BankoNo = 1, KatTipi = "Zemin Kat", SiraNo = 101 },
                    new TvSiraDto { BankoId = 2, BankoNo = 2, KatTipi = "Zemin Kat", SiraNo = 102 },
                    new TvSiraDto { BankoId = 3, BankoNo = 3, KatTipi = "Zemin Kat", SiraNo = 103 },
                    new TvSiraDto { BankoId = 4, BankoNo = 4, KatTipi = "Zemin Kat", SiraNo = 104 },
                    new TvSiraDto { BankoId = 5, BankoNo = 5, KatTipi = "Zemin Kat", SiraNo = 105 }
                };
            }
            catch (Exception ex)
            {
                errorMessage = $"Hata: {ex.Message}";
                Console.WriteLine($"TV data yüklenirken hata: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }
    }

    // DTO for TV Sira display
    public class TvSiraDto
    {
        public int BankoId { get; set; }
        public int BankoNo { get; set; }
        public string KatTipi { get; set; } = string.Empty;
        public int SiraNo { get; set; }
    }
}
