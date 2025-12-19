using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalAlt
{
    public partial class Index
    {

        [Inject] private IKanalAltApiService _kanalAltService { get; set; } = default!;
        [Inject] private IKanalApiService _kanalService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // URL Parameters
        [SupplyParameterFromQuery(Name = "kanalId")]
        public int? KanalIdParam { get; set; }
        
        // Lock state
        private bool isKanalLocked = false;

        // State
        private bool isLoading = false;
        private bool isDeleting = false;
        private bool showDeleteModal = false;

        // Data
        private List<KanalAltResponseDto> allKanalAltlar = new();
        private List<KanalAltResponseDto> filteredKanalAltlar = new();
        private List<KanalResponseDto> anaKanallar = new();
        private KanalAltResponseDto? selectedKanalAlt;

        // Filters
        private string searchText = string.Empty;
        private Aktiflik? selectedAktiflik = null;
        private int selectedKanalId = 0;
        private string sortBy = "name-asc";

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadData();
            
            // URL'den kanalId parametresi geldiyse
            if (KanalIdParam.HasValue && KanalIdParam.Value > 0)
            {
                selectedKanalId = KanalIdParam.Value;
                isKanalLocked = true;
                FilterKanalAltlar();
            }
        }
        
        private void UnlockFilters()
        {
            isKanalLocked = false;
            selectedKanalId = 0;
            FilterKanalAltlar();
        }

        private async Task LoadData()
        {
            try
            {
                isLoading = true;

                // Ana kanalları yükle
                var kanalResult = await _kanalService.GetAllAsync();
                if (kanalResult.Success && kanalResult.Data != null)
                {
                    anaKanallar = kanalResult.Data;
                }

                // Alt kanalları yükle
                var result = await _kanalAltService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    allKanalAltlar = result.Data;
                    FilterKanalAltlar();
                }
                else
                {
                    allKanalAltlar = new();
                    filteredKanalAltlar = new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alt kanallar yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Alt kanallar yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void FilterKanalAltlar()
        {
            var query = allKanalAltlar.AsEnumerable();

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(k => k.KanalAltAdi.ContainsTurkish(searchText, StringComparison.OrdinalIgnoreCase));
            }

            // Ana kanal filtresi
            if (selectedKanalId > 0)
            {
                query = query.Where(k => k.KanalId == selectedKanalId);
            }

            // Aktiflik filtresi
            if (selectedAktiflik.HasValue)
            {
                query = query.Where(k => k.Aktiflik == selectedAktiflik.Value);
            }

            // Sıralama
            query = sortBy switch
            {
                "name-asc" => query.OrderBy(k => k.KanalAltAdi),
                "name-desc" => query.OrderByDescending(k => k.KanalAltAdi),
                "date-desc" => query.OrderByDescending(k => k.EklenmeTarihi),
                "date-asc" => query.OrderBy(k => k.EklenmeTarihi),
                "islem-desc" => query.OrderByDescending(k => k.KanalAltIslemSayisi),
                "islem-asc" => query.OrderBy(k => k.KanalAltIslemSayisi),
                _ => query.OrderBy(k => k.KanalAltAdi)
            };

            filteredKanalAltlar = query.ToList();
        }

        private void SetAktiflikFilter(Aktiflik? aktiflik)
        {
            selectedAktiflik = aktiflik;
            FilterKanalAltlar();
        }

        private void ClearFilters()
        {
            searchText = string.Empty;
            selectedAktiflik = null;
            selectedKanalId = 0;
            sortBy = "name-asc";
            FilterKanalAltlar();
        }

        private async Task ToggleAktiflik(KanalAltResponseDto kanalAlt)
        {
            try
            {
                var newAktiflik = kanalAlt.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif;

                var updateDto = new KanalAltKanalUpdateRequestDto
                {
                    KanalId = kanalAlt.KanalId,
                    KanalAltAdi = kanalAlt.KanalAltAdi,
                    Aktiflik = newAktiflik
                };

                var result = await _kanalAltService.UpdateAsync(kanalAlt.KanalAltId, updateDto);

                if (result.Success)
                {
                    kanalAlt.Aktiflik = newAktiflik;
                    await _toastService.ShowSuccessAsync($"{kanalAlt.KanalAltAdi} {(newAktiflik == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı");
                    FilterKanalAltlar();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Durum değiştirilemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktiflik değiştirilirken hata oluştu");
                await _toastService.ShowErrorAsync("İşlem sırasında bir hata oluştu");
            }
        }

        private void ShowDeleteConfirm(KanalAltResponseDto kanalAlt)
        {
            selectedKanalAlt = kanalAlt;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            selectedKanalAlt = null;
            showDeleteModal = false;
        }

        private async Task DeleteKanalAlt()
        {
            if (selectedKanalAlt == null) return;

            try
            {
                isDeleting = true;
                var result = await _kanalAltService.DeleteAsync(selectedKanalAlt.KanalAltId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"{selectedKanalAlt.KanalAltAdi} başarıyla silindi");
                    allKanalAltlar.Remove(selectedKanalAlt);
                    FilterKanalAltlar();
                    HideDeleteConfirm();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Alt kanal silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alt kanal silinirken hata oluştu");
                await _toastService.ShowErrorAsync("Alt kanal silinirken bir hata oluştu");
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
                var worksheet = workbook.Worksheets.Add("Alt Kanallar");

                // Header
                worksheet.Cell(1, 1).Value = "Alt Kanal Adı";
                worksheet.Cell(1, 2).Value = "Ana Kanal";
                worksheet.Cell(1, 3).Value = "Alt İşlem Sayısı";
                worksheet.Cell(1, 4).Value = "Durum";
                worksheet.Cell(1, 5).Value = "Eklenme Tarihi";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var kanalAlt in filteredKanalAltlar)
                {
                    worksheet.Cell(row, 1).Value = kanalAlt.KanalAltAdi;
                    worksheet.Cell(row, 2).Value = kanalAlt.KanalAdi;
                    worksheet.Cell(row, 3).Value = kanalAlt.KanalAltIslemSayisi;
                    worksheet.Cell(row, 4).Value = kanalAlt.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 5).Value = kanalAlt.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"AltKanallar_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Text("Alt Kanal Listesi").FontSize(20).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Alt Kanal").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ana Kanal").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Alt İşlem").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eklenme").Bold();
                            });

                            foreach (var kanalAlt in filteredKanalAltlar)
                            {
                                table.Cell().Padding(5).Text(kanalAlt.KanalAltAdi);
                                table.Cell().Padding(5).Text(kanalAlt.KanalAdi);
                                table.Cell().Padding(5).Text(kanalAlt.KanalAltIslemSayisi.ToString());
                                table.Cell().Padding(5).Text(kanalAlt.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif");
                                table.Cell().Padding(5).Text(kanalAlt.EklenmeTarihi.ToString("dd.MM.yyyy"));
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
                var fileName = $"AltKanallar_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

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
