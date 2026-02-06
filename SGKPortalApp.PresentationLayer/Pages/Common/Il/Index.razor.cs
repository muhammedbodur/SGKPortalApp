using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.Il
{
    public partial class Index
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IIlApiService _ilService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<IlResponseDto> Iller { get; set; } = new();
        private List<IlResponseDto> FilteredIller { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string searchTerm = string.Empty;
        private string sortBy = "name-asc";

        // ═══════════════════════════════════════════════════════
        // PAGINATION PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => (int)Math.Ceiling(FilteredIller.Count / (double)PageSize);

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = true;
        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowDeleteModal { get; set; } = false;
        private int DeleteIlId { get; set; }
        private string DeleteIlAdi { get; set; } = string.Empty;
        private int DeleteIlIlceSayisi { get; set; }
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadIller();
        }

        private async Task LoadIller()
        {
            IsLoading = true;
            try
            {
                var result = await _ilService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    Iller = result.Data;
                    ApplyFiltersAndSort();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İller yüklenemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // FILTER & SORT METHODS
        // ═══════════════════════════════════════════════════════

        private void ApplyFiltersAndSort()
        {
            var filtered = Iller.AsEnumerable();

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.Where(i =>
                    i.IlAdi.ContainsTurkish(searchTerm));
            }

            // Sıralama
            filtered = sortBy switch
            {
                "name-asc" => filtered.OrderBy(i => i.IlAdi),
                "name-desc" => filtered.OrderByDescending(i => i.IlAdi),
                "date-newest" => filtered.OrderByDescending(i => i.EklenmeTarihi),
                "date-oldest" => filtered.OrderBy(i => i.EklenmeTarihi),
                "ilce-most" => filtered.OrderByDescending(i => i.IlceSayisi),
                "ilce-least" => filtered.OrderBy(i => i.IlceSayisi),
                _ => filtered.OrderBy(i => i.IlAdi)
            };

            FilteredIller = filtered.ToList();
            CurrentPage = 1;
        }

        private void OnSearchChanged()
        {
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
            sortBy = "name-asc";
            ApplyFiltersAndSort();
        }

        // ═══════════════════════════════════════════════════════
        // PAGINATION METHODS
        // ═══════════════════════════════════════════════════════

        private void ChangePage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
            }
        }

        // ═══════════════════════════════════════════════════════
        // NAVIGATION METHODS
        // ═══════════════════════════════════════════════════════

        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/common/il/add");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/common/il/manage/{id}");
        }

        // ═══════════════════════════════════════════════════════
        // DELETE METHODS
        // ═══════════════════════════════════════════════════════

        private void ShowDeleteConfirmation(int id, string ilAdi, int ilceSayisi)
        {
            DeleteIlId = id;
            DeleteIlAdi = ilAdi;
            DeleteIlIlceSayisi = ilceSayisi;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeleteIlId = 0;
            DeleteIlAdi = string.Empty;
            DeleteIlIlceSayisi = 0;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _ilService.DeleteAsync(DeleteIlId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("İl başarıyla silindi!");
                    CloseDeleteModal();
                    await LoadIller();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İl silinemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsDeleting = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // EXPORT METHODS
        // ═══════════════════════════════════════════════════════

        private async Task ExportToExcel()
        {
            IsExporting = true;
            ExportType = "excel";

            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("İller");

                // Header
                worksheet.Cell(1, 1).Value = "İl Adı";
                worksheet.Cell(1, 2).Value = "İlçe Sayısı";
                worksheet.Cell(1, 3).Value = "Eklenme Tarihi";
                worksheet.Cell(1, 4).Value = "Güncellenme Tarihi";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 4);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var il in FilteredIller)
                {
                    worksheet.Cell(row, 1).Value = il.IlAdi;
                    worksheet.Cell(row, 2).Value = il.IlceSayisi;
                    worksheet.Cell(row, 3).Value = il.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(row, 4).Value = il.DuzenlenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"Iller_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                await _toastService.ShowSuccessAsync("Excel dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
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
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Text("İl Listesi").FontSize(20).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("İl Adı").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("İlçe Sayısı").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eklenme").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Güncelleme").Bold();
                            });

                            foreach (var il in FilteredIller)
                            {
                                table.Cell().Padding(5).Text(il.IlAdi);
                                table.Cell().Padding(5).Text(il.IlceSayisi.ToString());
                                table.Cell().Padding(5).Text(il.EklenmeTarihi.ToString("dd.MM.yyyy"));
                                table.Cell().Padding(5).Text(il.DuzenlenmeTarihi.ToString("dd.MM.yyyy"));
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
                var fileName = $"Iller_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(pdfBytes), "application/pdf");
                await _toastService.ShowSuccessAsync("PDF dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
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
