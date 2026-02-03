using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Shared.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class InstantAttendanceRecords : FieldPermissionPageBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IZKTecoDeviceApiService DeviceApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<AttendanceLogDto> records = new List<AttendanceLogDto>();
        private List<AttendanceLogDto> filteredRecords = new List<AttendanceLogDto>();
        private List<AttendanceLogDto> pagedRecords = new List<AttendanceLogDto>();
        private List<DeviceResponseDto> devices = new List<DeviceResponseDto>();
        private int selectedDeviceId = 0;
        private bool isLoading = false;
        private int totalCount = 0;

        // Export state
        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = string.Empty;

        // Pagination
        private int currentPage = 1;
        private int pageSize = 50;
        private int totalPages = 0;

        // Filters
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private string enrollNumberFilter = "";
        private string sicilNoFilter = "";
        private string tcKimlikNoFilter = "";
        private VerifyMethod? verifyMethodFilter = null;
        private InOutMode? inOutModeFilter = null;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await base.OnInitializedAsync();
            await LoadDevices();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING
        // ═══════════════════════════════════════════════════════

        private async Task LoadDevices()
        {
            try
            {
                var result = await DeviceApiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    devices = result.Data;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihazlar yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task LoadRecords()
        {
            if (selectedDeviceId == 0)
            {
                await ToastService.ShowWarningAsync("Lütfen bir cihaz seçin");
                return;
            }

            isLoading = true;
            try
            {
                var result = await DeviceApiService.GetAttendanceLogsAsync(selectedDeviceId);
                if (result.Success && result.Data != null)
                {
                    records = result.Data;
                    ApplyFilters();
                    
                    var countResult = await DeviceApiService.GetAttendanceLogCountAsync(selectedDeviceId);
                    if (countResult.Success)
                    {
                        totalCount = countResult.Data;
                    }
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kayıtlar yüklenemedi");
                    records = new List<AttendanceLogDto>();
                    filteredRecords = new List<AttendanceLogDto>();
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
                records = new List<AttendanceLogDto>();
                filteredRecords = new List<AttendanceLogDto>();
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OnDeviceChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int deviceId))
            {
                selectedDeviceId = deviceId;
                await LoadRecords();
            }
            else
            {
                selectedDeviceId = 0;
                records.Clear();
                filteredRecords.Clear();
            }
        }

        // ═══════════════════════════════════════════════════════
        // FILTERING
        // ═══════════════════════════════════════════════════════

        private void ApplyFilters()
        {
            filteredRecords = records;

            if (startDate.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.DateTime >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.DateTime <= endDate.Value.AddDays(1).AddSeconds(-1)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(enrollNumberFilter))
            {
                filteredRecords = filteredRecords.Where(r => r.EnrollNumber.Contains(enrollNumberFilter)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(sicilNoFilter))
            {
                filteredRecords = filteredRecords.Where(r => 
                    !string.IsNullOrEmpty(r.PersonelSicilNo) && 
                    r.PersonelSicilNo.Contains(sicilNoFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(tcKimlikNoFilter))
            {
                filteredRecords = filteredRecords.Where(r => 
                    !string.IsNullOrEmpty(r.PersonelTcKimlikNo) && 
                    r.PersonelTcKimlikNo.Contains(tcKimlikNoFilter)).ToList();
            }

            if (verifyMethodFilter.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.VerifyMethod == verifyMethodFilter.Value).ToList();
            }

            if (inOutModeFilter.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.InOutMode == inOutModeFilter.Value).ToList();
            }

            // Pagination hesapla
            totalPages = (int)Math.Ceiling(filteredRecords.Count / (double)pageSize);
            if (currentPage > totalPages && totalPages > 0)
            {
                currentPage = totalPages;
            }
            if (currentPage < 1)
            {
                currentPage = 1;
            }

            pagedRecords = filteredRecords
                .OrderByDescending(r => r.DateTime)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private void ClearFilters()
        {
            startDate = null;
            endDate = null;
            enrollNumberFilter = "";
            sicilNoFilter = "";
            tcKimlikNoFilter = "";
            verifyMethodFilter = null;
            inOutModeFilter = null;
            currentPage = 1;
            ApplyFilters();
        }

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= totalPages)
            {
                currentPage = page;
                ApplyFilters();
            }
        }

        private void NextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                ApplyFilters();
            }
        }

        private void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                ApplyFilters();
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
                var worksheet = workbook.Worksheets.Add("Attendance Kayıtları");

                // Header
                worksheet.Cell(1, 1).Value = "Tarih/Saat";
                worksheet.Cell(1, 2).Value = "Enroll Number";
                worksheet.Cell(1, 3).Value = "Personel Ad Soyad";
                worksheet.Cell(1, 4).Value = "Sicil No";
                worksheet.Cell(1, 5).Value = "TC Kimlik No";
                worksheet.Cell(1, 6).Value = "Doğrulama Yöntemi";
                worksheet.Cell(1, 7).Value = "Giriş/Çıkış";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var record in filteredRecords.OrderByDescending(r => r.DateTime))
                {
                    worksheet.Cell(row, 1).Value = record.DateTime.ToString("dd.MM.yyyy HH:mm:ss");
                    worksheet.Cell(row, 2).Value = record.EnrollNumber;
                    worksheet.Cell(row, 3).Value = record.PersonelAdSoyad ?? "-";
                    worksheet.Cell(row, 4).Value = record.PersonelSicilNo ?? "-";
                    worksheet.Cell(row, 5).Value = record.PersonelTcKimlikNo ?? "-";
                    worksheet.Cell(row, 6).Value = GetVerifyMethodText(record.VerifyMethod);
                    worksheet.Cell(row, 7).Value = GetInOutModeText(record.InOutMode);
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"AttendanceKayitlari_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                await JS.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                await ToastService.ShowSuccessAsync("Excel dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Excel oluşturulurken hata: {ex.Message}");
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
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(9));

                        page.Header().Text("Anlık Attendance Kayıtları").FontSize(18).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tarih/Saat").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Enroll No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Personel").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Sicil No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("TC No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Doğrulama").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Giriş/Çıkış").Bold();
                            });

                            foreach (var record in filteredRecords.OrderByDescending(r => r.DateTime))
                            {
                                table.Cell().Padding(3).Text(record.DateTime.ToString("dd.MM.yyyy HH:mm"));
                                table.Cell().Padding(3).Text(record.EnrollNumber);
                                table.Cell().Padding(3).Text(record.PersonelAdSoyad ?? "-");
                                table.Cell().Padding(3).Text(record.PersonelSicilNo ?? "-");
                                table.Cell().Padding(3).Text(record.PersonelTcKimlikNo ?? "-");
                                table.Cell().Padding(3).Text(GetVerifyMethodText(record.VerifyMethod));
                                table.Cell().Padding(3).Text(GetInOutModeText(record.InOutMode));
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
                var fileName = $"AttendanceKayitlari_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                await JS.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(pdfBytes), "application/pdf");
                await ToastService.ShowSuccessAsync("PDF dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"PDF oluşturulurken hata: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }

        private string GetVerifyMethodText(VerifyMethod method)
        {
            return method switch
            {
                VerifyMethod.Card => "Kart",
                _ => "Bilinmiyor"
            };
        }

        private string GetInOutModeText(InOutMode mode)
        {
            return mode switch
            {
                InOutMode.CheckIn => "Giriş",
                InOutMode.CheckOut => "Çıkış",
                _ => "Bilinmiyor"
            };
        }
    }
}
