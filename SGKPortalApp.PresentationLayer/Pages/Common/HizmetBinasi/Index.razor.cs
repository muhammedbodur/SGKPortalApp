using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.HizmetBinasi
{
    public partial class Index
    {
        protected override string PagePermissionKey => "PORTAL.HIZMETBINASI.INDEX";
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<HizmetBinasiResponseDto> HizmetBinalari { get; set; } = new();
        private List<HizmetBinasiResponseDto> FilteredHizmetBinalari { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string searchTerm = string.Empty;
        private string filterStatus = "all";
        private string sortBy = "name-asc";

        // ═══════════════════════════════════════════════════════
        // PAGINATION PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => (int)Math.Ceiling(FilteredHizmetBinalari.Count / (double)PageSize);

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = true;
        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════
        // TOGGLE STATUS MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowToggleModal { get; set; } = false;
        private int ToggleHizmetBinasiId { get; set; }
        private string ToggleHizmetBinasiAdi { get; set; } = string.Empty;
        private Aktiflik ToggleHizmetBinasiCurrentStatus { get; set; }
        private bool IsToggling { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowDeleteModal { get; set; } = false;
        private int DeleteHizmetBinasiId { get; set; }
        private string DeleteHizmetBinasiAdi { get; set; } = string.Empty;
        private int DeleteHizmetBinasiPersonelSayisi { get; set; }
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadHizmetBinalari();
        }

        private async Task LoadHizmetBinalari()
        {
            IsLoading = true;
            try
            {
                var result = await _hizmetBinasiService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    HizmetBinalari = result.Data;
                    ApplyFiltersAndSort();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "HizmetBinalari yüklenemedi!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("HizmetBinalari yüklenirken bir hata oluştu!");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // FİLTRELEME VE SIRALAMA
        // ═══════════════════════════════════════════════════════

        private void OnSearchChanged()
        {
            CurrentPage = 1;
            ApplyFiltersAndSort();
        }

        private void OnFilterStatusChanged(ChangeEventArgs e)
        {
            filterStatus = e.Value?.ToString() ?? "all";
            CurrentPage = 1;
            ApplyFiltersAndSort();
        }

        private void OnSortByChanged(ChangeEventArgs e)
        {
            sortBy = e.Value?.ToString() ?? "name-asc";
            ApplyFiltersAndSort();
        }

        private void ClearFilters()
        {
            searchTerm = string.Empty;
            filterStatus = "all";
            sortBy = "name-asc";
            CurrentPage = 1;
            ApplyFiltersAndSort();
        }

        private void ApplyFiltersAndSort()
        {
            var query = HizmetBinalari.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.HizmetBinasiAdi.ContainsTurkish(searchTerm));
            }

            query = filterStatus switch
            {
                "active" => query.Where(d => d.Aktiflik == Aktiflik.Aktif),
                "passive" => query.Where(d => d.Aktiflik == Aktiflik.Pasif),
                _ => query
            };

            query = sortBy switch
            {
                "name-asc" => query.OrderBy(d => d.HizmetBinasiAdi),
                "name-desc" => query.OrderByDescending(d => d.HizmetBinasiAdi),
                "date-newest" => query.OrderByDescending(d => d.EklenmeTarihi),
                "date-oldest" => query.OrderBy(d => d.EklenmeTarihi),
                "personel-most" => query.OrderByDescending(d => d.PersonelSayisi),
                "personel-least" => query.OrderBy(d => d.PersonelSayisi),
                _ => query.OrderBy(d => d.HizmetBinasiAdi)
            };

            FilteredHizmetBinalari = query.ToList();
        }

        // ═══════════════════════════════════════════════════════
        // PAGİNATİON
        // ═══════════════════════════════════════════════════════

        private void ChangePage(int page)
        {
            if (page < 1 || page > TotalPages) return;
            CurrentPage = page;
            StateHasChanged();
        }

        // ═══════════════════════════════════════════════════════
        // NAVİGASYON
        // ═══════════════════════════════════════════════════════

        private void NavigateToCreate() => _navigationManager.NavigateTo("/common/hizmetbinasi/manage");
        private void NavigateToEdit(int id) => _navigationManager.NavigateTo($"/common/hizmetbinasi/manage/{id}");
        private void NavigateToDetail(int id) => _navigationManager.NavigateTo($"/common/hizmetbinasi/detail/{id}");

        // ═══════════════════════════════════════════════════════
        // TOGGLE STATUS MODAL
        // ═══════════════════════════════════════════════════════

        private void ShowToggleStatusConfirmation(int hizmetBinasiId, string hizmetBinasiAdi, Aktiflik currentStatus)
        {
            ToggleHizmetBinasiId = hizmetBinasiId;
            ToggleHizmetBinasiAdi = hizmetBinasiAdi;
            ToggleHizmetBinasiCurrentStatus = currentStatus;
            ShowToggleModal = true;
        }

        private void CloseToggleModal()
        {
            ShowToggleModal = false;
            ToggleHizmetBinasiId = 0;
            ToggleHizmetBinasiAdi = string.Empty;
        }

        private async Task ConfirmToggleStatus()
        {
            IsToggling = true;
            try
            {
                var hizmetBinasi = HizmetBinalari.FirstOrDefault(d => d.HizmetBinasiId == ToggleHizmetBinasiId);
                if (hizmetBinasi == null) return;

                var newStatus = hizmetBinasi.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif;
                var updateDto = new BusinessObjectLayer.DTOs.Request.Common.HizmetBinasiUpdateRequestDto
                {
                    HizmetBinasiAdi = hizmetBinasi.HizmetBinasiAdi,
                    DepartmanId = hizmetBinasi.DepartmanId,
                    Adres = hizmetBinasi.Adres ?? string.Empty,
                    Aktiflik = newStatus
                };

                var result = await _hizmetBinasiService.UpdateAsync(ToggleHizmetBinasiId, updateDto);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"Hizmet Binası {(newStatus == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı!");
                    CloseToggleModal();
                    await LoadHizmetBinalari();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Durum değiştirilemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsToggling = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL
        // ═══════════════════════════════════════════════════════

        private void ShowDeleteConfirmation(int hizmetBinasiId, string hizmetBinasiAdi, int personelSayisi)
        {
            DeleteHizmetBinasiId = hizmetBinasiId;
            DeleteHizmetBinasiAdi = hizmetBinasiAdi;
            DeleteHizmetBinasiPersonelSayisi = personelSayisi;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeleteHizmetBinasiId = 0;
            DeleteHizmetBinasiAdi = string.Empty;
            DeleteHizmetBinasiPersonelSayisi = 0;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _hizmetBinasiService.DeleteAsync(DeleteHizmetBinasiId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "HizmetBinasi başarıyla silindi.");
                    await LoadHizmetBinalari();
                    CloseDeleteModal();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "HizmetBinasi silinemedi!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Silme işlemi başarısız!");
            }
            finally
            {
                IsDeleting = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // EXCEL EXPORT (ClosedXML)
        // ═══════════════════════════════════════════════════════

        private async Task ExportToExcel()
        {
            IsExporting = true;
            ExportType = "excel";

            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("HizmetBinalari");

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                headerRow.Style.Font.FontColor = XLColor.White;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(1, 1).Value = "HizmetBinasi Adı";
                worksheet.Cell(1, 2).Value = "Personel Sayısı";
                worksheet.Cell(1, 3).Value = "Durum";
                worksheet.Cell(1, 4).Value = "Eklenme Tarihi";
                worksheet.Cell(1, 5).Value = "Son Güncelleme";

                int row = 2;
                foreach (var hizmetBinasi in FilteredHizmetBinalari)
                {
                    worksheet.Cell(row, 1).Value = hizmetBinasi.HizmetBinasiAdi;
                    worksheet.Cell(row, 2).Value = hizmetBinasi.PersonelSayisi;
                    worksheet.Cell(row, 3).Value = hizmetBinasi.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 4).Value = hizmetBinasi.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(row, 5).Value = hizmetBinasi.DuzenlenmeTarihi.ToString("dd.MM.yyyy HH:mm");

                    worksheet.Cell(row, 3).Style.Fill.BackgroundColor =
                        hizmetBinasi.Aktiflik == Aktiflik.Aktif ? XLColor.LightGreen : XLColor.LightPink;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                var fileName = $"HizmetBinalari_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                await _toastService.ShowSuccessAsync("Excel dosyası indirildi!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excel Hatası: {ex.Message}");
                await _toastService.ShowErrorAsync($"Excel hatası: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }

        // ═══════════════════════════════════════════════════════
        // PDF EXPORT (QuestPDF)
        // ═══════════════════════════════════════════════════════

        private async Task ExportToPdf()
        {
            IsExporting = true;
            ExportType = "pdf";

            try
            {
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("DEPARTMAN LİSTESİ").FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                                column.Item().Text($"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                            });
                        });

                        page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Border(1).Padding(5).Column(col =>
                                {
                                    col.Item().Text("Toplam").Bold();
                                    col.Item().Text(HizmetBinalari.Count.ToString()).FontSize(14).FontColor(Colors.Blue.Darken2);
                                });

                                row.RelativeItem().Border(1).Padding(5).Column(col =>
                                {
                                    col.Item().Text("Aktif").Bold();
                                    col.Item().Text(HizmetBinalari.Count(d => d.Aktiflik == Aktiflik.Aktif).ToString()).FontSize(14).FontColor(Colors.Green.Darken2);
                                });

                                row.RelativeItem().Border(1).Padding(5).Column(col =>
                                {
                                    col.Item().Text("Pasif").Bold();
                                    col.Item().Text(HizmetBinalari.Count(d => d.Aktiflik == Aktiflik.Pasif).ToString()).FontSize(14).FontColor(Colors.Red.Darken2);
                                });
                            });

                            column.Item().PaddingTop(15);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("HizmetBinasi").Bold();
                                    header.Cell().Element(CellStyle).Text("Personel").Bold();
                                    header.Cell().Element(CellStyle).Text("Durum").Bold();
                                    header.Cell().Element(CellStyle).Text("Tarih").Bold();

                                    static IContainer CellStyle(IContainer container) => container.Border(1).Background(Colors.Grey.Lighten3).Padding(5).AlignCenter();
                                });

                                foreach (var d in FilteredHizmetBinalari)
                                {
                                    table.Cell().Border(1).Padding(5).Text(d.HizmetBinasiAdi);
                                    table.Cell().Border(1).Padding(5).AlignCenter().Text(d.PersonelSayisi.ToString());
                                    table.Cell().Border(1).Padding(5).AlignCenter().Text(d.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif").FontColor(d.Aktiflik == Aktiflik.Aktif ? Colors.Green.Darken2 : Colors.Red.Darken2);
                                    table.Cell().Border(1).Padding(5).AlignCenter().Text(d.EklenmeTarihi.ToString("dd.MM.yyyy"));
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(t =>
                        {
                            t.Span("Sayfa ");
                            t.CurrentPageNumber();
                            t.Span(" / ");
                            t.TotalPages();
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                var fileName = $"HizmetBinalari_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(pdfBytes), "application/pdf");

                await _toastService.ShowSuccessAsync("PDF dosyası indirildi!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PDF Hatası: {ex.Message}");
                await _toastService.ShowErrorAsync($"PDF hatası: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }
    }
}
