using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices;

namespace SGKPortalApp.PresentationLayer.Pages.Common.Ilce
{
    public partial class Index : ComponentBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IIlceApiService _ilceService { get; set; } = default!;
        [Inject] private IIlApiService _ilService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<IlceResponseDto> Ilceler { get; set; } = new();
        private List<IlceResponseDto> FilteredIlceler { get; set; } = new();
        private List<IlResponseDto> Iller { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string searchTerm = string.Empty;
        private int filterIlId = 0;
        private string sortBy = "ilce-asc";

        // ═══════════════════════════════════════════════════════
        // PAGINATION PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => (int)Math.Ceiling(FilteredIlceler.Count / (double)PageSize);

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
        private int DeleteIlceId { get; set; }
        private string DeleteIlceAdi { get; set; } = string.Empty;
        private string DeleteIlceIlAdi { get; set; } = string.Empty;
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadIller();
            await LoadIlceler();
        }

        private async Task LoadIller()
        {
            try
            {
                var result = await _ilService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    Iller = result.Data;
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"İller yüklenemedi: {ex.Message}");
            }
        }

        private async Task LoadIlceler()
        {
            IsLoading = true;
            try
            {
                var result = await _ilceService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    Ilceler = result.Data;
                    ApplyFiltersAndSort();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İlçeler yüklenemedi!");
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
            var filtered = Ilceler.AsEnumerable();

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.Where(i =>
                    i.IlceAdi.ContainsTurkish(searchTerm));
            }

            // İl filtresi
            if (filterIlId > 0)
            {
                filtered = filtered.Where(i => i.IlId == filterIlId);
            }

            // Sıralama
            filtered = sortBy switch
            {
                "ilce-asc" => filtered.OrderBy(i => i.IlceAdi),
                "ilce-desc" => filtered.OrderByDescending(i => i.IlceAdi),
                "il-asc" => filtered.OrderBy(i => i.IlAdi).ThenBy(i => i.IlceAdi),
                "il-desc" => filtered.OrderByDescending(i => i.IlAdi).ThenBy(i => i.IlceAdi),
                "date-newest" => filtered.OrderByDescending(i => i.EklenmeTarihi),
                "date-oldest" => filtered.OrderBy(i => i.EklenmeTarihi),
                _ => filtered.OrderBy(i => i.IlAdi).ThenBy(i => i.IlceAdi)
            };

            FilteredIlceler = filtered.ToList();
            CurrentPage = 1;
        }

        private void OnSearchChanged()
        {
            ApplyFiltersAndSort();
        }

        private void OnFilterIlChanged(ChangeEventArgs e)
        {
            filterIlId = int.Parse(e.Value?.ToString() ?? "0");
            ApplyFiltersAndSort();
        }

        private void OnSortByChanged(ChangeEventArgs e)
        {
            sortBy = e.Value?.ToString() ?? "ilce-asc";
            ApplyFiltersAndSort();
        }

        private void ClearFilters()
        {
            searchTerm = string.Empty;
            filterIlId = 0;
            sortBy = "ilce-asc";
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
            _navigationManager.NavigateTo("/common/ilce/manage");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/common/ilce/manage/{id}");
        }

        // ═══════════════════════════════════════════════════════
        // DELETE METHODS
        // ═══════════════════════════════════════════════════════

        private void ShowDeleteConfirmation(int id, string ilceAdi, string ilAdi)
        {
            DeleteIlceId = id;
            DeleteIlceAdi = ilceAdi;
            DeleteIlceIlAdi = ilAdi;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeleteIlceId = 0;
            DeleteIlceAdi = string.Empty;
            DeleteIlceIlAdi = string.Empty;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _ilceService.DeleteAsync(DeleteIlceId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("İlçe başarıyla silindi!");
                    CloseDeleteModal();
                    await LoadIlceler();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İlçe silinemedi!");
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
                var worksheet = workbook.Worksheets.Add("İlçeler");

                // Header
                worksheet.Cell(1, 1).Value = "İl";
                worksheet.Cell(1, 2).Value = "İlçe Adı";
                worksheet.Cell(1, 3).Value = "Eklenme Tarihi";
                worksheet.Cell(1, 4).Value = "Güncellenme Tarihi";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 4);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var ilce in FilteredIlceler)
                {
                    worksheet.Cell(row, 1).Value = ilce.IlAdi;
                    worksheet.Cell(row, 2).Value = ilce.IlceAdi;
                    worksheet.Cell(row, 3).Value = ilce.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(row, 4).Value = ilce.DuzenlenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"Ilceler_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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

                        page.Header().Text("İlçe Listesi").FontSize(20).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("İl").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("İlçe").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eklenme").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Güncelleme").Bold();
                            });

                            foreach (var ilce in FilteredIlceler)
                            {
                                table.Cell().Padding(5).Text(ilce.IlAdi);
                                table.Cell().Padding(5).Text(ilce.IlceAdi);
                                table.Cell().Padding(5).Text(ilce.EklenmeTarihi.ToString("dd.MM.yyyy"));
                                table.Cell().Padding(5).Text(ilce.DuzenlenmeTarihi.ToString("dd.MM.yyyy"));
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
                var fileName = $"Ilceler_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

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
