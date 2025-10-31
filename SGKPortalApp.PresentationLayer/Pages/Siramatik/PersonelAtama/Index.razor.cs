using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.Reflection;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.PersonelAtama
{
    /// <summary>
    /// PersonelAtama - Kanal/Alt Kanal'a Personel Uzmanlık Atama Sayfası
    /// 
    /// AMAÇ:
    /// - Excel benzeri matrix grid'de personel-kanal işlem atamalarını yönetir
    /// - Her hücre toggle düğmesi gibi çalışır: 0 → 1 → 2 → 0 döngüsü
    /// - Renkli gösterim: 1=Yeşil, 2=Mavi, 0=Gri
    /// </summary>
    public partial class Index : ComponentBase
    {

        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IPersonelApiService _personelService { get; set; } = default!;
        [Inject] private IKanalAltIslemApiService _kanalAltIslemService { get; set; } = default!;
        [Inject] private IKanalPersonelApiService _kanalPersonelService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;


        private List<HizmetBinasiResponseDto> HizmetBinalari { get; set; } = new();
        
        private List<KanalPersonelResponseDto> Personeller { get; set; } = new();
        
        private List<KanalAltIslemResponseDto> KanalAltIslemler { get; set; } = new();


        private Dictionary<string, KanalPersonelResponseDto> AtamalarDict { get; set; } = new();
        private HashSet<string> YuklenenHucreler { get; set; } = new();


        private int SelectedHizmetBinasiId { get; set; } = 0;
        private string PersonelSearchTerm { get; set; } = string.Empty;
        private string KanalAltIslemSearchTerm { get; set; } = string.Empty;


        private bool IsLoading { get; set; } = true;
        private bool IsLoadingDropdowns { get; set; } = false;
        private bool IsLoadingMatrix { get; set; } = false;

        private List<KanalPersonelResponseDto> FilteredPersoneller
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PersonelSearchTerm))
                    return Personeller;

                var searchLower = PersonelSearchTerm.ToLower().Trim();
                return Personeller
                    .Where(p => p.PersonelAdSoyad.ToLower().Contains(searchLower) ||
                                p.TcKimlikNo.Contains(searchLower))
                    .ToList();
            }
        }

        private List<KanalAltIslemResponseDto> FilteredKanalAltIslemler
        {
            get
            {
                if (string.IsNullOrWhiteSpace(KanalAltIslemSearchTerm))
                    return KanalAltIslemler;

                var searchLower = KanalAltIslemSearchTerm.ToLower().Trim();
                return KanalAltIslemler
                    .Where(kai => kai.KanalAltAdi.ToLower().Contains(searchLower) ||
                                  kai.KanalAdi.ToLower().Contains(searchLower))
                    .ToList();
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // #7 LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            try
            {
                await LoadHizmetBinalari();
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Sayfa yüklenmesinde hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // #8 DATA LOADING METHODS
        // ═══════════════════════════════════════════════════════════════════════════════

        private async Task LoadHizmetBinalari()
        {
            IsLoadingDropdowns = true;
            try
            {
                var result = await _hizmetBinasiService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    HizmetBinalari = result.Data;

                    if (HizmetBinalari.Any() && SelectedHizmetBinasiId == 0)
                    {
                        SelectedHizmetBinasiId = HizmetBinalari.First().HizmetBinasiId;
                        await OnHizmetBinasiChanged();
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Hizmet binaları yüklenemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hizmet binaları yüklenirken hata: {ex.Message}");
            }
            finally
            {
                IsLoadingDropdowns = false;
            }
        }

        private async Task OnHizmetBinasiChanged()
        {
            if (SelectedHizmetBinasiId == 0)
            {
                Personeller.Clear();
                KanalAltIslemler.Clear();
                AtamalarDict.Clear();
                return;
            }

            IsLoadingMatrix = true;
            try
            {
                // 1. Personeller'i yükle (satırlar) - 
                var personelResult = await _kanalPersonelService.GetPersonellerByHizmetBinasiIdAsync(SelectedHizmetBinasiId);
                Personeller = personelResult.Success && personelResult.Data != null
                    ? personelResult.Data
                    : new List<KanalPersonelResponseDto>();

                // 2. Kanal Alt İşlemleri yükle (sütunlar)
                var kanalAltIslemResult = await _kanalAltIslemService.GetByHizmetBinasiIdAsync(SelectedHizmetBinasiId);
                KanalAltIslemler = kanalAltIslemResult.Success && kanalAltIslemResult.Data != null
                    ? kanalAltIslemResult.Data
                    : new List<KanalAltIslemResponseDto>();

                // 3. Atamaları yükle
                await LoadAtamalar();

                // Arama terimlerini temizle
                PersonelSearchTerm = string.Empty;
                KanalAltIslemSearchTerm = string.Empty;
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Matrix verisi yüklenirken hata: {ex.Message}");
            }
            finally
            {
                IsLoadingMatrix = false;
            }
        }

        private async Task LoadAtamalar()
        {
            try
            {
                var result = await _kanalPersonelService.GetPersonellerByHizmetBinasiIdAsync(SelectedHizmetBinasiId);

                AtamalarDict.Clear();

                if (result.Success && result.Data != null)
                {
                    foreach (var atama in result.Data)
                    {
                        var key = GetHucreKey(atama.TcKimlikNo, atama.KanalAltIslemId);
                        AtamalarDict[key] = atama;
                    }
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Atamalar yüklenirken hata: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // #9 MATRIX HELPER METHODS
        // ═══════════════════════════════════════════════════════════════════════════════

        private string GetHucreKey(string personelTc, int kanalAltIslemId)
            => $"{personelTc}-{kanalAltIslemId}";

        private PersonelUzmanlik GetHucreValue(string personelTc, int kanalAltIslemId)
        {
            var key = GetHucreKey(personelTc, kanalAltIslemId);
            return AtamalarDict.ContainsKey(key)
                ? AtamalarDict[key].Uzmanlik
                : PersonelUzmanlik.BilgisiYok;
        }

        private string GetHucreId(string personelTc, int kanalAltIslemId)
            => $"hucre-{personelTc}-{kanalAltIslemId}";

        private bool IsHucreLoading(string personelTc, int kanalAltIslemId)
        {
            var key = GetHucreKey(personelTc, kanalAltIslemId);
            return YuklenenHucreler.Contains(key);
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // #10 ENUM HELPER METHODS
        // ═══════════════════════════════════════════════════════════════════════════════

        private string GetUzmanlikDisplayName(PersonelUzmanlik uzmanlik)
        {
            var fieldInfo = uzmanlik.GetType().GetField(uzmanlik.ToString());
            var displayAttribute = fieldInfo?
                .GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                as System.ComponentModel.DataAnnotations.DisplayAttribute;

            return displayAttribute?.GetName() ?? uzmanlik.ToString();
        }

        private string GetHucreButtonClass(PersonelUzmanlik uzmanlik)
        {
            return uzmanlik switch
            {
                PersonelUzmanlik.Uzman => "btn btn-success",
                PersonelUzmanlik.YrdUzman => "btn btn-info",
                PersonelUzmanlik.BilgisiYok => "btn btn-secondary", 
                _ => "btn btn-secondary"
            };
        }

        private PersonelUzmanlik GetNextUzmanlikValue(PersonelUzmanlik mevcutValue)
        {
            return mevcutValue switch
            {
                PersonelUzmanlik.BilgisiYok => PersonelUzmanlik.Uzman,
                PersonelUzmanlik.Uzman => PersonelUzmanlik.YrdUzman,
                PersonelUzmanlik.YrdUzman => PersonelUzmanlik.BilgisiYok, 
                _ => PersonelUzmanlik.BilgisiYok
            };
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // #11 TOGGLE/HÜCRE CLICK METHODS - ANA İŞLEV
        // ═══════════════════════════════════════════════════════════════════════════════

        private async Task OnHucreClicked(string personelTc, int kanalAltIslemId)
        {
            var key = GetHucreKey(personelTc, kanalAltIslemId);

            if (YuklenenHucreler.Contains(key))
                return;

            YuklenenHucreler.Add(key);

            try
            {
                var mevcutValue = GetHucreValue(personelTc, kanalAltIslemId);
                var yeniValue = GetNextUzmanlikValue(mevcutValue);

                await ToggleAtama(personelTc, kanalAltIslemId, mevcutValue, yeniValue);
            }
            finally
            {
                YuklenenHucreler.Remove(key);
                StateHasChanged();
            }
        }

        private async Task ToggleAtama(string personelTc, int kanalAltIslemId,
            PersonelUzmanlik mevcutValue, PersonelUzmanlik yeniValue)
        {
            try
            {
                if (yeniValue == PersonelUzmanlik.BilgisiYok && mevcutValue != PersonelUzmanlik.BilgisiYok)
                {
                    // DELETE işlemi
                    var key = GetHucreKey(personelTc, kanalAltIslemId);
                    if (AtamalarDict.ContainsKey(key))
                    {
                        var kanalPersonelId = AtamalarDict[key].KanalPersonelId;
                        var deleteResult = await _kanalPersonelService.DeleteAsync(kanalPersonelId);

                        if (deleteResult.Success)
                        {
                            AtamalarDict.Remove(key);
                            await _toastService.ShowSuccessAsync("Atama kaldırıldı");
                        }
                        else
                        {
                            await _toastService.ShowErrorAsync(deleteResult.Message ?? "Silme işlemi başarısız");
                        }
                    }
                }
                else if (mevcutValue == PersonelUzmanlik.BilgisiYok && yeniValue != PersonelUzmanlik.BilgisiYok)
                {
                    // POST işlemi (yeni atama oluştur)
                    var createRequest = new KanalPersonelCreateRequestDto
                    {
                        TcKimlikNo = personelTc,
                        KanalAltIslemId = kanalAltIslemId,
                        Uzmanlik = yeniValue
                    };

                    var createResult = await _kanalPersonelService.CreateAsync(createRequest);

                    if (createResult.Success && createResult.Data != null)
                    {
                        var key = GetHucreKey(personelTc, kanalAltIslemId);
                        AtamalarDict[key] = createResult.Data;
                        await _toastService.ShowSuccessAsync("Atama oluşturuldu");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(createResult.Message ?? "Oluşturma işlemi başarısız");
                    }
                }
                else if (mevcutValue != PersonelUzmanlik.BilgisiYok && yeniValue != PersonelUzmanlik.BilgisiYok)
                {
                    // PUT işlemi (uzmanlık değerini güncelle)
                    var key = GetHucreKey(personelTc, kanalAltIslemId);
                    if (AtamalarDict.ContainsKey(key))
                    {
                        var kanalPersonelId = AtamalarDict[key].KanalPersonelId;
                        var updateRequest = new KanalPersonelUpdateRequestDto
                        {
                            Uzmanlik = yeniValue
                        };

                        var updateResult = await _kanalPersonelService.UpdateAsync(kanalPersonelId, updateRequest);

                        if (updateResult.Success && updateResult.Data != null)
                        {
                            AtamalarDict[key] = updateResult.Data;
                            await _toastService.ShowSuccessAsync("Atama güncellendi");
                        }
                        else
                        {
                            await _toastService.ShowErrorAsync(updateResult.Message ?? "Güncelleme işlemi başarısız");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"İşlem sırasında hata: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // #12 FILTER METHODS
        // ═══════════════════════════════════════════════════════════════════════════════

        private void OnPersonelSearchChanged(ChangeEventArgs e)
        {
            PersonelSearchTerm = e.Value?.ToString() ?? string.Empty;
        }

        private void OnKanalAltIslemSearchChanged(ChangeEventArgs e)
        {
            KanalAltIslemSearchTerm = e.Value?.ToString() ?? string.Empty;
        }

        private void ClearAllFilters()
        {
            PersonelSearchTerm = string.Empty;
            KanalAltIslemSearchTerm = string.Empty;
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // #13 EXPORT METHODS - DEPARTMAN PATTERN
        // ═══════════════════════════════════════════════════════════════════════════════

        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = string.Empty;

        private async Task ExportToExcel()
        {
            IsExporting = true;
            ExportType = "excel";
            try
            {
                await Task.Delay(1000); // Simulated export
                await _toastService.ShowInfoAsync("Excel export özelliği yakında eklenecek");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }

        private async Task ExportToPdf()
        {
            IsExporting = true;
            ExportType = "pdf";
            try
            {
                await Task.Delay(1000); // Simulated export
                await _toastService.ShowInfoAsync("PDF export özelliği yakında eklenecek");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }
    }
}