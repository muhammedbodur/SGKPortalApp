using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
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
    public partial class Index
    {
        protected override string PagePermissionKey => "SIRA.PERSONELATAMA.INDEX";


        // URL Parameters
        [SupplyParameterFromQuery(Name = "kanalAltIslemId")]
        public int? KanalAltIslemIdParam { get; set; }
        
        [SupplyParameterFromQuery(Name = "hizmetBinasiId")]
        public int? HizmetBinasiIdParam { get; set; }
        
        // Lock state
        private bool isFiltersLocked = false;
        private int lockedKanalAltIslemId = 0;

        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IPersonelApiService _personelService { get; set; } = default!;
        [Inject] private IKanalAltIslemApiService _kanalAltIslemService { get; set; } = default!;
        [Inject] private IKanalPersonelApiService _kanalPersonelService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;


        private List<HizmetBinasiResponseDto> HizmetBinalari { get; set; } = new();
        
        // Yeni Matrix Yapısı
        private List<PersonelAtamaMatrixDto> PersonelMatrix { get; set; } = new();
        
        private List<KanalAltIslemResponseDto> KanalAltIslemler { get; set; } = new();

        private List<ServisResponseDto> Servisler { get; set; } = new();
        private int SelectedServisId { get; set; } = 0;

        // Hücre işlemleri için
        private Dictionary<string, PersonelKanalAtamaDto> AtamalarDict { get; set; } = new();
        private HashSet<string> YuklenenHucreler { get; set; } = new();


        private int SelectedHizmetBinasiId { get; set; } = 0;
        private string PersonelSearchTerm { get; set; } = string.Empty;
        private string KanalAltIslemSearchTerm { get; set; } = string.Empty;


        private bool IsLoading { get; set; } = true;
        private bool IsLoadingDropdowns { get; set; } = false;
        private bool IsLoadingMatrix { get; set; } = false;

        private List<PersonelAtamaMatrixDto> FilteredPersoneller
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PersonelSearchTerm))
                    return PersonelMatrix;

                var searchLower = PersonelSearchTerm.ToLower().Trim();
                return PersonelMatrix
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

                // Select option'dan gelen ID'ye göre filtrele
                if (int.TryParse(KanalAltIslemSearchTerm, out int selectedId))
                {
                    return KanalAltIslemler
                        .Where(kai => kai.KanalAltIslemId == selectedId)
                        .ToList();
                }

                return KanalAltIslemler;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // #7 LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await base.OnInitializedAsync();

            IsLoading = true;
            try
            {
                await LoadHizmetBinalari();

                // URL'den kanalAltIslemId parametresi geldiyse
                if (KanalAltIslemIdParam.HasValue && KanalAltIslemIdParam.Value > 0)
                {
                    await LoadFromKanalAltIslemId(KanalAltIslemIdParam.Value);
                }
                // URL'den hizmetBinasiId parametresi geldiyse
                else if (HizmetBinasiIdParam.HasValue && HizmetBinasiIdParam.Value > 0)
                {
                    SelectedHizmetBinasiId = HizmetBinasiIdParam.Value;
                    await LoadServisler();
                    await LoadMatrixData();
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Sayfa yüklenmesınde hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadFromKanalAltIslemId(int kanalAltIslemId)
        {
            try
            {
                // Kanal alt işlem bilgisini al
                var kanalAltIslemResult = await _kanalAltIslemService.GetByIdWithDetailsAsync(kanalAltIslemId);
                if (kanalAltIslemResult.Success && kanalAltIslemResult.Data != null)
                {
                    var kanalAltIslem = kanalAltIslemResult.Data;
                    
                    // Hizmet binasını seç
                    SelectedHizmetBinasiId = kanalAltIslem.HizmetBinasiId;
                    
                    // Kilitle
                    isFiltersLocked = true;
                    lockedKanalAltIslemId = kanalAltIslemId;
                    
                    // Servisleri yükle
                    await LoadServisler();
                    
                    // Matrix verisini yükle
                    await LoadMatrixData();
                    
                    // Kanal alt işlem filtresini ayarla
                    KanalAltIslemSearchTerm = kanalAltIslemId.ToString();
                }
                else
                {
                    await _toastService.ShowErrorAsync("Kanal alt işlem bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal alt işlem yüklenirken hata: {KanalAltIslemId}", kanalAltIslemId);
                await _toastService.ShowErrorAsync("Veriler yüklenirken hata oluştu");
            }
        }
        
        private void UnlockFilters()
        {
            isFiltersLocked = false;
            lockedKanalAltIslemId = 0;
            KanalAltIslemSearchTerm = string.Empty;
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
                        // Kullanıcının kendi Hizmet Binasını default olarak seç
                        var userHizmetBinasiId = GetCurrentUserHizmetBinasiId();
                        if (userHizmetBinasiId > 0 && HizmetBinalari.Any(b => b.HizmetBinasiId == userHizmetBinasiId))
                        {
                            SelectedHizmetBinasiId = userHizmetBinasiId;
                            await LoadServisler();
                            await LoadMatrixData();
                        }
                        else if (HizmetBinalari.Any())
                        {
                            SelectedHizmetBinasiId = HizmetBinalari.First().HizmetBinasiId;
                            await LoadServisler();
                            await LoadMatrixData();
                        }
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

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (e.Value != null && int.TryParse(e.Value.ToString(), out int selectedId))
            {
                // ✅ Güvenlik kontrolü: Kullanıcı başka Hizmet Binasını seçmeye çalışıyor mu?
                if (selectedId > 0 && !CanAccessHizmetBinasi(selectedId))
                {
                    await _toastService.ShowWarningAsync("Bu Hizmet Binasını görüntüleme yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz Hizmet Binası erişim denemesi: {BinaId}", selectedId);
                    return; // İşlemi durdur
                }

                SelectedHizmetBinasiId = selectedId;
            }

            SelectedServisId = 0;
            Servisler.Clear();

            if (SelectedHizmetBinasiId > 0)
            {
                await LoadServisler();
            }

            await LoadMatrixData();
        }

        private async Task OnServisChanged(ChangeEventArgs e)
        {
            if (e.Value != null && int.TryParse(e.Value.ToString(), out int selectedId))
            {
                SelectedServisId = selectedId;
            }
            else
            {
                SelectedServisId = 0;
            }

            await LoadMatrixData();
        }

        private async Task LoadMatrixData()
        {
            if (SelectedHizmetBinasiId == 0)
            {
                PersonelMatrix.Clear();
                KanalAltIslemler.Clear();
                AtamalarDict.Clear();
                return;
            }

            IsLoadingMatrix = true;
            try
            {
                // 1. Personel Matrix'i yükle (personeller + kanal atamaları)
                var matrixResult = await _kanalPersonelService.GetPersonelAtamaMatrixAsync(SelectedHizmetBinasiId);
                var allPersonelMatrix = matrixResult.Success && matrixResult.Data != null
                    ? matrixResult.Data
                    : new List<PersonelAtamaMatrixDto>();

                // 🆕 Servise göre filtrele
                if (SelectedServisId > 0)
                {
                    PersonelMatrix = allPersonelMatrix
                        .Where(p => p.ServisId == SelectedServisId)
                        .ToList();
                }
                else
                {
                    PersonelMatrix = allPersonelMatrix;
                }

                // 2. Kanal Alt İşlemleri yükle (sütunlar)
                var kanalAltIslemResult = await _kanalAltIslemService.GetByHizmetBinasiIdAsync(SelectedHizmetBinasiId);
                KanalAltIslemler = kanalAltIslemResult.Success && kanalAltIslemResult.Data != null
                    ? kanalAltIslemResult.Data
                    : new List<KanalAltIslemResponseDto>();

                // 3. Atamaları dictionary'e yükle (hızlı erişim için)
                LoadAtamalariDictionary();

                // Arama terimlerini temizle (kilitli değilse)
                PersonelSearchTerm = string.Empty;
                if (!isFiltersLocked)
                {
                    KanalAltIslemSearchTerm = string.Empty;
                }
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

        private async Task LoadServisler()
        {
            try
            {
                var result = await _hizmetBinasiService.GetServislerByHizmetBinasiIdAsync(SelectedHizmetBinasiId);

                if (result.Success && result.Data != null)
                {
                    Servisler = result.Data;
                    _logger.LogInformation($"✅ {Servisler.Count} servis yüklendi (Hizmet Binası ID: {SelectedHizmetBinasiId})");
                }
                else
                {
                    Servisler = new List<ServisResponseDto>();
                    _logger.LogWarning("Seçili hizmet binasında personel servisi bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servisler yüklenirken hata oluştu");
                Servisler = new List<ServisResponseDto>();
                await _toastService.ShowErrorAsync("Servisler yüklenirken hata oluştu");
            }
        }

        private void LoadAtamalariDictionary()
        {
            AtamalarDict.Clear();

            foreach (var personel in PersonelMatrix)
            {
                foreach (var atama in personel.KanalAtamalari)
                {
                    var key = GetHucreKey(personel.TcKimlikNo, atama.KanalAltIslemId);
                    AtamalarDict[key] = atama;
                }
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
                PersonelUzmanlik.Sef => "btn btn-danger",
                PersonelUzmanlik.BilgisiYok => "btn btn-secondary", 
                _ => "btn btn-secondary"
            };
        }

        private PersonelUzmanlik GetNextUzmanlikValue(PersonelUzmanlik mevcutValue)
        {
            return mevcutValue switch
            {
                PersonelUzmanlik.Uzman => PersonelUzmanlik.YrdUzman,
                PersonelUzmanlik.YrdUzman => PersonelUzmanlik.BilgisiYok,
                PersonelUzmanlik.BilgisiYok => PersonelUzmanlik.Sef,
                PersonelUzmanlik.Sef => PersonelUzmanlik.Uzman,
                _ => PersonelUzmanlik.Uzman
            };
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // #11 TOGGLE/HÜCRE CLICK METHODS - ANA İŞLEV
        // ═══════════════════════════════════════════════════════════════════════════════

        private async Task OnHucreClicked(string personelTc, int kanalAltIslemId, bool isPasif = false)
        {
            // Pasif kanal alt işlem kontrolü
            if (isPasif)
            {
                await _toastService.ShowWarningAsync("Bu Alt Kanal İşlemi Pasif durumdadır. Lütfen önce aktif edin.");
                return;
            }

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
                            // Matrix'ten de kaldır
                            var personel = PersonelMatrix.FirstOrDefault(p => p.TcKimlikNo == personelTc);
                            if (personel != null)
                            {
                                var atama = personel.KanalAtamalari.FirstOrDefault(a => a.KanalAltIslemId == kanalAltIslemId);
                                if (atama != null)
                                {
                                    personel.KanalAtamalari.Remove(atama);
                                }
                            }
                            await _toastService.ShowSuccessAsync("Atama kaldırıldı");
                        }
                        else
                        {
                            await _toastService.ShowErrorAsync(deleteResult.Message ?? "Silme işlemi başarısız");
                        }
                    }
                }else if (mevcutValue != PersonelUzmanlik.BilgisiYok && yeniValue != PersonelUzmanlik.BilgisiYok)
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
                            var guncellenenAtama = new PersonelKanalAtamaDto
                            {
                                KanalPersonelId = updateResult.Data.KanalPersonelId,
                                KanalAltIslemId = updateResult.Data.KanalAltIslemId,
                                KanalAltIslemAdi = updateResult.Data.KanalAltIslemAdi,
                                Uzmanlik = updateResult.Data.Uzmanlik,
                                EklenmeTarihi = updateResult.Data.EklenmeTarihi,
                                DuzenlenmeTarihi = updateResult.Data.DuzenlenmeTarihi
                            };
                            AtamalarDict[key] = guncellenenAtama;
                            // Matrix'te de güncelle
                            var personel = PersonelMatrix.FirstOrDefault(p => p.TcKimlikNo == personelTc);
                            if (personel != null)
                            {
                                var atama = personel.KanalAtamalari.FirstOrDefault(a => a.KanalAltIslemId == kanalAltIslemId);
                                if (atama != null)
                                {
                                    atama.Uzmanlik = yeniValue;
                                    atama.DuzenlenmeTarihi = updateResult.Data.DuzenlenmeTarihi;
                                }
                            }
                            await _toastService.ShowSuccessAsync("Atama güncellendi");
                        }
                        else
                        {
                            await _toastService.ShowErrorAsync(updateResult.Message ?? "Güncelleme işlemi başarısız");
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
                        Uzmanlik = yeniValue,
                        Aktiflik = Aktiflik.Aktif
                    };

                    var createResult = await _kanalPersonelService.CreateAsync(createRequest);

                    if (createResult.Success && createResult.Data != null)
                    {
                        var key = GetHucreKey(personelTc, kanalAltIslemId);
                        var yeniAtama = new PersonelKanalAtamaDto
                        {
                            KanalPersonelId = createResult.Data.KanalPersonelId,
                            KanalAltIslemId = createResult.Data.KanalAltIslemId,
                            KanalAltIslemAdi = createResult.Data.KanalAltIslemAdi,
                            Uzmanlik = createResult.Data.Uzmanlik,
                            EklenmeTarihi = createResult.Data.EklenmeTarihi,
                            DuzenlenmeTarihi = createResult.Data.DuzenlenmeTarihi
                        };
                        AtamalarDict[key] = yeniAtama;
                        // Matrix'e de ekle
                        var personel = PersonelMatrix.FirstOrDefault(p => p.TcKimlikNo == personelTc);
                        if (personel != null)
                        {
                            personel.KanalAtamalari.Add(yeniAtama);
                        }
                        await _toastService.ShowSuccessAsync("Atama oluşturuldu");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(createResult.Message ?? "Oluşturma işlemi başarısız");
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
            // Kilitli değilse kanal alt işlem filtresini de temizle
            if (!isFiltersLocked)
            {
                KanalAltIslemSearchTerm = string.Empty;
            }
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
                if (!PersonelMatrix.Any() || !KanalAltIslemler.Any())
                {
                    await _toastService.ShowWarningAsync("Export için veri bulunamadı");
                    return;
                }

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Personel Atama");

                // Header - Personel bilgileri
                worksheet.Cell(1, 1).Value = "Personel Ad Soyad";
                worksheet.Cell(1, 2).Value = "TC Kimlik No";
                
                // Header - Kanal Alt İşlemler
                int col = 3;
                foreach (var kai in FilteredKanalAltIslemler)
                {
                    worksheet.Cell(1, col).Value = $"{kai.KanalAdi} - {kai.KanalAltAdi}";
                    col++;
                }

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, col - 1);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Data rows
                int row = 2;
                foreach (var personel in FilteredPersoneller)
                {
                    worksheet.Cell(row, 1).Value = personel.PersonelAdSoyad;
                    worksheet.Cell(row, 2).Value = personel.TcKimlikNo;
                    
                    col = 3;
                    foreach (var kai in FilteredKanalAltIslemler)
                    {
                        var uzmanlik = GetHucreValue(personel.TcKimlikNo, kai.KanalAltIslemId);
                        var uzmanlikText = GetUzmanlikDisplayName(uzmanlik);
                        var cell = worksheet.Cell(row, col);
                        cell.Value = uzmanlikText;
                        
                        // Renklendirme
                        if (uzmanlik == PersonelUzmanlik.Uzman)
                            cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                        else if (uzmanlik == PersonelUzmanlik.YrdUzman)
                            cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                        else if (uzmanlik == PersonelUzmanlik.Sef)
                            cell.Style.Fill.BackgroundColor = XLColor.LightCoral;
                        else
                            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                        
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        col++;
                    }
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"PersonelAtama_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                await _toastService.ShowSuccessAsync("Excel dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel export hatası");
                await _toastService.ShowErrorAsync($"Excel oluşturulurken hata: {ex.Message}");
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
                if (!PersonelMatrix.Any() || !KanalAltIslemler.Any())
                {
                    await _toastService.ShowWarningAsync("Export için veri bulunamadı");
                    return;
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(1, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(8));

                        page.Header().Column(column =>
                        {
                            column.Item().Text("Personel Atama Matrix").FontSize(16).Bold();
                            column.Item().Text($"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(9);
                        });

                        page.Content().Table(table =>
                        {
                            // Sütun tanımları
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Personel Ad Soyad
                                columns.RelativeColumn(2); // TC Kimlik No
                                
                                // Her kanal alt işlem için sütun
                                foreach (var _ in FilteredKanalAltIslemler)
                                {
                                    columns.RelativeColumn(2);
                                }
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Personel").Bold().FontSize(8);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("TC No").Bold().FontSize(8);
                                
                                foreach (var kai in FilteredKanalAltIslemler)
                                {
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3)
                                        .Text($"{kai.KanalAdi}\n{kai.KanalAltAdi}").Bold().FontSize(7);
                                }
                            });

                            // Data rows
                            foreach (var personel in FilteredPersoneller)
                            {
                                table.Cell().Padding(3).Text(personel.PersonelAdSoyad).FontSize(7);
                                table.Cell().Padding(3).Text(personel.TcKimlikNo).FontSize(7);
                                
                                foreach (var kai in FilteredKanalAltIslemler)
                                {
                                    var uzmanlik = GetHucreValue(personel.TcKimlikNo, kai.KanalAltIslemId);
                                    var uzmanlikText = GetUzmanlikDisplayName(uzmanlik);
                                    
                                    // Renklendirme - her cell için yeni chain
                                    if (uzmanlik == PersonelUzmanlik.Uzman)
                                        table.Cell().Background(Colors.Green.Lighten3).Padding(3).Text(uzmanlikText).FontSize(7);
                                    else if (uzmanlik == PersonelUzmanlik.YrdUzman)
                                        table.Cell().Background(Colors.Blue.Lighten3).Padding(3).Text(uzmanlikText).FontSize(7);
                                    else if (uzmanlik == PersonelUzmanlik.Sef)
                                        table.Cell().Background(Colors.Red.Lighten3).Padding(3).Text(uzmanlikText).FontSize(7);
                                    else
                                        table.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text(uzmanlikText).FontSize(7);
                                }
                            }
                        });

                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Sayfa ");
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                var fileName = $"PersonelAtama_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(pdfBytes), "application/pdf");
                await _toastService.ShowSuccessAsync("PDF dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF export hatası");
                await _toastService.ShowErrorAsync($"PDF oluşturulurken hata: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }
    }
}