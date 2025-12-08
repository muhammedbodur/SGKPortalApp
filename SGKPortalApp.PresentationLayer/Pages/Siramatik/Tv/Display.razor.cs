using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Tv
{
    public partial class Display : IAsyncDisposable
    {
        [Parameter]
        public int TvId { get; set; }

        [Inject]
        private ITvApiService _tvService { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        private List<TvSiraDto> siralar = new();
        private string hizmetBinasiAdi = "Yükleniyor...";
        private string katTipi = "";
        private string tvAciklama = "SGK'ya hoş geldiniz. Lütfen sıranızı bekleyiniz.";
        private bool isLoading = true;
        private string errorMessage = "";
        private System.Threading.Timer? _timer;
        private int _maxVisibleRows = 5; // Varsayılan değer

        protected override async Task OnInitializedAsync()
        {
            // Yetkilendirme kontrolü
            if (!await CheckAuthorizationAsync())
            {
                return; // Yetkisiz - Zaten yönlendirildi
            }

            await LoadTvDataAsync();
            StartPeriodicRefresh();
        }

        /// <summary>
        /// TV Display sayfası için yetkilendirme kontrolü
        /// Sadece TV User veya yetkili Personel girebilir
        /// </summary>
        private async Task<bool> CheckAuthorizationAsync()
        {
            try
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                // Login kontrolü
                if (!user.Identity?.IsAuthenticated ?? true)
                {
                    Console.WriteLine("❌ Kullanıcı giriş yapmamış - Login sayfasına yönlendiriliyor");
                    NavigationManager.NavigateTo("/auth/login", forceLoad: true);
                    return false;
                }

                var userType = user.FindFirst("UserType")?.Value;
                var tcKimlikNo = user.FindFirst("TcKimlikNo")?.Value;

                Console.WriteLine($"🔐 Yetki kontrolü: UserType={userType}, TcKimlikNo={tcKimlikNo}, TvId={TvId}");

                // TV User kontrolü
                if (userType == "TvUser")
                {
                    // TV User sadece kendi TV'sini görebilir
                    // TcKimlikNo formatı: TV0000001
                    if (tcKimlikNo?.StartsWith("TV") == true)
                    {
                        // TcKimlikNo'dan TvId'yi çıkar
                        var tvIdFromUser = int.Parse(tcKimlikNo.Substring(2)); // "TV0000001" -> 1
                        
                        if (tvIdFromUser == TvId)
                        {
                            Console.WriteLine($"✅ TV User yetkili: TV#{TvId}");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine($"❌ TV User yetkisiz: Kendi TV'si (#{tvIdFromUser}) değil, #{TvId} erişmeye çalışıyor");
                            NavigationManager.NavigateTo("/error/unauthorized", forceLoad: true);
                            return false;
                        }
                    }
                }

                // Personel kontrolü
                if (userType == "Personel")
                {
                    // TODO: Personel için yetki kontrolü eklenebilir
                    // Örneğin: Sadece belirli departman/rol TV'leri görebilir
                    // Şimdilik tüm personel görebilir
                    Console.WriteLine($"✅ Personel yetkili: {tcKimlikNo}");
                    return true;
                }

                // Bilinmeyen kullanıcı tipi
                Console.WriteLine($"❌ Bilinmeyen kullanıcı tipi: {userType}");
                NavigationManager.NavigateTo("/error/unauthorized", forceLoad: true);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Yetkilendirme hatası: {ex.Message}");
                NavigationManager.NavigateTo("/error/unauthorized", forceLoad: true);
                return false;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    // Ekran yüksekliğine göre maksimum sıra sayısını belirle
                    var screenHeight = await JSRuntime.InvokeAsync<int>("eval", "window.innerHeight");

                    if (screenHeight <= 720)
                    {
                        _maxVisibleRows = 4; // 720p için 4 sıra
                    }
                    else if (screenHeight <= 900)
                    {
                        _maxVisibleRows = 5; // 768p için 5 sıra
                    }
                    else if (screenHeight <= 1080)
                    {
                        _maxVisibleRows = 6; // 1080p için 6 sıra
                    }
                    else
                    {
                        _maxVisibleRows = 7; // 4K için 7 sıra
                    }

                    Console.WriteLine($"Ekran yüksekliği: {screenHeight}px, Maksimum sıra sayısı: {_maxVisibleRows}");
                    
                    // TV Display JavaScript fonksiyonlarını başlat
                    await JSRuntime.InvokeVoidAsync("tvDisplay.startClock");
                    await JSRuntime.InvokeVoidAsync("tvDisplay.startVideo");
                    
                    // ÖNCE ConnectionType'ı güncelle, SONRA TV grubuna katıl
                    await JSRuntime.InvokeVoidAsync("tvDisplay.updateConnectionTypeToTvMode");
                    await JSRuntime.InvokeVoidAsync("tvDisplay.initializeSignalR", TvId);
                    
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TV Display başlatma hatası: {ex.Message}");
                }
            }
        }

        private void StartPeriodicRefresh()
        {
            // Her 30 saniyede bir sayfayı yenile
            _timer = new System.Threading.Timer(async _ =>
            {
                try
                {
                    await InvokeAsync(async () =>
                    {
                        await LoadTvDataAsync();
                        StateHasChanged();
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Periyodik yenileme hatası: {ex.Message}");
                }
            }, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        private async Task LoadTvDataAsync()
        {
            try
            {
                isLoading = true;
                errorMessage = "";

                // TV bilgilerini getir
                var tvResult = await _tvService.GetByIdAsync(TvId);
                if (tvResult.Success && tvResult.Data != null)
                {
                    hizmetBinasiAdi = tvResult.Data.HizmetBinasiAdi ?? "";
                    katTipi = tvResult.Data.KatTipi.ToString();
                    tvAciklama = tvResult.Data.TvAciklama ?? "SGK Portal'a hoş geldiniz. Lütfen sıranızı bekleyiniz.";
                }
                else
                {
                    errorMessage = "TV bilgileri yüklenemedi";
                    Console.WriteLine($"TV bilgisi alınamadı: {tvResult.Message}");
                }

                // TV'ye ait aktif sıraları getir
                var siralarResult = await _tvService.GetActiveSiralarByTvIdAsync(TvId);
                if (siralarResult.Success && siralarResult.Data != null)
                {
                    // Sadece maksimum görüntülenebilir sayı kadar sıra al
                    // En son çağrılan sıralar en üstte olacak şekilde sırala
                    siralar = siralarResult.Data
                        .OrderByDescending(s => s.SiraNo)
                        .Take(_maxVisibleRows)
                        .ToList();

                    Console.WriteLine($"✅ TV#{TvId} için {siralarResult.Data.Count} sıra bulundu, {siralar.Count} tanesi gösteriliyor");
                }
                else
                {
                    Console.WriteLine($"⚠️ TV#{TvId} için sıra bulunamadı: {siralarResult.Message}");
                    siralar = new List<TvSiraDto>();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Hata: {ex.Message}";
                Console.WriteLine($"❌ TV data yüklenirken hata: {ex.Message}");
                siralar = new List<TvSiraDto>();
            }
            finally
            {
                isLoading = false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            _timer?.Dispose();
            await Task.CompletedTask;
        }
    }
}