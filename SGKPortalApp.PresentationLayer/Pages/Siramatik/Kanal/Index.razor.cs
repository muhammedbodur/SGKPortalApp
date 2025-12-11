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

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Kanal
{
    public partial class Index
    {
        [Inject] private IKanalApiService _kanalService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // State
        private bool isLoading = false;
        private bool isDeleting = false;
        private bool showDeleteModal = false;

        // Data
        private List<KanalResponseDto> allKanallar = new();
        private List<KanalResponseDto> filteredKanallar = new();
        private KanalResponseDto? selectedKanal;

        // Filters
        private string searchText = string.Empty;
        private Aktiflik? selectedAktiflik = null;
        private string sortBy = "name-asc";

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadKanallar();
        }

        private async Task LoadKanallar()
        {
            try
            {
                isLoading = true;
                var result = await _kanalService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    allKanallar = result.Data;
                    FilterKanallar();
                }
                else
                {
                    allKanallar = new();
                    filteredKanallar = new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ana kanallar yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Ana kanallar yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void FilterKanallar()
        {
            var query = allKanallar.AsEnumerable();

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(k => k.KanalAdi.ContainsTurkish(searchText, StringComparison.OrdinalIgnoreCase));
            }

            // Aktiflik filtresi
            if (selectedAktiflik.HasValue)
            {
                query = query.Where(k => k.Aktiflik == selectedAktiflik.Value);
            }

            // Sıralama
            query = sortBy switch
            {
                "name-asc" => query.OrderBy(k => k.KanalAdi),
                "name-desc" => query.OrderByDescending(k => k.KanalAdi),
                "date-desc" => query.OrderByDescending(k => k.EklenmeTarihi),
                "date-asc" => query.OrderBy(k => k.EklenmeTarihi),
                "altkanal-desc" => query.OrderByDescending(k => k.KanalAltSayisi),
                "altkanal-asc" => query.OrderBy(k => k.KanalAltSayisi),
                _ => query.OrderBy(k => k.KanalAdi)
            };

            filteredKanallar = query.ToList();
        }

        private void SetAktiflikFilter(Aktiflik? aktiflik)
        {
            selectedAktiflik = aktiflik;
            FilterKanallar();
        }

        private void ClearFilters()
        {
            searchText = string.Empty;
            selectedAktiflik = null;
            sortBy = "name-asc";
            FilterKanallar();
        }

        private async Task ToggleAktiflik(KanalResponseDto kanal)
        {
            try
            {
                var newAktiflik = kanal.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif;

                var updateDto = new KanalUpdateRequestDto
                {
                    KanalAdi = kanal.KanalAdi,
                    Aktiflik = newAktiflik
                };

                var result = await _kanalService.UpdateAsync(kanal.KanalId, updateDto);

                if (result.Success)
                {
                    kanal.Aktiflik = newAktiflik;
                    await _toastService.ShowSuccessAsync($"{kanal.KanalAdi} {(newAktiflik == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı");
                    FilterKanallar();
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

        private void ShowDeleteConfirm(KanalResponseDto kanal)
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
                var result = await _kanalService.DeleteAsync(selectedKanal.KanalId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"{selectedKanal.KanalAdi} başarıyla silindi");
                    allKanallar.Remove(selectedKanal);
                    FilterKanallar();
                    HideDeleteConfirm();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Ana kanal silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ana kanal silinirken hata oluştu");
                await _toastService.ShowErrorAsync("Ana kanal silinirken bir hata oluştu");
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
                var worksheet = workbook.Worksheets.Add("Ana Kanallar");

                // Header
                worksheet.Cell(1, 1).Value = "Ana Kanal Adı";
                worksheet.Cell(1, 2).Value = "Kanal İşlem Sayısı";
                worksheet.Cell(1, 3).Value = "Alt İşlem Sayısı";
                worksheet.Cell(1, 4).Value = "Durum";
                worksheet.Cell(1, 5).Value = "Eklenme Tarihi";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var kanal in filteredKanallar)
                {
                    worksheet.Cell(row, 1).Value = kanal.KanalAdi;
                    worksheet.Cell(row, 2).Value = kanal.KanalIslemSayisi;
                    worksheet.Cell(row, 3).Value = kanal.KanalAltSayisi;
                    worksheet.Cell(row, 4).Value = kanal.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 5).Value = kanal.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"AnaKanallar_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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

                        page.Header().Text("Ana Kanal Listesi").FontSize(20).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ana Kanal").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kanal İşlem").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Alt İşlem").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eklenme").Bold();
                            });

                            foreach (var kanal in filteredKanallar)
                            {
                                table.Cell().Padding(5).Text(kanal.KanalAdi);
                                table.Cell().Padding(5).Text(kanal.KanalIslemSayisi.ToString());
                                table.Cell().Padding(5).Text(kanal.KanalAltSayisi.ToString());
                                table.Cell().Padding(5).Text(kanal.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif");
                                table.Cell().Padding(5).Text(kanal.EklenmeTarihi.ToString("dd.MM.yyyy"));
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
                var fileName = $"AnaKanallar_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

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
