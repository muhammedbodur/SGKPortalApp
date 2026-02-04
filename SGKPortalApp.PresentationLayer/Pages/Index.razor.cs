using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Pages
{
    public partial class Index
    {
        [Inject] private IDashboardApiService DashboardApi { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        private bool IsLoading = true;
        private string UserName = "Kullanýcý";
        private List<DuyuruResponseDto> SliderDuyurular = new();
        private List<DuyuruResponseDto> ListeDuyurular = new();
        private List<DuyuruResponseDto> GenelMudurlukDuyurulari = new();
        private List<SikKullanilanProgramResponseDto> SikKullanilanProgramlar = new();
        private List<OnemliLinkResponseDto> OnemliLinkler = new();
        private GununMenusuResponseDto? GununMenusu;
        private List<BugunDoganResponseDto> BugunDoganlar = new();

        // Quick Access Programs (Static for now)
        private List<QuickAccessItem> QuickAccessPrograms = new()
        {
            new QuickAccessItem { Name = "Ýþveren Uygulama Rehberi", Description = "Güncel Enim Ann Versions", Icon = "bx-book-reader", Color = "primary", Url = "#" },
            new QuickAccessItem { Name = "Tevkifat Kontrol", Description = "e-Rehber", Icon = "bx-check-shield", Color = "info", Url = "#" },
            new QuickAccessItem { Name = "Yetki Talep", Description = "Kobileri", Icon = "bx-key", Color = "warning", Url = "/yetki/talep" },
            new QuickAccessItem { Name = "Destek Talep", Description = "Se sin 3admin", Icon = "bx-support", Color = "success", Url = "/destek" },
            new QuickAccessItem { Name = "Ankara Görüþler", Description = "Merkez Ýletiþim", Icon = "bx-conversation", Color = "danger", Url = "#" },
            new QuickAccessItem { Name = "Daha Fazlasý", Description = "Tüm Uygulamalar", Icon = "bx-plus", Color = "secondary", Url = "#" }
        };

        protected override async Task OnInitializedAsync()
        {
            await GetUserName();
            await LoadDashboardData();
        }

        private async Task GetUserName()
        {
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    UserName = user.Identity.Name ?? user.FindFirst("name")?.Value ?? "Kullanýcý";
                }
            }
            catch
            {
                UserName = "Kullanýcý";
            }
        }

        private async Task LoadDashboardData()
        {
            try
            {
                IsLoading = true;
                var response = await DashboardApi.GetDashboardDataAsync();

                if (response.Success && response.Data != null)
                {
                    SliderDuyurular = response.Data.SliderDuyurular ?? new();
                    ListeDuyurular = response.Data.ListeDuyurular ?? new();
                    GenelMudurlukDuyurulari = response.Data.ListeDuyurular?.Take(4).Select((d, i) => new DuyuruResponseDto
                    {
                        Baslik = d.Baslik,
                        YayinTarihi = d.YayinTarihi,
                        BadgeColor = GetBadgeColor(i),
                        BadgeText = GetBadgeText(i)
                    }).ToList() ?? new();
                    SikKullanilanProgramlar = response.Data.SikKullanilanProgramlar ?? new();
                    OnemliLinkler = response.Data.OnemliLinkler ?? new();
                    GununMenusu = response.Data.GununMenusu;
                    BugunDoganlar = response.Data.BugunDoganlar ?? new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dashboard veri yükleme hatasý: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private string GetFirstMenuItem()
        {
            if (GununMenusu?.Icerik == null) return "";
            var items = GununMenusu.Icerik.Split(new[] { '\n', '\r', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);
            return items.FirstOrDefault()?.Trim() ?? "";
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
            2 => "Yeþil",
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
