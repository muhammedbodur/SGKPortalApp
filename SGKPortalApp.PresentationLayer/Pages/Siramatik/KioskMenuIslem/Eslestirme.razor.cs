using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskMenuIslem
{
    public partial class Eslestirme
    {
        [Inject]
        private IKioskMenuApiService _kioskMenuService { get; set; } = default!;

        [Inject]
        private IKioskMenuIslemApiService _kioskMenuIslemService { get; set; } = default!;

        [Inject]
        private IKanalAltIslemApiService _kanalAltIslemService { get; set; } = default!;

        [Inject]
        private IToastService _toastService { get; set; } = default!;

        [Inject]
        private NavigationManager _navigationManager { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "kioskMenuId")]
        public int? KioskMenuIdFromQuery { get; set; }

        private List<KioskMenuResponseDto> kioskMenuleri = new();
        private List<KanalAltIslemResponseDto> allKanalAltIslemler = new();
        private List<KioskMenuIslemResponseDto> assignedIslemler = new();
        private List<KanalAltIslemResponseDto> unassignedIslemler = new();

        // Her menünün işlem sayısını tutan dictionary
        private Dictionary<int, int> menuIslemCounts = new();

        private int selectedKioskMenuId = 0;
        private string selectedMenuAdi = string.Empty;

        // Arama filtreleri
        private string searchAssigned = string.Empty;
        private string searchUnassigned = string.Empty;

        private bool isLoading = true;
        private bool isLoadingIslemler = false;
        private bool isSaving = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;

            try
            {
                // Tüm menüleri yükle
                var menuResult = await _kioskMenuService.GetAllAsync();
                if (menuResult.Success && menuResult.Data != null)
                {
                    kioskMenuleri = menuResult.Data
                        .OrderBy(m => m.MenuSira)
                        .ThenBy(m => m.MenuAdi)
                        .ToList();

                    // Her menü için işlem sayısını yükle
                    await LoadMenuIslemCountsAsync();
                }

                // Query string'den kioskMenuId geldi mi?
                if (KioskMenuIdFromQuery.HasValue && KioskMenuIdFromQuery.Value > 0)
                {
                    var menu = kioskMenuleri.FirstOrDefault(m => m.KioskMenuId == KioskMenuIdFromQuery.Value);
                    if (menu != null)
                    {
                        selectedKioskMenuId = KioskMenuIdFromQuery.Value;
                        selectedMenuAdi = menu.MenuAdi;

                        await LoadIslemlerAsync();
                    }
                }
                else if (kioskMenuleri.Any())
                {
                    // Query string yoksa, ilk menüyü seç
                    var firstMenu = kioskMenuleri.First();
                    selectedKioskMenuId = firstMenu.KioskMenuId;
                    selectedMenuAdi = firstMenu.MenuAdi;

                    await LoadIslemlerAsync();
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veri yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadMenuIslemCountsAsync()
        {
            menuIslemCounts.Clear();

            foreach (var menu in kioskMenuleri)
            {
                var islemResult = await _kioskMenuIslemService.GetByKioskMenuAsync(menu.KioskMenuId);
                if (islemResult.Success && islemResult.Data != null)
                {
                    menuIslemCounts[menu.KioskMenuId] = islemResult.Data.Count();
                }
                else
                {
                    menuIslemCounts[menu.KioskMenuId] = 0;
                }
            }
        }

        private async Task SelectMenu(int kioskMenuId)
        {
            selectedKioskMenuId = kioskMenuId;
            var menu = kioskMenuleri.FirstOrDefault(m => m.KioskMenuId == kioskMenuId);
            selectedMenuAdi = menu?.MenuAdi ?? string.Empty;

            await LoadIslemlerAsync();
        }

        private async Task LoadIslemlerAsync()
        {
            if (selectedKioskMenuId == 0) return;

            isLoadingIslemler = true;

            try
            {
                // Tüm kanal alt işlemleri yükle
                var kanalAltResult = await _kanalAltIslemService.GetAllAsync();
                if (kanalAltResult.Success && kanalAltResult.Data != null)
                {
                    allKanalAltIslemler = kanalAltResult.Data
                        .OrderBy(k => k.KanalAdi)
                        .ThenBy(k => k.KanalAltAdi)
                        .ToList();
                }

                // Bu menüye atanmış işlemleri al
                var islemResult = await _kioskMenuIslemService.GetByKioskMenuAsync(selectedKioskMenuId);
                if (islemResult.Success && islemResult.Data != null)
                {
                    assignedIslemler = islemResult.Data.ToList();

                    var assignedKanalAltIds = assignedIslemler.Select(i => i.KanalAltId).ToList();

                    unassignedIslemler = allKanalAltIslemler
                        .Where(k => !assignedKanalAltIds.Contains(k.KanalAltId))
                        .ToList();
                }
                else
                {
                    assignedIslemler.Clear();
                    unassignedIslemler = allKanalAltIslemler.ToList();
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"İşlemler yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                isLoadingIslemler = false;
            }
        }

        // Add/Remove Operations
        private async Task AddIslem(int kanalAltIslemId)
        {
            isSaving = true;

            try
            {
                var kanalAlt = unassignedIslemler.FirstOrDefault(k => k.KanalAltIslemId == kanalAltIslemId);
                if (kanalAlt == null)
                {
                    await _toastService.ShowErrorAsync("İşlem bulunamadı");
                    return;
                }

                // Yeni sıra numarası: mevcut işlemlerin max sırası + 1
                int yeniSira = assignedIslemler.Any() ? assignedIslemler.Max(i => i.MenuSira) + 1 : 1;

                var createDto = new KioskMenuIslemCreateRequestDto
                {
                    KioskMenuId = selectedKioskMenuId,
                    KanalAltId = kanalAlt.KanalAltId,
                    IslemAdi = kanalAlt.KanalAltAdi,
                    MenuSira = yeniSira
                };

                var result = await _kioskMenuIslemService.CreateAsync(createDto);

                if (result.Success && result.Data != null)
                {
                    // Yeni eklenen işlemi assignedIslemler'e ekle
                    assignedIslemler.Add(result.Data);
                    unassignedIslemler.Remove(kanalAlt);

                    // İşlem sayacını güncelle
                    if (menuIslemCounts.ContainsKey(selectedKioskMenuId))
                    {
                        menuIslemCounts[selectedKioskMenuId]++;
                    }

                    await _toastService.ShowSuccessAsync($"{kanalAlt.KanalAltAdi} işlemi eklendi");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İşlem eklenemedi");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task RemoveIslem(int kioskMenuIslemId)
        {
            isSaving = true;

            try
            {
                var result = await _kioskMenuIslemService.DeleteAsync(kioskMenuIslemId);

                if (result.Success)
                {
                    var islem = assignedIslemler.FirstOrDefault(i => i.KioskMenuIslemId == kioskMenuIslemId);
                    if (islem != null)
                    {
                        assignedIslemler.Remove(islem);

                        // İşlem sayacını güncelle
                        if (menuIslemCounts.ContainsKey(selectedKioskMenuId))
                        {
                            menuIslemCounts[selectedKioskMenuId]--;
                        }

                        // Kaldırılan işlemin kanal alt bilgisini bul ve unassigned'a ekle
                        var kanalAlt = allKanalAltIslemler.FirstOrDefault(k => k.KanalAltId == islem.KanalAltId);
                        if (kanalAlt != null)
                        {
                            unassignedIslemler.Add(kanalAlt);
                            unassignedIslemler = unassignedIslemler
                                .OrderBy(k => k.KanalAdi)
                                .ThenBy(k => k.KanalAltAdi)
                                .ToList();

                            await _toastService.ShowSuccessAsync($"{islem.KanalAltAdi} işlemi kaldırıldı");
                        }
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İşlem kaldırılamadı");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        // Arama filtreleme metodları
        private List<KioskMenuIslemResponseDto> GetFilteredAssignedIslemler()
        {
            if (string.IsNullOrWhiteSpace(searchAssigned))
                return assignedIslemler.OrderBy(i => i.MenuSira).ToList();

            return assignedIslemler
                .Where(i => i.KanalAltAdi.ContainsTurkish(searchAssigned, StringComparison.OrdinalIgnoreCase))
                .OrderBy(i => i.MenuSira)
                .ToList();
        }

        private List<KanalAltIslemResponseDto> GetFilteredUnassignedIslemler()
        {
            if (string.IsNullOrWhiteSpace(searchUnassigned))
                return unassignedIslemler;

            return unassignedIslemler
                .Where(k => k.KanalAltAdi.ContainsTurkish(searchUnassigned, StringComparison.OrdinalIgnoreCase) ||
                           k.KanalAdi.ContainsTurkish(searchUnassigned, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/kiosk-menu-islem");
        }
    }
}
