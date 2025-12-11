using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskMenuAtama
{
    public partial class Eslestirme
    {
        [Inject]
        private IKioskApiService _kioskService { get; set; } = default!;

        [Inject]
        private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;

        [Inject]
        private IKioskMenuApiService _kioskMenuService { get; set; } = default!;

        [Inject]
        private IKioskMenuAtamaApiService _kioskMenuAtamaService { get; set; } = default!;

        [Inject]
        private IToastService _toastService { get; set; } = default!;

        [Inject]
        private NavigationManager _navigationManager { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "kioskId")]
        public int? KioskIdFromQuery { get; set; }

        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<KioskResponseDto> kiosklar = new();
        private List<KioskMenuResponseDto> allMenuler = new();
        private List<KioskMenuResponseDto> assignedMenuler = new();
        private List<KioskMenuResponseDto> unassignedMenuler = new();

        // Her kiosk'un menü sayısını tutan dictionary
        private Dictionary<int, int> kioskMenuCounts = new();

        private int selectedHizmetBinasiId = 0;
        private int selectedKioskId = 0;
        private string selectedKioskName = string.Empty;

        // Arama filtreleri
        private string searchAssigned = string.Empty;
        private string searchUnassigned = string.Empty;

        private bool isLoading = true;
        private bool isLoadingMenuler = false;
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
                // Hizmet binalarını yükle
                var binaResult = await _hizmetBinasiService.GetAllAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data.OrderBy(b => b.HizmetBinasiAdi).ToList();
                }

                // Query string'den kioskId geldi mi?
                if (KioskIdFromQuery.HasValue && KioskIdFromQuery.Value > 0)
                {
                    // Kiosk detaylarını al
                    var kioskResult = await _kioskService.GetByIdAsync(KioskIdFromQuery.Value);
                    if (kioskResult.Success && kioskResult.Data != null)
                    {
                        var kiosk = kioskResult.Data;

                        // Kiosk'un hizmet binasını seç
                        selectedHizmetBinasiId = kiosk.HizmetBinasiId;

                        // O hizmet binasının kiosk'larını yükle
                        var kioskListResult = await _kioskService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                        if (kioskListResult.Success && kioskListResult.Data != null)
                        {
                            kiosklar = kioskListResult.Data
                                .Where(k => k.Aktiflik == Aktiflik.Aktif)
                                .OrderBy(k => k.KioskAdi)
                                .ToList();

                            // Her kiosk için menü sayısını yükle
                            await LoadKioskMenuCountsAsync();
                        }

                        // İlgili kiosk'u seç
                        selectedKioskId = KioskIdFromQuery.Value;
                        selectedKioskName = kiosk.KioskAdi;

                        // Menüleri yükle
                        await LoadMenulerAsync();
                    }
                }
                else if (hizmetBinalari.Any())
                {
                    // Query string yoksa, ilk hizmet binasını seç
                    selectedHizmetBinasiId = hizmetBinalari.First().HizmetBinasiId;

                    // İlk hizmet binasının kiosk'larını yükle
                    var kioskListResult = await _kioskService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                    if (kioskListResult.Success && kioskListResult.Data != null)
                    {
                        kiosklar = kioskListResult.Data
                            .Where(k => k.Aktiflik == Aktiflik.Aktif)
                            .OrderBy(k => k.KioskAdi)
                            .ToList();

                        // Her kiosk için menü sayısını yükle
                        await LoadKioskMenuCountsAsync();

                        // İlk kiosk'u seç (varsa)
                        if (kiosklar.Any())
                        {
                            var firstKiosk = kiosklar.First();
                            selectedKioskId = firstKiosk.KioskId;
                            selectedKioskName = firstKiosk.KioskAdi;

                            // Menüleri yükle
                            await LoadMenulerAsync();
                        }
                    }
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

        private async Task LoadKioskMenuCountsAsync()
        {
            kioskMenuCounts.Clear();

            foreach (var kiosk in kiosklar)
            {
                var atamaResult = await _kioskMenuAtamaService.GetByKioskAsync(kiosk.KioskId);
                if (atamaResult.Success && atamaResult.Data != null)
                {
                    kioskMenuCounts[kiosk.KioskId] = atamaResult.Data.Count(a => a.Aktiflik == Aktiflik.Aktif);
                }
                else
                {
                    kioskMenuCounts[kiosk.KioskId] = 0;
                }
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            selectedHizmetBinasiId = int.Parse(e.Value?.ToString() ?? "0");
            selectedKioskId = 0;
            selectedKioskName = string.Empty;
            kiosklar.Clear();
            assignedMenuler.Clear();
            unassignedMenuler.Clear();

            if (selectedHizmetBinasiId > 0)
            {
                try
                {
                    var result = await _kioskService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                    if (result.Success && result.Data != null)
                    {
                        kiosklar = result.Data
                            .Where(k => k.Aktiflik == Aktiflik.Aktif)
                            .OrderBy(k => k.KioskAdi)
                            .ToList();

                        // Her kiosk için menü sayısını yükle
                        await LoadKioskMenuCountsAsync();
                    }
                }
                catch (Exception ex)
                {
                    await _toastService.ShowErrorAsync($"Kiosk'lar yüklenirken hata oluştu: {ex.Message}");
                }
            }
        }

        private async Task SelectKiosk(int kioskId)
        {
            selectedKioskId = kioskId;
            var kiosk = kiosklar.FirstOrDefault(k => k.KioskId == kioskId);
            selectedKioskName = kiosk?.KioskAdi ?? string.Empty;

            await LoadMenulerAsync();
        }

        private async Task LoadMenulerAsync()
        {
            if (selectedKioskId == 0) return;

            isLoadingMenuler = true;

            try
            {
                // Tüm menüleri yükle
                var menuResult = await _kioskMenuService.GetAllAsync();
                if (menuResult.Success && menuResult.Data != null)
                {
                    allMenuler = menuResult.Data
                        .Where(m => m.Aktiflik == Aktiflik.Aktif)
                        .OrderBy(m => m.MenuSira)
                        .ThenBy(m => m.MenuAdi)
                        .ToList();

                    // Kiosk'a atanmış menüleri al
                    var atamaResult = await _kioskMenuAtamaService.GetByKioskAsync(selectedKioskId);
                    if (atamaResult.Success && atamaResult.Data != null)
                    {
                        var assignedMenuIds = atamaResult.Data
                            .Where(a => a.Aktiflik == Aktiflik.Aktif)
                            .Select(a => a.KioskMenuId)
                            .ToList();

                        assignedMenuler = allMenuler
                            .Where(m => assignedMenuIds.Contains(m.KioskMenuId))
                            .ToList();

                        unassignedMenuler = allMenuler
                            .Where(m => !assignedMenuIds.Contains(m.KioskMenuId))
                            .ToList();
                    }
                    else
                    {
                        assignedMenuler.Clear();
                        unassignedMenuler = allMenuler.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Menüler yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                isLoadingMenuler = false;
            }
        }

        // Add/Remove Operations
        private async Task AddMenu(int kioskMenuId)
        {
            isSaving = true;

            try
            {
                var createDto = new KioskMenuAtamaCreateRequestDto
                {
                    KioskId = selectedKioskId,
                    KioskMenuId = kioskMenuId,
                    Aktiflik = Aktiflik.Aktif
                };

                var result = await _kioskMenuAtamaService.CreateAsync(createDto);

                if (result.Success)
                {
                    var menu = unassignedMenuler.FirstOrDefault(m => m.KioskMenuId == kioskMenuId);
                    if (menu != null)
                    {
                        unassignedMenuler.Remove(menu);
                        assignedMenuler.Add(menu);
                        assignedMenuler = assignedMenuler.OrderBy(m => m.MenuSira).ThenBy(m => m.MenuAdi).ToList();

                        // Menü sayacını güncelle
                        if (kioskMenuCounts.ContainsKey(selectedKioskId))
                        {
                            kioskMenuCounts[selectedKioskId]++;
                        }

                        await _toastService.ShowSuccessAsync($"{menu.MenuAdi} menüsü eklendi");
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Menü eklenemedi");
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

        private async Task RemoveMenu(int kioskMenuId)
        {
            isSaving = true;

            try
            {
                // Önce atama kaydını bul
                var atamaResult = await _kioskMenuAtamaService.GetByKioskAsync(selectedKioskId);
                if (atamaResult.Success && atamaResult.Data != null)
                {
                    var atama = atamaResult.Data.FirstOrDefault(a =>
                        a.KioskMenuId == kioskMenuId &&
                        a.Aktiflik == Aktiflik.Aktif);

                    if (atama != null)
                    {
                        var result = await _kioskMenuAtamaService.DeleteAsync(atama.KioskMenuAtamaId);

                        if (result.Success)
                        {
                            var menu = assignedMenuler.FirstOrDefault(m => m.KioskMenuId == kioskMenuId);
                            if (menu != null)
                            {
                                assignedMenuler.Remove(menu);
                                unassignedMenuler.Add(menu);
                                unassignedMenuler = unassignedMenuler.OrderBy(m => m.MenuSira).ThenBy(m => m.MenuAdi).ToList();

                                // Menü sayacını güncelle
                                if (kioskMenuCounts.ContainsKey(selectedKioskId))
                                {
                                    kioskMenuCounts[selectedKioskId]--;
                                }

                                await _toastService.ShowSuccessAsync($"{menu.MenuAdi} menüsü kaldırıldı");
                            }
                        }
                        else
                        {
                            await _toastService.ShowErrorAsync(result.Message ?? "Menü kaldırılamadı");
                        }
                    }
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
        private List<KioskMenuResponseDto> GetFilteredAssignedMenuler()
        {
            if (string.IsNullOrWhiteSpace(searchAssigned))
                return assignedMenuler;

            return assignedMenuler
                .Where(m => m.MenuAdi.Contains(searchAssigned, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private List<KioskMenuResponseDto> GetFilteredUnassignedMenuler()
        {
            if (string.IsNullOrWhiteSpace(searchUnassigned))
                return unassignedMenuler;

            return unassignedMenuler
                .Where(m => m.MenuAdi.Contains(searchUnassigned, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/kiosk-menu-atama");
        }
    }
}
