using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Sendika
{
    public partial class Index
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private ISendikaApiService _sendikaService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<SendikaResponseDto> Sendikalar { get; set; } = new();
        private List<SendikaResponseDto> FilteredSendikalar { get; set; } = new();

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
        private int TotalPages => (int)Math.Ceiling(FilteredSendikalar.Count / (double)PageSize);

        // ═══════════════════════════════════════════════════════
        // STATISTICS
        // ═══════════════════════════════════════════════════════

        private int TotalCount => Sendikalar.Count;
        private int ActiveCount => Sendikalar.Count(s => s.Aktiflik == Aktiflik.Aktif);
        private int PassiveCount => Sendikalar.Count(s => s.Aktiflik == Aktiflik.Pasif);

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
        private int ToggleSendikaId { get; set; }
        private string ToggleSendikaAdi { get; set; } = string.Empty;
        private Aktiflik ToggleSendikaCurrentStatus { get; set; }
        private int ToggleSendikaPersonelSayisi { get; set; }
        private bool IsToggling { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private SendikaResponseDto? SendikaToDelete { get; set; }
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadSendikalar();
        }

        private async Task LoadSendikalar()
        {
            IsLoading = true;
            try
            {
                var result = await _sendikaService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    Sendikalar = result.Data;
                    ApplyFiltersAndSort();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Sendikalar yüklenemedi!");
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
            var filtered = Sendikalar.AsEnumerable();

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.Where(s =>
                    s.SendikaAdi.ContainsTurkish(searchTerm));
            }

            // Durum filtresi
            if (filterStatus != "all")
            {
                var status = filterStatus == "active" ? Aktiflik.Aktif : Aktiflik.Pasif;
                filtered = filtered.Where(s => s.Aktiflik == status);
            }

            // Sıralama
            filtered = sortBy switch
            {
                "name-asc" => filtered.OrderBy(s => s.SendikaAdi),
                "name-desc" => filtered.OrderByDescending(s => s.SendikaAdi),
                "date-newest" => filtered.OrderByDescending(s => s.EklenmeTarihi),
                "date-oldest" => filtered.OrderBy(s => s.EklenmeTarihi),
                "personel-most" => filtered.OrderByDescending(s => s.PersonelSayisi),
                "personel-least" => filtered.OrderBy(s => s.PersonelSayisi),
                _ => filtered.OrderBy(s => s.SendikaAdi)
            };

            FilteredSendikalar = filtered.ToList();
            CurrentPage = 1;
        }

        private void OnSearchChanged()
        {
            ApplyFiltersAndSort();
            StateHasChanged();
        }

        private void OnFilterStatusChanged(ChangeEventArgs e)
        {
            filterStatus = e.Value?.ToString() ?? "all";
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
            ApplyFiltersAndSort();
        }

        // ═══════════════════════════════════════════════════════
        // NAVIGATION METHODS
        // ═══════════════════════════════════════════════════════

        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/personel/sendika/add");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/personel/sendika/manage/{id}");
        }

        // ═══════════════════════════════════════════════════════
        // TOGGLE STATUS MODAL
        // ═══════════════════════════════════════════════════════

        private async Task ShowToggleStatusConfirmation(int sendikaId, string sendikaAdi, Aktiflik currentStatus)
        {
            ToggleSendikaId = sendikaId;
            ToggleSendikaAdi = sendikaAdi;
            ToggleSendikaCurrentStatus = currentStatus;

            // Personel sayısını al
            var personelCountResult = await _sendikaService.GetPersonelCountAsync(sendikaId);
            ToggleSendikaPersonelSayisi = personelCountResult.Success ? personelCountResult.Data : 0;

            ShowToggleModal = true;
        }

        private void CloseToggleModal()
        {
            ShowToggleModal = false;
            ToggleSendikaId = 0;
            ToggleSendikaAdi = string.Empty;
        }

        private async Task ConfirmToggleStatus()
        {
            IsToggling = true;
            try
            {
                var sendika = Sendikalar.FirstOrDefault(s => s.SendikaId == ToggleSendikaId);
                if (sendika == null) return;

                var newStatus = sendika.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif;
                var updateDto = new BusinessObjectLayer.DTOs.Request.PersonelIslemleri.SendikaUpdateRequestDto
                {
                    SendikaAdi = sendika.SendikaAdi,
                    Aktiflik = newStatus
                };

                var result = await _sendikaService.UpdateAsync(ToggleSendikaId, updateDto);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"Sendika {(newStatus == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı!");
                    CloseToggleModal();
                    await LoadSendikalar();
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
        // DELETE METHODS
        // ═══════════════════════════════════════════════════════

        private void ConfirmDelete(SendikaResponseDto sendika)
        {
            SendikaToDelete = sendika;
        }

        private void CancelDelete()
        {
            SendikaToDelete = null;
        }

        private async Task DeleteSendika()
        {
            if (SendikaToDelete == null) return;

            IsDeleting = true;
            try
            {
                var result = await _sendikaService.DeleteAsync(SendikaToDelete.SendikaId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Sendika başarıyla silindi!");
                    await LoadSendikalar();
                    SendikaToDelete = null;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Sendika silinemedi!");
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
                var worksheet = workbook.Worksheets.Add("Sendikalar");

                // Header
                worksheet.Cell(1, 1).Value = "Sendika Adı";
                worksheet.Cell(1, 2).Value = "Personel Sayısı";
                worksheet.Cell(1, 3).Value = "Durum";
                worksheet.Cell(1, 4).Value = "Eklenme Tarihi";
                worksheet.Cell(1, 5).Value = "Güncelleme Tarihi";

                // Header style
                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var sendika in FilteredSendikalar)
                {
                    worksheet.Cell(row, 1).Value = sendika.SendikaAdi;
                    worksheet.Cell(row, 2).Value = sendika.PersonelSayisi;
                    worksheet.Cell(row, 3).Value = sendika.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 4).Value = sendika.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(row, 5).Value = sendika.DuzenlenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                await JSRuntime.InvokeVoidAsync("downloadFile", "Sendikalar.xlsx", Convert.ToBase64String(content));
                await _toastService.ShowSuccessAsync("Excel dosyası indirildi!");
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
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header()
                            .Text("Sendika Listesi")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Sendika Adı").SemiBold();
                                    header.Cell().Element(CellStyle).Text("Personel").SemiBold();
                                    header.Cell().Element(CellStyle).Text("Durum").SemiBold();
                                    header.Cell().Element(CellStyle).Text("Eklenme").SemiBold();
                                    header.Cell().Element(CellStyle).Text("Güncelleme").SemiBold();

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold())
                                            .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                    }
                                });

                                foreach (var sendika in FilteredSendikalar)
                                {
                                    table.Cell().Element(CellStyle).Text(sendika.SendikaAdi);
                                    table.Cell().Element(CellStyle).Text(sendika.PersonelSayisi.ToString());
                                    table.Cell().Element(CellStyle).Text(sendika.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif");
                                    table.Cell().Element(CellStyle).Text(sendika.EklenmeTarihi.ToString("dd.MM.yyyy"));
                                    table.Cell().Element(CellStyle).Text(sendika.DuzenlenmeTarihi.ToString("dd.MM.yyyy"));

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .PaddingVertical(5);
                                    }
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Sayfa ");
                                x.CurrentPageNumber();
                            });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                await JSRuntime.InvokeVoidAsync("downloadFile", "Sendikalar.pdf", Convert.ToBase64String(pdfBytes));
                await _toastService.ShowSuccessAsync("PDF dosyası indirildi!");
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
