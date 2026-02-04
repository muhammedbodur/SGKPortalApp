using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Pages
{
    public partial class Index
    {
        [Inject] private IDashboardApiService DashboardApi { get; set; } = default!;
        [Inject] private IHaberApiService HaberApi { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        private bool IsLoading = true;

        // Haber verileri
        private List<HaberResponseDto> SliderHaberler = new();
        private List<HaberResponseDto> ListeHaberler = new();
        private List<HaberResponseDto> GenelMudurlukHaberler = new();

        // Dashboard diğer verileri
        private List<OnemliLinkResponseDto> OnemliLinkler = new();
        private GununMenusuResponseDto? GununMenusu;
        private List<BugunDoganResponseDto> BugunDoganlar = new();

        private List<QuickAccessItem> QuickAccessPrograms = new()
        {
            new QuickAccessItem { Name = "İşveren Uygulama Rehberi", Description = "Güncel Enim Ann Versions", Icon = "bx-book-reader", Color = "primary", Url = "#" },
            new QuickAccessItem { Name = "Tevkifat Kontrol", Description = "e-Rehber", Icon = "bx-check-shield", Color = "info", Url = "#" },
            new QuickAccessItem { Name = "Yetki Talep", Description = "Kobileri", Icon = "bx-key", Color = "warning", Url = "/yetki/talep" },
            new QuickAccessItem { Name = "Destek Talep", Description = "Se sin 3admin", Icon = "bx-support", Color = "success", Url = "/destek" },
            new QuickAccessItem { Name = "Ankara Görüşler", Description = "Merkez İletişim", Icon = "bx-conversation", Color = "danger", Url = "#" },
            new QuickAccessItem { Name = "Daha Fazlası", Description = "Tüm Uygulamalar", Icon = "bx-plus", Color = "secondary", Url = "#" }
        };

        protected override async Task OnInitializedAsync()
        {
            await LoadDashboardData();
        }

        private async Task LoadDashboardData()
        {
            try
            {
                IsLoading = true;

                // Slider haberleri
                var sliderResponse = await HaberApi.GetSliderHaberleriAsync(5);
                if (sliderResponse.Success && sliderResponse.Data != null)
                    SliderHaberler = sliderResponse.Data;

                // Liste haberleri
                var listeResponse = await HaberApi.GetHaberListeAsync(1, 10);
                if (listeResponse.Success && listeResponse.Data != null)
                {
                    ListeHaberler = listeResponse.Data.Items;

                    // Genel Müdürlük bölümü için badge atama
                    GenelMudurlukHaberler = listeResponse.Data.Items.Take(4).Select((h, i) =>
                    {
                        h.BadgeColor = GetBadgeColor(i);
                        h.BadgeText = GetBadgeText(i);
                        return h;
                    }).ToList();
                }

                // Dashboard diğer verileri (önemli linkler, menü, doğanlar)
                var dashResponse = await DashboardApi.GetDashboardDataAsync();
                if (dashResponse.Success && dashResponse.Data != null)
                {
                    OnemliLinkler = dashResponse.Data.OnemliLinkler ?? new();
                    GununMenusu = dashResponse.Data.GununMenusu;
                    BugunDoganlar = dashResponse.Data.BugunDoganlar ?? new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dashboard veri yükleme hatası: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private List<string> GetMenuItems()
        {
            if (GununMenusu?.Icerik == null) return new();
            return GununMenusu.Icerik
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Take(5)
                .ToList();
        }

        private string GetBadgeColor(int index) => index switch
        {
            0 => "info",
            1 => "warning",
            2 => "success",
            _ => "primary"
        };

        private string GetBadgeText(int index) => index switch
        {
            0 => "Mavi",
            1 => "Turuncu",
            2 => "Yeşil",
            _ => "Genel"
        };

        private void NavigateToPersonel(string tcKimlikNo)
        {
            Navigation.NavigateTo($"/personel/detail/{tcKimlikNo}");
        }

        private class QuickAccessItem
        {
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public string Icon { get; set; } = "";
            public string Color { get; set; } = "";
            public string Url { get; set; } = "#";
        }
    }
}
