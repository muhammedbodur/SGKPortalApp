using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Banko
{
    public partial class PersonelAtama
    {
        [Inject] private IBankoApiService _bankoService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IPersonelApiService _personelService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<PersonelAtama> _logger { get; set; } = default!;

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool showAtaModal = false;
        private bool showCikarModal = false;

        // Data
        private List<BankoResponseDto> allBankolar = new();
        private List<BankoResponseDto> filteredBankolar = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<PersonelResponseDto> availablePersoneller = new();
        private BankoResponseDto? selectedBanko;

        // Filters
        private int selectedHizmetBinasiId = 0;
        private KatTipi? selectedKat = null;
        private string selectedDurum = string.Empty;
        private string selectedPersonelTcKimlikNo = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownData();

            // İlk hizmet binasını seç
            if (hizmetBinalari.Any())
            {
                selectedHizmetBinasiId = hizmetBinalari.First().HizmetBinasiId;
                await LoadData();
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                var binaResult = await _hizmetBinasiService.GetActiveAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data;
                }
                else
                {
                    hizmetBinalari = new List<HizmetBinasiResponseDto>();
                    await _toastService.ShowWarningAsync("Aktif hizmet binası bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
        }

        private async Task LoadData()
        {
            if (selectedHizmetBinasiId <= 0)
            {
                await _toastService.ShowWarningAsync("Lütfen bir hizmet binası seçiniz");
                return;
            }

            try
            {
                isLoading = true;

                var result = await _bankoService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                if (result.Success && result.Data != null)
                {
                    allBankolar = result.Data;
                    ApplyFilters();
                }
                else
                {
                    allBankolar = new List<BankoResponseDto>();
                    filteredBankolar = new List<BankoResponseDto>();
                    await _toastService.ShowWarningAsync(result.Message ?? "Banko bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bankolar yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Bankolar yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ApplyFilters()
        {
            filteredBankolar = allBankolar.ToList();

            // Kat filtresi
            if (selectedKat.HasValue)
            {
                filteredBankolar = filteredBankolar.Where(b => b.KatTipi == selectedKat.Value).ToList();
            }

            // Durum filtresi
            if (!string.IsNullOrEmpty(selectedDurum))
            {
                if (selectedDurum == "bos")
                {
                    filteredBankolar = filteredBankolar.Where(b => b.BankoMusaitMi).ToList();
                }
                else if (selectedDurum == "dolu")
                {
                    filteredBankolar = filteredBankolar.Where(b => !b.BankoMusaitMi).ToList();
                }
            }

            // Sıralama: Önce kat, sonra banko no
            filteredBankolar = filteredBankolar
                .OrderBy(b => (int)b.KatTipi)
                .ThenBy(b => b.BankoNo)
                .ToList();
        }

        // ═══════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════════════════════════════

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int binaId))
            {
                selectedHizmetBinasiId = binaId;
                await LoadData();
            }
        }

        private void OnKatChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                selectedKat = null;
            }
            else if (int.TryParse(e.Value.ToString(), out int katValue))
            {
                selectedKat = (KatTipi)katValue;
            }
            ApplyFilters();
        }

        private void OnDurumChanged(ChangeEventArgs e)
        {
            selectedDurum = e.Value?.ToString() ?? string.Empty;
            ApplyFilters();
        }

        private void ClearFilters()
        {
            selectedKat = null;
            selectedDurum = string.Empty;
            ApplyFilters();
        }

        // ═══════════════════════════════════════════════════════
        // MODAL OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task ShowAtaModal(BankoResponseDto banko)
        {
            selectedBanko = banko;
            selectedPersonelTcKimlikNo = string.Empty;

            // Hizmet binası kontrolü
            if (selectedHizmetBinasiId <= 0)
            {
                await _toastService.ShowWarningAsync("Lütfen önce hizmet binası seçiniz");
                return;
            }

            // Müsait personelleri yükle
            try
            {
                // Hizmet binasına göre personelleri getir (API'den filtrelenmiş)
                var result = await _personelService.GetPersonellerByHizmetBinasiIdAsync(selectedHizmetBinasiId);
                if (result.Success && result.Data != null)
                {
                    // Sadece başka bankoya atanmamış personelleri filtrele
                    var atananPersonelTcler = allBankolar
                        .Where(b => b.AtananPersonel != null)
                        .Select(b => b.AtananPersonel!.TcKimlikNo)
                        .ToHashSet();

                    availablePersoneller = result.Data
                        .Where(p => !atananPersonelTcler.Contains(p.TcKimlikNo))
                        .ToList();

                    if (!availablePersoneller.Any())
                    {
                        var selectedBina = hizmetBinalari.FirstOrDefault(h => h.HizmetBinasiId == selectedHizmetBinasiId);
                        await _toastService.ShowWarningAsync($"{selectedBina?.HizmetBinasiAdi ?? "Bu hizmet binasında"} müsait personel bulunamadı");
                    }
                }
                else
                {
                    availablePersoneller = new List<PersonelResponseDto>();
                    await _toastService.ShowWarningAsync("Personel bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personeller yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Personeller yüklenirken hata oluştu");
            }

            showAtaModal = true;
        }

        private void HideAtaModal()
        {
            showAtaModal = false;
            selectedBanko = null;
            selectedPersonelTcKimlikNo = string.Empty;
        }

        private void ShowCikarModal(BankoResponseDto banko)
        {
            selectedBanko = banko;
            showCikarModal = true;
        }

        private void HideCikarModal()
        {
            showCikarModal = false;
            selectedBanko = null;
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL ATAMA OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task PersonelAta()
        {
            if (selectedBanko == null || string.IsNullOrEmpty(selectedPersonelTcKimlikNo)) return;

            try
            {
                isSaving = true;

                var atamaDto = new BankoPersonelAtaDto
                {
                    BankoId = selectedBanko.BankoId,
                    TcKimlikNo = selectedPersonelTcKimlikNo
                };

                var result = await _bankoService.PersonelAtaAsync(atamaDto);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Personel başarıyla atandı");
                    HideAtaModal();
                    await LoadData();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Personel atanamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel atanırken hata oluştu");
                await _toastService.ShowErrorAsync("Personel atanırken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task PersonelCikar()
        {
            if (selectedBanko?.AtananPersonel == null) return;

            try
            {
                isSaving = true;

                var result = await _bankoService.PersonelCikarAsync(selectedBanko.AtananPersonel.TcKimlikNo);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Personel başarıyla çıkarıldı");
                    HideCikarModal();
                    await LoadData();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Personel çıkarılamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel çıkarılırken hata oluştu");
                await _toastService.ShowErrorAsync("Personel çıkarılırken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private string GetBankoTipiBadgeClass(BankoTipi tip)
        {
            return tip switch
            {
                BankoTipi.Normal => "bg-label-primary",
                BankoTipi.Oncelikli => "bg-label-warning",
                BankoTipi.Engelli => "bg-label-info",
                BankoTipi.SefMasasi => "bg-label-success",
                _ => "bg-label-secondary"
            };
        }
    }
}
