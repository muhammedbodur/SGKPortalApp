using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalIslem
{
    public partial class Index
    {
        protected override string PagePermissionKey => "SIRA.KANALISLEM.INDEX";

        [Inject] private IKanalIslemApiService _kanalService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // TODO: Login olan kullanıcının bilgisini almak için
        // [Inject] private IAuthService _authService { get; set; } = default!;

        // State
        private bool isLoading = false;
        private bool isDeleting = false;
        private bool showDeleteModal = false;

        // Data
        private List<KanalIslemResponseDto> allKanallar = new();
        private List<KanalIslemResponseDto> filteredKanallar = new();
        private KanalIslemResponseDto? selectedKanal;

        // Dropdown Data
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private int selectedHizmetBinasiId = 0; // Seçili hizmet binası

        // Filters
        private string searchText = string.Empty;
        private Aktiflik? selectedAktiflik = null;
        private string sortBy = "sira-asc"; // Default: Sıraya göre

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await base.OnInitializedAsync();
            await LoadDropdownData();

            // Kullanıcının kendi Hizmet Binasını default olarak seç
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
                var result = await _kanalService.GetByHizmetBinasiIdAsync(selectedHizmetBinasiId);

                if (result.Success && result.Data != null)
                {
                    allKanallar = result.Data;
                    ApplyFilters();
                }
                else
                {
                    allKanallar = new();
                    filteredKanallar = new();
                    await _toastService.ShowInfoAsync("Seçili hizmet binasında kanal işlem bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlemleri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Kanal işlemleri yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        // Hizmet binası değiştiğinde tetiklenir
        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            // ✅ 1. YETKİ KONTROLÜ: Kullanıcı bu filtreyi değiştirebilir mi?
            if (!CanEditFieldInList("HIZMET_BINASI"))
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
                await LoadData();
            }
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
            var query = allKanallar.Where(k =>
            {
                // Arama filtresi (Kanal Adında ara)
                if (!string.IsNullOrEmpty(searchText))
                {
                    var search = searchText.ToLowerInvariant();
                    if (!k.KanalAdi.ToLowerInvariant().Contains(search))
                    {
                        return false;
                    }
                }

                // Aktiflik filtresi
                if (selectedAktiflik.HasValue && k.Aktiflik != selectedAktiflik.Value)
                {
                    return false;
                }

                return true;
            });

            // Sıralama
            filteredKanallar = sortBy switch
            {
                "name-asc" => query.OrderBy(k => k.KanalAdi).ToList(),
                "name-desc" => query.OrderByDescending(k => k.KanalAdi).ToList(),
                "sira-asc" => query.OrderBy(k => k.Sira).ToList(), // ✅ Sıraya göre
                "sira-desc" => query.OrderByDescending(k => k.Sira).ToList(),
                "date-newest" => query.OrderByDescending(k => k.EklenmeTarihi).ToList(),
                "date-oldest" => query.OrderBy(k => k.EklenmeTarihi).ToList(),
                _ => query.OrderBy(k => k.Sira).ToList() // Default: Sıraya göre
            };
        }

        private async Task ToggleAktiflik(KanalIslemResponseDto kanal)
        {
            try
            {
                var yeniAktiflik = kanal.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif;

                var updateDto = new KanalIslemUpdateRequestDto
                {
                    KanalId = kanal.KanalId,
                    HizmetBinasiId = kanal.HizmetBinasiId,
                    BaslangicNumara = kanal.BaslangicNumara,
                    BitisNumara = kanal.BitisNumara,
                    Sira = kanal.Sira,
                    Aktiflik = yeniAktiflik
                };

                var result = await _kanalService.UpdateAsync(kanal.KanalIslemId, updateDto);

                if (result.Success)
                {
                    kanal.Aktiflik = yeniAktiflik;
                    await _toastService.ShowSuccessAsync($"Durum başarıyla değiştirildi");
                    StateHasChanged();
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

        private void ShowDeleteConfirm(KanalIslemResponseDto kanal)
        {
            selectedKanal = kanal;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            selectedKanal = null;
            showDeleteModal = false;
        }

        private async Task DeleteKanal()
        {
            if (selectedKanal == null) return;

            try
            {
                isDeleting = true;

                var result = await _kanalService.DeleteAsync(selectedKanal.KanalIslemId);

                if (result.Success)
                {
                    allKanallar.Remove(selectedKanal);
                    ApplyFilters();
                    await _toastService.ShowSuccessAsync($"{selectedKanal.KanalAdi} başarıyla silindi");
                    HideDeleteConfirm();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem silinirken hata oluştu");
                await _toastService.ShowErrorAsync("Silme işlemi sırasında bir hata oluştu");
            }
            finally
            {
                isDeleting = false;
            }
        }

        private void OnSortChanged(ChangeEventArgs e)
        {
            sortBy = e.Value?.ToString() ?? "sira-asc";
            ApplyFilters();
        }

        private void ClearFilters()
        {
            searchText = string.Empty;
            selectedAktiflik = null;
            sortBy = "sira-asc";
            ApplyFilters();
        }

        private async Task ExportToExcel()
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Kanal İşlemleri");

                // Header
                worksheet.Cell(1, 1).Value = "Kanal Adı";
                worksheet.Cell(1, 2).Value = "Hizmet Binası";
                worksheet.Cell(1, 3).Value = "Başlangıç No";
                worksheet.Cell(1, 4).Value = "Bitiş No";
                worksheet.Cell(1, 5).Value = "Sıra";
                worksheet.Cell(1, 6).Value = "Durum";
                worksheet.Cell(1, 7).Value = "Eklenme Tarihi";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var kanal in filteredKanallar)
                {
                    worksheet.Cell(row, 1).Value = kanal.KanalAdi;
                    worksheet.Cell(row, 2).Value = kanal.HizmetBinasiAdi;
                    worksheet.Cell(row, 3).Value = kanal.BaslangicNumara;
                    worksheet.Cell(row, 4).Value = kanal.BitisNumara;
                    worksheet.Cell(row, 5).Value = kanal.Sira;
                    worksheet.Cell(row, 6).Value = kanal.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 7).Value = kanal.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"KanalIslemleri_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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

                        page.Header().Text("Kanal İşlem Listesi").FontSize(18).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Kanal Adı
                                columns.RelativeColumn(2); // Hizmet Binası
                                columns.RelativeColumn(1); // Başlangıç
                                columns.RelativeColumn(1); // Bitiş
                                columns.RelativeColumn(1); // Sıra
                                columns.RelativeColumn(1); // Durum
                                columns.RelativeColumn(2); // Tarih
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kanal").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Hizmet Binası").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Başlangıç").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Bitiş").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Sıra").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eklenme").Bold();
                            });

                            foreach (var kanal in filteredKanallar)
                            {
                                table.Cell().Padding(4).Text(kanal.KanalAdi);
                                table.Cell().Padding(4).Text(kanal.HizmetBinasiAdi);
                                table.Cell().Padding(4).Text(kanal.BaslangicNumara.ToString());
                                table.Cell().Padding(4).Text(kanal.BitisNumara.ToString());
                                table.Cell().Padding(4).Text(kanal.Sira.ToString());
                                table.Cell().Padding(4).Text(kanal.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif");
                                table.Cell().Padding(4).Text(kanal.EklenmeTarihi.ToString("dd.MM.yyyy"));
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
                var fileName = $"KanalIslemleri_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

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