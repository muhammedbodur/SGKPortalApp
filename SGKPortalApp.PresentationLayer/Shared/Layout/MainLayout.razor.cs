using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Shared.Layout
{
    public partial class MainLayout
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private List<SiraCagirmaResponseDto> siraListesi = new();
        private bool siraPanelAcik = false;

        protected override void OnInitialized()
        {
            // TODO: Gerçek uygulamada service/SignalR'dan gelecek
            OrnekSiraVerileriYukle();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(500);

                try
                {
                    await JS.InvokeVoidAsync("initSneatMenu");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MainLayout JS hatası: {ex.Message}");
                }
            }
        }

        private void OrnekSiraVerileriYukle()
        {
            // Örnek veri - Gerçek uygulamada service'den gelecek
            siraListesi = new List<SiraCagirmaResponseDto>
            {
                new() { SiraId = 1, SiraNo = 1, KanalAltAdi = "Emeklilik İşlemleri", BeklemeDurum = BeklemeDurum.Beklemede, SiraAlisZamani = DateTime.Now, HizmetBinasiId = 1, HizmetBinasiAdi = "İzmir SGK" },
                new() { SiraId = 2, SiraNo = 2, KanalAltAdi = "SGK Kayıt", BeklemeDurum = BeklemeDurum.Beklemede, SiraAlisZamani = DateTime.Now, HizmetBinasiId = 1, HizmetBinasiAdi = "İzmir SGK" },
                new() { SiraId = 3, SiraNo = 3, KanalAltAdi = "Sağlık Raporu", BeklemeDurum = BeklemeDurum.Beklemede, SiraAlisZamani = DateTime.Now, HizmetBinasiId = 1, HizmetBinasiAdi = "İzmir SGK" },
                new() { SiraId = 4, SiraNo = 4, KanalAltAdi = "Borç Sorgulama", BeklemeDurum = BeklemeDurum.Beklemede, SiraAlisZamani = DateTime.Now, HizmetBinasiId = 1, HizmetBinasiAdi = "İzmir SGK" }
            };
        }

        private void HandleSiraCagir(int siraId)
        {
            // Sırayı çağır
            var sira = siraListesi.FirstOrDefault(x => x.SiraId == siraId);
            if (sira != null)
            {
                // Önceki çağrılmış sırayı beklemede yap
                var oncekiCagrilan = siraListesi.FirstOrDefault(x => x.BeklemeDurum == BeklemeDurum.Cagrildi);
                if (oncekiCagrilan != null)
                {
                    oncekiCagrilan.BeklemeDurum = BeklemeDurum.Beklemede;
                }

                // Yeni sırayı çağır
                sira.BeklemeDurum = BeklemeDurum.Cagrildi;
                sira.IslemBaslamaZamani = DateTime.Now;

                // TODO: Gerçek uygulamada API'ye çağrı yapılacak
                // await SiraService.CagirAsync(siraId);

                StateHasChanged();
            }
        }

        private void HandlePanelStateChanged(bool isVisible)
        {
            siraPanelAcik = isVisible;
            StateHasChanged();
        }
    }
}
