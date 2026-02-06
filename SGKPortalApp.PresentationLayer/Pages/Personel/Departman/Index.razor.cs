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

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Departman
{
    public partial class Index
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IDepartmanApiService _departmanService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<DepartmanResponseDto> Departmanlar { get; set; } = new();
        private List<DepartmanResponseDto> FilteredDepartmanlar { get; set; } = new();

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
        private int TotalPages => (int)Math.Ceiling(FilteredDepartmanlar.Count / (double)PageSize);

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
        private int ToggleDepartmanId { get; set; }
        private string ToggleDepartmanAdi { get; set; } = string.Empty;
        private Aktiflik ToggleDepartmanCurrentStatus { get; set; }
        private int ToggleDepartmanPersonelSayisi { get; set; }
        private bool IsToggling { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowDeleteModal { get; set; } = false;
        private int DeleteDepartmanId { get; set; }
        private string DeleteDepartmanAdi { get; set; } = string.Empty;
        private int DeleteDepartmanPersonelSayisi { get; set; }
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadDepartmanlar();
        }

        private async Task LoadDepartmanlar()
        {
            IsLoading = true;
            try
            {
                var result = await _departmanService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    Departmanlar = result.Data;
                    ApplyFiltersAndSort();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Departmanlar yüklenemedi!");
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
            var filtered = Departmanlar.AsEnumerable();

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.Where(d =>
                    d.DepartmanAdi.ContainsTurkish(searchTerm));
            }

            // Durum filtresi
            if (filterStatus != "all")
            {
                var status = filterStatus == "active" ? Aktiflik.Aktif : Aktiflik.Pasif;
                filtered = filtered.Where(d => d.Aktiflik == status);
            }

            // Sıralama
            filtered = sortBy switch
            {
                "name-asc" => filtered.OrderBy(d => d.DepartmanAdi),
                "name-desc" => filtered.OrderByDescending(d => d.DepartmanAdi),
                "date-newest" => filtered.OrderByDescending(d => d.EklenmeTarihi),
                "date-oldest" => filtered.OrderBy(d => d.EklenmeTarihi),
                "personel-most" => filtered.OrderByDescending(d => d.PersonelSayisi),
                "personel-least" => filtered.OrderBy(d => d.PersonelSayisi),
                _ => filtered.OrderBy(d => d.DepartmanAdi)
            };

            FilteredDepartmanlar = filtered.ToList();
            CurrentPage = 1;
        }

        private void OnSearchChanged()
        {
            ApplyFiltersAndSort();
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
            _navigationManager.NavigateTo("/personel/departman/add");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/personel/departman/manage/{id}");
        }

        // ═══════════════════════════════════════════════════════
        // TOGGLE STATUS METHODS
        // ═══════════════════════════════════════════════════════

        private async Task ShowToggleStatusConfirmation(int id, string departmanAdi, Aktiflik currentStatus)
        {
            ToggleDepartmanId = id;
            ToggleDepartmanAdi = departmanAdi;
            ToggleDepartmanCurrentStatus = currentStatus;

            // Personel sayısını al
            var personelCountResult = await _departmanService.GetPersonelCountAsync(id);
            ToggleDepartmanPersonelSayisi = personelCountResult.Success ? personelCountResult.Data : 0;

            ShowToggleModal = true;
        }

        private void CloseToggleModal()
        {
            ShowToggleModal = false;
            ToggleDepartmanId = 0;
            ToggleDepartmanAdi = string.Empty;
        }

        private async Task ConfirmToggleStatus()
        {
            IsToggling = true;
            try
            {
                var departman = Departmanlar.FirstOrDefault(d => d.DepartmanId == ToggleDepartmanId);
                if (departman == null) return;

                var newStatus = departman.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif;
                var updateDto = new BusinessObjectLayer.DTOs.Request.PersonelIslemleri.DepartmanUpdateRequestDto
                {
                    DepartmanAdi = departman.DepartmanAdi,
                    Aktiflik = newStatus
                };

                var result = await _departmanService.UpdateAsync(ToggleDepartmanId, updateDto);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"Departman {(newStatus == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı!");
                    CloseToggleModal();
                    await LoadDepartmanlar();
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

        private void ShowDeleteConfirmation(int id, string departmanAdi, int personelSayisi)
        {
            DeleteDepartmanId = id;
            DeleteDepartmanAdi = departmanAdi;
            DeleteDepartmanPersonelSayisi = personelSayisi;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeleteDepartmanId = 0;
            DeleteDepartmanAdi = string.Empty;
            DeleteDepartmanPersonelSayisi = 0;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _departmanService.DeleteAsync(DeleteDepartmanId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Departman başarıyla silindi!");
                    CloseDeleteModal();
                    await LoadDepartmanlar();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Departman silinemedi!");
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
                var worksheet = workbook.Worksheets.Add("Departmanlar");

                // Header
                worksheet.Cell(1, 1).Value = "Departman Adı";
                worksheet.Cell(1, 2).Value = "Personel Sayısı";
                worksheet.Cell(1, 3).Value = "Durum";
                worksheet.Cell(1, 4).Value = "Eklenme Tarihi";
                worksheet.Cell(1, 5).Value = "Güncellenme Tarihi";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var departman in FilteredDepartmanlar)
                {
                    worksheet.Cell(row, 1).Value = departman.DepartmanAdi;
                    worksheet.Cell(row, 2).Value = departman.PersonelSayisi;
                    worksheet.Cell(row, 3).Value = departman.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 4).Value = departman.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(row, 5).Value = departman.DuzenlenmeTarihi.ToString("dd.MM.yyyy HH:mm");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"Departmanlar_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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

                        page.Header().Text("Departman Listesi").FontSize(20).Bold();

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
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Departman").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Personel").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eklenme").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Güncelleme").Bold();
                            });

                            foreach (var departman in FilteredDepartmanlar)
                            {
                                table.Cell().Padding(5).Text(departman.DepartmanAdi);
                                table.Cell().Padding(5).Text(departman.PersonelSayisi.ToString());
                                table.Cell().Padding(5).Text(departman.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif");
                                table.Cell().Padding(5).Text(departman.EklenmeTarihi.ToString("dd.MM.yyyy"));
                                table.Cell().Padding(5).Text(departman.DuzenlenmeTarihi.ToString("dd.MM.yyyy"));
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
                var fileName = $"Departmanlar_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

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
