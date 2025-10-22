using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Concrete;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalIslem
{
    public partial class Index
    {
        [Inject] private IKanalIslemApiService _kanalService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // State
        private bool isLoading = false;
        private bool isDeleting = false;
        private bool showDeleteModal = false;

        // Data
        private List<KanalIslemResponseDto> allKanallar = new();
        private List<KanalIslemResponseDto> filteredKanallar = new();
        private KanalIslemResponseDto? selectedKanal;

        // Filters
        private string searchText = string.Empty;
        private Aktiflik? selectedAktiflik = null;
        private string sortBy = "name-asc";

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                isLoading = true;
                var result = await _kanalService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    allKanallar = result.Data;
                    ApplyFilters();
                }
                else
                {
                    allKanallar = new();
                    filteredKanallar = new();
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

        private void ApplyFilters()
        {
            var query = allKanallar.Where(k =>
            {
                // Arama filtresi
                if (!string.IsNullOrEmpty(searchText))
                {
                    var search = searchText.ToLowerInvariant();
                    if (!k.KanalIslemAdi.ToLowerInvariant().Contains(search))
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
                "name-desc" => query.OrderByDescending(k => k.KanalIslemAdi).ToList(),
                "date-newest" => query.OrderByDescending(k => k.EklenmeTarihi).ToList(),
                "date-oldest" => query.OrderBy(k => k.EklenmeTarihi).ToList(),
                _ => query.OrderBy(k => k.KanalIslemAdi).ToList() // name-asc (default)
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
                    KanalIslemAdi = kanal.KanalIslemAdi,
                    Sira = kanal.Sira,
                    Aktiflik = yeniAktiflik
                };

                var result = await _kanalService.UpdateAsync(kanal.KanalIslemId, updateDto);

                if (result.Success)
                {
                    kanal.Aktiflik = yeniAktiflik;
                    kanal.DuzenlenmeTarihi = DateTime.Now;

                    var mesaj = yeniAktiflik == Aktiflik.Aktif
                        ? $"{kanal.KanalIslemAdi} aktif hale getirildi"
                        : $"{kanal.KanalIslemAdi} pasif hale getirildi";

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
                    await _toastService.ShowSuccessAsync($"{selectedKanal.KanalIslemAdi} başarıyla silindi");
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
            sortBy = e.Value?.ToString() ?? "name-asc";
            ApplyFilters();
        }

        private void ClearFilters()
        {
            searchText = string.Empty;
            selectedAktiflik = null;
            sortBy = "name-asc";
            ApplyFilters();
        }

        private async Task ExportToExcel()
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Kanal İşlemleri");

                // Header
                worksheet.Cell(1, 1).Value = "Kanal İşlem Adı";
                worksheet.Cell(1, 2).Value = "Ana Kanal";
                worksheet.Cell(1, 3).Value = "Sıra";
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
                    worksheet.Cell(row, 1).Value = kanal.KanalIslemAdi;
                    worksheet.Cell(row, 2).Value = kanal.KanalAdi;
                    worksheet.Cell(row, 3).Value = kanal.Sira;
                    worksheet.Cell(row, 4).Value = kanal.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 5).Value = kanal.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
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
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kanal İşlem").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ana Kanal").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Sıra").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eklenme").Bold();
                            });

                            foreach (var kanal in filteredKanallar)
                            {
                                table.Cell().Padding(4).Text(kanal.KanalIslemAdi);
                                table.Cell().Padding(4).Text(kanal.KanalAdi);
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
