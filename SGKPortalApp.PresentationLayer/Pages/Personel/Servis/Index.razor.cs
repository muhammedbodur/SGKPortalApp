using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Servis
{
    public partial class Index : ComponentBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IServisApiService _servisService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<ServisResponseDto> Servisler { get; set; } = new();
        private List<ServisResponseDto> FilteredServisler { get; set; } = new();

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
        private int TotalPages => (int)Math.Ceiling(FilteredServisler.Count / (double)PageSize);

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
        private int ToggleServisId { get; set; }
        private string ToggleServisAdi { get; set; } = string.Empty;
        private Aktiflik ToggleServisCurrentStatus { get; set; }
        private bool IsToggling { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowDeleteModal { get; set; } = false;
        private int DeleteServisId { get; set; }
        private string DeleteServisAdi { get; set; } = string.Empty;
        private int DeleteServisPersonelSayisi { get; set; }
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadServisler();
        }

        private async Task LoadServisler()
        {
            IsLoading = true;
            try
            {
                var result = await _servisService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    Servisler = result.Data;
                    ApplyFiltersAndSort();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Servisler yüklenemedi!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Servisler yüklenirken bir hata oluştu!");
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
            var query = Servisler.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.ServisAdi.ContainsTurkish(searchTerm));
            }

            query = filterStatus switch
            {
                "active" => query.Where(d => d.Aktiflik == Aktiflik.Aktif),
                "passive" => query.Where(d => d.Aktiflik == Aktiflik.Pasif),
                _ => query
            };

            query = sortBy switch
            {
                "name-asc" => query.OrderBy(d => d.ServisAdi),
                "name-desc" => query.OrderByDescending(d => d.ServisAdi),
                "date-newest" => query.OrderByDescending(d => d.EklenmeTarihi),
                "date-oldest" => query.OrderBy(d => d.EklenmeTarihi),
                "personel-most" => query.OrderByDescending(d => d.PersonelSayisi),
                "personel-least" => query.OrderBy(d => d.PersonelSayisi),
                _ => query.OrderBy(d => d.ServisAdi)
            };

            FilteredServisler = query.ToList();
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

        private void NavigateToCreate() => _navigationManager.NavigateTo("/personel/Servis/manage");
        private void NavigateToEdit(int id) => _navigationManager.NavigateTo($"/personel/Servis/manage/{id}");

        // ═══════════════════════════════════════════════════════
        // TOGGLE STATUS MODAL
        // ═══════════════════════════════════════════════════════

        private void ShowToggleStatusConfirmation(int ServisId, string ServisAdi, Aktiflik currentStatus)
        {
            ToggleServisId = ServisId;
            ToggleServisAdi = ServisAdi;
            ToggleServisCurrentStatus = currentStatus;
            ShowToggleModal = true;
        }

        private void CloseToggleModal()
        {
            ShowToggleModal = false;
            ToggleServisId = 0;
            ToggleServisAdi = string.Empty;
        }

        private async Task ConfirmToggleStatus()
        {
            IsToggling = true;
            try
            {
                var servis = Servisler.FirstOrDefault(d => d.ServisId == ToggleServisId);
                if (servis == null) return;

                var newStatus = servis.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif;
                var updateDto = new BusinessObjectLayer.DTOs.Request.PersonelIslemleri.ServisUpdateRequestDto
                {
                    ServisAdi = servis.ServisAdi,
                    Aktiflik = newStatus
                };

                var result = await _servisService.UpdateAsync(ToggleServisId, updateDto);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"Servis {(newStatus == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı!");
                    CloseToggleModal();
                    await LoadServisler();
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

        private void ShowDeleteConfirmation(int ServisId, string ServisAdi, int personelSayisi)
        {
            DeleteServisId = ServisId;
            DeleteServisAdi = ServisAdi;
            DeleteServisPersonelSayisi = personelSayisi;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeleteServisId = 0;
            DeleteServisAdi = string.Empty;
            DeleteServisPersonelSayisi = 0;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _servisService.DeleteAsync(DeleteServisId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "Servis başarıyla silindi.");
                    await LoadServisler();
                    CloseDeleteModal();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Servis silinemedi!");
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
                var worksheet = workbook.Worksheets.Add("Servisler");

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                headerRow.Style.Font.FontColor = XLColor.White;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(1, 1).Value = "Servis Adı";
                worksheet.Cell(1, 2).Value = "Personel Sayısı";
                worksheet.Cell(1, 3).Value = "Durum";
                worksheet.Cell(1, 4).Value = "Eklenme Tarihi";
                worksheet.Cell(1, 5).Value = "Son Güncelleme";

                int row = 2;
                foreach (var Servis in FilteredServisler)
                {
                    worksheet.Cell(row, 1).Value = Servis.ServisAdi;
                    worksheet.Cell(row, 2).Value = Servis.PersonelSayisi;
                    worksheet.Cell(row, 3).Value = Servis.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 4).Value = Servis.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(row, 5).Value = Servis.DuzenlenmeTarihi.ToString("dd.MM.yyyy HH:mm");

                    worksheet.Cell(row, 3).Style.Fill.BackgroundColor =
                        Servis.Aktiflik == Aktiflik.Aktif ? XLColor.LightGreen : XLColor.LightPink;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                var fileName = $"Servisler_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
                                column.Item().Text("Servis LİSTESİ").FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
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
                                    col.Item().Text(Servisler.Count.ToString()).FontSize(14).FontColor(Colors.Blue.Darken2);
                                });

                                row.RelativeItem().Border(1).Padding(5).Column(col =>
                                {
                                    col.Item().Text("Aktif").Bold();
                                    col.Item().Text(Servisler.Count(d => d.Aktiflik == Aktiflik.Aktif).ToString()).FontSize(14).FontColor(Colors.Green.Darken2);
                                });

                                row.RelativeItem().Border(1).Padding(5).Column(col =>
                                {
                                    col.Item().Text("Pasif").Bold();
                                    col.Item().Text(Servisler.Count(d => d.Aktiflik == Aktiflik.Pasif).ToString()).FontSize(14).FontColor(Colors.Red.Darken2);
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
                                    header.Cell().Element(CellStyle).Text("Servis").Bold();
                                    header.Cell().Element(CellStyle).Text("Personel").Bold();
                                    header.Cell().Element(CellStyle).Text("Durum").Bold();
                                    header.Cell().Element(CellStyle).Text("Tarih").Bold();

                                    static IContainer CellStyle(IContainer container) => container.Border(1).Background(Colors.Grey.Lighten3).Padding(5).AlignCenter();
                                });

                                foreach (var d in FilteredServisler)
                                {
                                    table.Cell().Border(1).Padding(5).Text(d.ServisAdi);
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
                var fileName = $"Servisler_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
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
