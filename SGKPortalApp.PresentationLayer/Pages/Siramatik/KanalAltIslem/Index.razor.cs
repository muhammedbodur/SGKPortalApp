using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalAltIslem
{
    public partial class Index
    {
        // ⚡ PagePermissionKey artık otomatik çözümleniyor!
        // Route: /siramatik/kanal-alt-islem → PermissionKey: SIRA.KANALALTISLEM.INDEX
        // Manuel override gerekmez, FieldPermissionPageBase otomatik halleder

        [Inject] private IKanalAltIslemApiService _kanalAltIslemService { get; set; } = default!;
        [Inject] private IKanalIslemApiService _kanalIslemService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // URL Parameters
        [SupplyParameterFromQuery(Name = "kanalIslemId")]
        public int? KanalIslemIdParam { get; set; }
        
        [SupplyParameterFromQuery(Name = "hizmetBinasiId")]
        public int? HizmetBinasiIdParam { get; set; }
        
        // State
        private bool isLoading = false;
        private bool isDeleting = false;
        private bool showDeleteModal = false;
        private bool isKanalIslemLocked = false; // URL'den geldiğinde kilitli

        // Data
        private List<KanalAltIslemResponseDto> allAltIslemler = new();
        private List<KanalAltIslemResponseDto> filteredAltIslemler = new();
        private List<KanalIslemResponseDto> kanalIslemler = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private KanalAltIslemResponseDto? selectedAltIslem;

        // Filters
        private string searchText = string.Empty;
        private int selectedHizmetBinasiId = 0;
        private int selectedKanalIslemId = 0;
        private Aktiflik? selectedAktiflik = null;

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await base.OnInitializedAsync();
            await LoadDropdownData();

            // URL'den kanalIslemId parametresi geldiyse
            if (KanalIslemIdParam.HasValue && KanalIslemIdParam.Value > 0)
            {
                await LoadFromKanalIslemId(KanalIslemIdParam.Value);
            }
            // URL'den hizmetBinasiId parametresi geldiyse
            else if (HizmetBinasiIdParam.HasValue && HizmetBinasiIdParam.Value > 0)
            {
                selectedHizmetBinasiId = HizmetBinasiIdParam.Value;
                await LoadData();
            }
            // Varsayılan: Kullanıcının kendi Hizmet Binasını seç
            else
            {
                try
                {
                    var userHizmetBinasiId = GetCurrentUserHizmetBinasiId();
                    if (userHizmetBinasiId > 0 && hizmetBinalari.Any(b => b.HizmetBinasiId == userHizmetBinasiId))
                    {
                        selectedHizmetBinasiId = userHizmetBinasiId;
                        await LoadData();
                    }
                    else if (hizmetBinalari.Any())
                    {
                        selectedHizmetBinasiId = hizmetBinalari.First().HizmetBinasiId;
                        await LoadData();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "KanalAltIslem OnInitializedAsync hatası");
                    // Hata olursa ilk hizmet binasını seç
                    if (hizmetBinalari.Any())
                    {
                        selectedHizmetBinasiId = hizmetBinalari.First().HizmetBinasiId;
                        await LoadData();
                    }
                }
            }
        }
        
        private async Task LoadFromKanalIslemId(int kanalIslemId)
        {
            try
            {
                isLoading = true;
                
                // Kanal işlem bilgisini al
                var kanalIslemResult = await _kanalIslemService.GetByIdAsync(kanalIslemId);
                if (kanalIslemResult.Success && kanalIslemResult.Data != null)
                {
                    var kanalIslem = kanalIslemResult.Data;
                    
                    // Hizmet binası ve kanal işlem seç
                    selectedHizmetBinasiId = kanalIslem.HizmetBinasiId;
                    selectedKanalIslemId = kanalIslemId;
                    isKanalIslemLocked = true; // Dropdown'ları kilitle
                    
                    // Verileri yükle
                    await LoadKanalIslemler();
                    await LoadAltIslemler();
                    ApplyFilters();
                }
                else
                {
                    await _toastService.ShowErrorAsync("Kanal işlem bulunamadı");
                    // Varsayılana dön
                    if (hizmetBinalari.Any())
                    {
                        selectedHizmetBinasiId = hizmetBinalari.First().HizmetBinasiId;
                        await LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem yüklenirken hata: {KanalIslemId}", kanalIslemId);
                await _toastService.ShowErrorAsync("Veriler yüklenirken hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Hizmet binalarını yükle
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

                // Paralel olarak verileri yükle
                var altIslemTask = LoadAltIslemler();
                var kanalTask = LoadKanalIslemler();

                await Task.WhenAll(altIslemTask, kanalTask);

                ApplyFilters();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veriler yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadAltIslemler()
        {
            var result = await _kanalAltIslemService.GetByHizmetBinasiIdAsync(selectedHizmetBinasiId);

            if (result.Success && result.Data != null)
            {
                allAltIslemler = result.Data;
            }
            else
            {
                allAltIslemler = new();
                await _toastService.ShowInfoAsync("Seçili hizmet binasında alt işlem bulunamadı");
            }
        }

        private async Task LoadKanalIslemler()
        {
            var result = await _kanalIslemService.GetByHizmetBinasiIdAsync(selectedHizmetBinasiId);

            if (result.Success && result.Data != null)
            {
                kanalIslemler = result.Data;
            }
            else
            {
                kanalIslemler = new();
            }
        }

        // Hizmet binası değiştiğinde tetiklenir
        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            // ✅ 1. YETKİ KONTROLÜ: Kullanıcı bu filtreyi değiştirebilir mi?
            if (!CanEditFieldInList("HIZMETBINASIID"))
            {
                await _toastService.ShowWarningAsync("Bu filtreyi değiştirme yetkiniz yok!");
                _logger.LogWarning("Yetkisiz filtre değiştirme denemesi: HIZMET_BINASI");
                return; // ❌ İşlemi durdur
            }

            if (int.TryParse(e.Value?.ToString(), out int binaId))
            {
                // ✅ 2. DATA VALİDATİON: Kullanıcı başka Hizmet Binasını seçmeye çalışıyor mu?
                if (binaId > 0 && !CanAccessHizmetBinasi(binaId))
                {
                    await _toastService.ShowWarningAsync("Bu Hizmet Binasını görüntüleme yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz Hizmet Binası erişim denemesi: {BinaId}", binaId);
                    return; // ❌ İşlemi durdur
                }

                selectedHizmetBinasiId = binaId;
                selectedKanalIslemId = 0; // Kanal işlem filtresini sıfırla
                await LoadData();
            }
        }

        private void OnKanalIslemChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int kanalIslemId))
            {
                selectedKanalIslemId = kanalIslemId;
            }
            else
            {
                selectedKanalIslemId = 0;
            }

            ApplyFilters();
        }
        
        private void UnlockFilters()
        {
            isKanalIslemLocked = false;
            selectedKanalIslemId = 0; // Tüm kanal işlemleri göster
            ApplyFilters();
        }
        
        private string GetManageUrl()
        {
            var url = "/siramatik/kanal-alt-islem/manage";
            var queryParams = new List<string>();
            
            if (selectedHizmetBinasiId > 0)
                queryParams.Add($"hizmetBinasiId={selectedHizmetBinasiId}");
            
            if (selectedKanalIslemId > 0)
                queryParams.Add($"kanalIslemId={selectedKanalIslemId}");
            
            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);
            
            return url;
        }

        private void OnSearchChanged()
        {
            ApplyFilters();
        }

        private void OnAktiflikChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int aktiflikValue))
            {
                selectedAktiflik = (Aktiflik)aktiflikValue;
            }
            else
            {
                selectedAktiflik = null;
            }

            ApplyFilters();
        }

        private void SetAktiflikFilter(Aktiflik? aktiflik)
        {
            selectedAktiflik = aktiflik;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            filteredAltIslemler = allAltIslemler
                .Where(a =>
                {
                    // Kanal işlem filtresi
                    if (selectedKanalIslemId > 0 && a.KanalIslemId != selectedKanalIslemId)
                    {
                        return false;
                    }

                    // Arama filtresi
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        var search = searchText.ToLowerInvariant();
                        if (!a.KanalAltAdi.ToLowerInvariant().Contains(search))
                        {
                            return false;
                        }
                    }

                    // Aktiflik filtresi
                    if (selectedAktiflik.HasValue && a.Aktiflik != selectedAktiflik.Value)
                    {
                        return false;
                    }

                    return true;
                })
                .OrderBy(a => a.KanalAdi)
                .ThenBy(a => a.KanalAltAdi)
                .ToList();
        }

        private void ClearFilters()
        {
            searchText = string.Empty;
            // Kilitli değilse kanal işlem filtresini de temizle
            if (!isKanalIslemLocked)
            {
                selectedKanalIslemId = 0;
            }
            selectedAktiflik = null;
            ApplyFilters();
        }

        private async Task ToggleAktiflik(KanalAltIslemResponseDto altIslem)
        {
            try
            {
                var yeniAktiflik = altIslem.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif;

                var updateDto = new KanalAltIslemUpdateRequestDto
                {
                    KanalAltId = altIslem.KanalAltId,
                    KanalIslemId = altIslem.KanalIslemId,
                    HizmetBinasiId = altIslem.HizmetBinasiId,
                    KioskIslemGrupId = null,
                    Aktiflik = yeniAktiflik
                };

                var result = await _kanalAltIslemService.UpdateAsync(altIslem.KanalAltIslemId, updateDto);

                if (result.Success)
                {
                    altIslem.Aktiflik = yeniAktiflik;
                    altIslem.DuzenlenmeTarihi = DateTime.Now;

                    var mesaj = yeniAktiflik == Aktiflik.Aktif
                        ? $"{altIslem.KanalAltAdi} aktif hale getirildi"
                        : $"{altIslem.KanalAltAdi} pasif hale getirildi";

                    await _toastService.ShowSuccessAsync(mesaj);
                    ApplyFilters();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Aktiflik durumu değiştirilemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktiflik durumu değiştirilirken hata oluştu");
                await _toastService.ShowErrorAsync("İşlem sırasında bir hata oluştu");
            }
        }

        private void ShowDeleteConfirm(KanalAltIslemResponseDto altIslem)
        {
            selectedAltIslem = altIslem;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            selectedAltIslem = null;
            showDeleteModal = false;
        }

        private async Task DeleteAltIslem()
        {
            if (selectedAltIslem == null) return;

            try
            {
                isDeleting = true;

                var result = await _kanalAltIslemService.DeleteAsync(selectedAltIslem.KanalAltIslemId);

                if (result.Success)
                {
                    allAltIslemler.Remove(selectedAltIslem);
                    ApplyFilters();
                    await _toastService.ShowSuccessAsync($"{selectedAltIslem.KanalAltAdi} başarıyla silindi");
                    HideDeleteConfirm();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Alt işlem silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alt işlem silinirken hata oluştu");
                await _toastService.ShowErrorAsync("Silme işlemi sırasında bir hata oluştu");
            }
            finally
            {
                isDeleting = false;
            }
        }

        private async Task ExportToExcel()
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Kanal Alt İşlemler");

                worksheet.Cell(1, 1).Value = "Hizmet Binası Adı";
                worksheet.Cell(1, 2).Value = "Alt İşlem Adı";
                worksheet.Cell(1, 3).Value = "Kanal İşlem";
                worksheet.Cell(1, 4).Value = "Durum";
                worksheet.Cell(1, 5).Value = "Eklenme Tarihi";

                var headerRange = worksheet.Range(1, 1, 1, 4);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                int row = 2;
                foreach (var item in filteredAltIslemler)
                {
                    worksheet.Cell(row, 1).Value = item.HizmetBinasiAdi;
                    worksheet.Cell(row, 2).Value = item.KanalAltAdi;
                    worksheet.Cell(row, 3).Value = item.KanalAdi;
                    worksheet.Cell(row, 4).Value = item.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 5).Value = item.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"KanalAltIslemler_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                await _toastService.ShowSuccessAsync("Excel dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel export error");
                await _toastService.ShowErrorAsync($"Excel oluşturulurken hata: {ex.Message}");
            }
        }

        private async Task ExportToPdf()
        {
            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(9));

                        page.Header().Text("Kanal Alt İşlem Listesi").FontSize(18).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4); // Hizmet Binası Adı
                                columns.RelativeColumn(3); // Alt İşlem
                                columns.RelativeColumn(2); // Kanal İşlem
                                columns.RelativeColumn(1); // Durum
                                columns.RelativeColumn(2); // Eklenme
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Hizmet Binası Adı").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Alt İşlem").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kanal İşlem").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eklenme").Bold();
                            });

                            foreach (var item in filteredAltIslemler)
                            {
                                table.Cell().Padding(4).Text(item.HizmetBinasiAdi);
                                table.Cell().Padding(4).Text(item.KanalAltAdi);
                                table.Cell().Padding(4).Text(item.KanalAdi);
                                table.Cell().Padding(4).Text(item.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif");
                                table.Cell().Padding(4).Text(item.EklenmeTarihi.ToString("dd.MM.yyyy"));
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
                var fileName = $"KanalAltIslemler_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(pdfBytes), "application/pdf");
                await _toastService.ShowSuccessAsync("PDF dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF export error");
                await _toastService.ShowErrorAsync($"PDF oluşturulurken hata: {ex.Message}");
            }
        }
    }
}
