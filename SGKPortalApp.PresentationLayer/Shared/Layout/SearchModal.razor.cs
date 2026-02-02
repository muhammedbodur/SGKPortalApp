using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Elasticsearch;
using SGKPortalApp.PresentationLayer.Services.StateServices;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SGKPortalApp.PresentationLayer.Shared.Layout
{
    public partial class SearchModal : IDisposable
    {
        [Inject] private IPersonelSearchApiService PersonelSearchApiService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private PermissionStateService PermissionState { get; set; } = default!;

        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }

        private ElementReference searchInput;
        private string _searchTerm = string.Empty;
        private string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnSearchTermChanged();
            }
        }
        private bool IsSearching { get; set; } = false;
        private List<PersonelSearchResult> PersonelResults { get; set; } = new();
        private System.Threading.Timer? _debounceTimer;

        private List<SearchItem> AllSearchItems = new();
        private List<SearchItem> PopularSearches = new();
        private List<SearchItem> QuickAccess = new();

        private List<SearchItem> FilteredResults
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchTerm))
                    return new List<SearchItem>();

                return AllSearchItems
                                    .Where(x => HasPermission(x.PermissionKey) &&
                                               (x.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                (x.Category?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)))
                                    .Take(10)
                                    .ToList();
            }
        }

        private bool HasPermission(string? permissionKey)
        {
            if (string.IsNullOrWhiteSpace(permissionKey))
                return true;

            var level = PermissionState.GetLevel(permissionKey);
            return level >= YetkiSeviyesi.View;
        }

        protected string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        private void OnSearchTermChanged()
        {
            _debounceTimer?.Dispose();
            _debounceTimer = new System.Threading.Timer(async _ =>
            {
                try
                {
                    await InvokeAsync(async () =>
                    {
                        if (!string.IsNullOrWhiteSpace(_searchTerm) && _searchTerm.Length >= 2)
                        {
                            await SearchPersonel();
                        }
                        else
                        {
                            PersonelResults.Clear();
                            StateHasChanged();
                        }
                    });
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception)
                {
                }
            }, null, 300, Timeout.Infinite);
        }

        private async Task SearchPersonel()
        {
            try
            {
                IsSearching = true;
                // Elasticsearch fuzzy search - Türkçe karakter toleranslı
                var result = await PersonelSearchApiService.SearchAsync(SearchTerm, sadeceAktif: false, size: 20);

                if (result.Success && result.Data != null)
                {
                    PersonelResults = result.Data.Select(p => new PersonelSearchResult
                    {
                        TcKimlikNo = p.TcKimlikNo,
                        Resim = p.Resim,
                        AdSoyad = p.AdSoyad,
                        SicilNo = p.SicilNo,
                        DepartmanAdi = p.DepartmanAdi,
                        UnvanAdi = p.UnvanAdi,
                        PersonelAktiflikDurum = p.PersonelAktiflikDurum
                    }).ToList();
                    Console.WriteLine($"✅ Elasticsearch arama başarılı: {PersonelResults.Count} sonuç");
                }
                else
                {
                    Console.WriteLine($"⚠️ Elasticsearch arama başarısız: {result.Message}");
                    PersonelResults.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Elasticsearch arama hatası: {ex.Message}");
                PersonelResults.Clear();
            }
            finally
            {
                IsSearching = false;
                StateHasChanged();
            }
        }

        protected override void OnInitialized()
        {
            InitializeSearchItems();
        }

        private void InitializeSearchItems()
        {
            AllSearchItems = new List<SearchItem>
            {
                new SearchItem { Title = "Personel Listesi", Url = "/personel", Icon = "bx bx-user", Category = "Personel İşlemleri", PermissionKey = "PER.PERSONEL.INDEX" },
                new SearchItem { Title = "Yeni Personel Ekle", Url = "/personel/add", Icon = "bx bx-user-plus", Category = "Personel İşlemleri", PermissionKey = "PER.PERSONEL.CREATE" },
                new SearchItem { Title = "Departmanlar", Url = "/personel/departman", Icon = "bx bx-git-branch", Category = "Personel İşlemleri", PermissionKey = "PER.DEPARTMAN.INDEX" },
                new SearchItem { Title = "Servisler", Url = "/personel/servis", Icon = "bx bx-group", Category = "Personel İşlemleri", PermissionKey = "PER.SERVIS.INDEX" },
                new SearchItem { Title = "Ünvanlar", Url = "/personel/unvan", Icon = "bx bx-id-card", Category = "Personel İşlemleri", PermissionKey = "PER.UNVAN.INDEX" },
                new SearchItem { Title = "Sendikalar", Url = "/personel/sendika", Icon = "bx bx-unite", Category = "Personel İşlemleri", PermissionKey = "PER.SENDIKA.INDEX" },
                new SearchItem { Title = "Atanma Nedenleri", Url = "/personel/atanma-nedeni", Icon = "bx bx-file", Category = "Personel İşlemleri", PermissionKey = "PER.ATANMANEDENI.INDEX" },
                new SearchItem { Title = "Hizmet Binaları", Url = "/common/hizmetbinasi", Icon = "bx bx-building", Category = "Ortak İşlemler", PermissionKey = "COM.HIZMETBINASI.INDEX" },
                new SearchItem { Title = "Departman-Bina Eşleştirme", Url = "/common/departman-hizmet-binasi", Icon = "bx bx-link", Category = "Ortak İşlemler", PermissionKey = "COM.DEPARTMANHIZMETBINASI.INDEX" },
                new SearchItem { Title = "İzin Taleplerim", Url = "/pdks/izin/taleplerim", Icon = "bx bx-list-ul", Category = "PDKS İşlemleri", PermissionKey = "PDKS.IZIN.TALEPLERIM" },
                new SearchItem { Title = "Yeni İzin Talebi", Url = "/pdks/izin/yeni-talep", Icon = "bx bx-plus-circle", Category = "PDKS İşlemleri", PermissionKey = "PDKS.IZIN.CREATE" },
                new SearchItem { Title = "İzin Onay Bekleyenler", Url = "/pdks/izin/onay-bekleyenler", Icon = "bx bx-check-circle", Category = "PDKS İşlemleri", PermissionKey = "PDKS.IZIN.ONAY" },
                new SearchItem { Title = "İzin Takip", Url = "/pdks/izin/takip", Icon = "bx bx-list-check", Category = "PDKS İşlemleri", PermissionKey = "PDKS.IZIN.TAKIP" },
                new SearchItem { Title = "İzin Türü Tanımları", Url = "/pdks/izin/tur-tanimlari", Icon = "bx bx-list-ul", Category = "PDKS İşlemleri", PermissionKey = "PDKS.IZINTUR.INDEX" },
                new SearchItem { Title = "Mesai Listeleme", Url = "/pdks/mesai/listele", Icon = "bx bx-list-ul", Category = "PDKS İşlemleri", PermissionKey = "PDKS.MESAI.INDEX" },
                new SearchItem { Title = "Mesai Personel Listesi", Url = "/pdks/mesai/personel-listesi", Icon = "bx bx-user-check", Category = "PDKS İşlemleri", PermissionKey = "PDKS.MESAI.PERSONEL" },
                new SearchItem { Title = "Departman Mesai Raporu", Url = "/pdks/mesai/departman-rapor", Icon = "bx bx-bar-chart", Category = "PDKS İşlemleri", PermissionKey = "PDKS.MESAI.DEPARTMAN" },
                new SearchItem { Title = "Giriş/Çıkış", Url = "/pdks/giris-cikis", Icon = "bx bx-log-in", Category = "PDKS İşlemleri", PermissionKey = "PDKS.GIRISCIKIS.INDEX" },
                new SearchItem { Title = "Realtime İzleme", Url = "/pdks/giris-cikis/realtime", Icon = "bx bx-broadcast", Category = "PDKS İşlemleri", PermissionKey = "PDKS.GIRISCIKIS.REALTIME" },
                new SearchItem { Title = "Cihaz Yönetimi", Url = "/pdks/zkteco/devices", Icon = "bx bx-devices", Category = "PDKS İşlemleri", PermissionKey = "PDKS.CIHAZ.INDEX" },
                new SearchItem { Title = "Anlık Attendance Kayıtları", Url = "/pdks/zkteco/instant-attendance", Icon = "bx bx-list-check", Category = "PDKS İşlemleri", PermissionKey = "PDKS.ATTENDANCE.INDEX" },
                new SearchItem { Title = "Banko Listesi", Url = "/siramatik/banko", Icon = "bx bx-store", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.BANKO.INDEX" },
                new SearchItem { Title = "Yeni Banko Ekle", Url = "/siramatik/banko/manage", Icon = "bx bx-plus-circle", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.BANKO.CREATE" },
                new SearchItem { Title = "Kanallar", Url = "/siramatik/kanal", Icon = "bx bx-git-branch", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.KANAL.INDEX" },
                new SearchItem { Title = "Alt Kanallar", Url = "/siramatik/alt-kanal", Icon = "bx bx-subdirectory-right", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.KANALALT.INDEX" },
                new SearchItem { Title = "Kanal İşlemleri", Url = "/siramatik/kanal-islem", Icon = "bx bx-cog", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.KANALISLEM.INDEX" },
                new SearchItem { Title = "Alt Kanal İşlemleri", Url = "/siramatik/kanal-alt-islem", Icon = "bx bx-list-ul", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.KANALALTISLEM.INDEX" },
                new SearchItem { Title = "Personel Atama", Url = "/siramatik/personel-atama", Icon = "bx bx-user-check", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.KANALPERSONEL.INDEX" },
                new SearchItem { Title = "Kiosk Menüleri", Url = "/siramatik/kiosk-menu", Icon = "bx bx-menu", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.KIOSKMENU.INDEX" },
                new SearchItem { Title = "Kiosk Cihazları", Url = "/siramatik/kiosk-yonetimi", Icon = "bx bx-devices", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.KIOSK.INDEX" },
                new SearchItem { Title = "Kiosk Simülatör", Url = "/siramatik/kiosk-simulator", Icon = "bx bx-terminal", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.KIOSK.SIMULATOR" },
                new SearchItem { Title = "TV Listesi", Url = "/siramatik/tv", Icon = "bx bx-tv", Category = "Sıramatik İşlemleri", PermissionKey = "SIR.TV.INDEX" },
                new SearchItem { Title = "Yetki Atama", Url = "/yetki-atama", Icon = "bx bx-user-check", Category = "Yetki Yönetimi", PermissionKey = "YET.PERSONELYETKI.INDEX" },
                new SearchItem { Title = "Modüller", Url = "/yetki-modul", Icon = "bx bx-folder", Category = "Yetki Yönetimi", PermissionKey = "YET.MODUL.INDEX" },
                new SearchItem { Title = "Controller'lar", Url = "/yetki-controller", Icon = "bx bx-folder-open", Category = "Yetki Yönetimi", PermissionKey = "YET.CONTROLLER.INDEX" },
                new SearchItem { Title = "İşlemler", Url = "/yetki-islem", Icon = "bx bx-list-ul", Category = "Yetki Yönetimi", PermissionKey = "YET.ISLEM.INDEX" },
                new SearchItem { Title = "Denetim Logları", Url = "/audit/log", Icon = "bx bx-detail", Category = "Audit", PermissionKey = "AUD.AUDITLOG.INDEX" },
                new SearchItem { Title = "Giriş/Çıkış Logları", Url = "/audit/login-logout", Icon = "bx bx-log-in", Category = "Audit", PermissionKey = "AUD.LOGINLOGOUT.INDEX" },
                new SearchItem { Title = "İller", Url = "/common/il", Icon = "bx bx-map", Category = "Ortak İşlemler", PermissionKey = "COM.IL.INDEX" },
                new SearchItem { Title = "İlçeler", Url = "/common/ilce", Icon = "bx bx-map-pin", Category = "Ortak İşlemler", PermissionKey = "COM.ILCE.INDEX" },
                new SearchItem { Title = "Resmi Tatiller", Url = "/common/resmitatil", Icon = "bx bx-calendar", Category = "Ortak İşlemler", PermissionKey = "COM.RESMITATIL.INDEX" },
                new SearchItem { Title = "Background Servisler", Url = "/common/background-services", Icon = "bx bx-server", Category = "Ortak İşlemler", PermissionKey = "COM.BACKGROUNDSERVICE.INDEX" },
                new SearchItem { Title = "Profilim", Url = "/account/profile", Icon = "bx bx-user-circle", Category = "Hesap" },
                new SearchItem { Title = "Şifre Değiştir", Url = "/account/change-password", Icon = "bx bx-lock", Category = "Hesap" },
            };

            var popularItems = new List<SearchItem>
            {
                new SearchItem { Title = "Personel Listesi", Url = "/personel", Icon = "bx bx-user", PermissionKey = "PER.PERSONEL.INDEX" },
                new SearchItem { Title = "Yeni İzin Talebi", Url = "/pdks/izin/yeni-talep", Icon = "bx bx-plus-circle", PermissionKey = "PDKS.IZIN.CREATE" },
                new SearchItem { Title = "Banko Listesi", Url = "/siramatik/banko", Icon = "bx bx-store", PermissionKey = "SIR.BANKO.INDEX" },
                new SearchItem { Title = "Departmanlar", Url = "/personel/departman", Icon = "bx bx-git-branch", PermissionKey = "PER.DEPARTMAN.INDEX" },
            };
            PopularSearches = popularItems.Where(x => HasPermission(x.PermissionKey)).ToList();

            var quickAccessItems = new List<SearchItem>
            {
                new SearchItem { Title = "Profilim", Url = "/account/profile", Icon = "bx bx-user-circle" },
                new SearchItem { Title = "Şifre Değiştir", Url = "/account/change-password", Icon = "bx bx-lock" },
                new SearchItem { Title = "İzin Taleplerim", Url = "/pdks/izin/taleplerim", Icon = "bx bx-calendar", PermissionKey = "PDKS.IZIN.TALEPLERIM" },
                new SearchItem { Title = "Mesai Listele", Url = "/pdks/mesai/listele", Icon = "bx bx-time", PermissionKey = "PDKS.MESAI.INDEX" },
            };
            QuickAccess = quickAccessItems.Where(x => HasPermission(x.PermissionKey)).ToList();
        }

        private string HighlightMatch(string text, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return text;

            var index = text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                return text;

            var before = text.Substring(0, index);
            var match = text.Substring(index, searchTerm.Length);
            var after = text.Substring(index + searchTerm.Length);

            return $"{before}<mark class='bg-warning bg-opacity-25 text-dark'>{match}</mark>{after}";
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "??";

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();

            return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
        }

        private async Task NavigateAndClose(string url)
        {
            _debounceTimer?.Dispose();
            _debounceTimer = null;

            _searchTerm = string.Empty;
            PersonelResults.Clear();
            IsSearching = false;

            await OnClose.InvokeAsync();
            Navigation.NavigateTo(url);
        }

        private async Task Close()
        {
            _debounceTimer?.Dispose();
            _debounceTimer = null;

            _searchTerm = string.Empty;
            PersonelResults.Clear();
            IsSearching = false;

            await OnClose.InvokeAsync();
        }

        public void Dispose()
        {
            _debounceTimer?.Dispose();
            _debounceTimer = null;
        }
    }
}
