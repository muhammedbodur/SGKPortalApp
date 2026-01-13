using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.CardSyncComparison
{
    public partial class Index
    {
        [Inject] private ICardSyncComparisonApiService CardSyncComparisonApiService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        private DeviceCardSyncReportDto? report;
        private bool isLoading;
        private bool isExporting;
        private int pageSize = 50;

        // Filtreler
        private string filterMismatchType = string.Empty;
        private int filterDeviceId;
        private string filterUserType = string.Empty;
        private string searchTerm = string.Empty;

        private double ProgressPercentage => report?.TotalDevicesChecked > 0
            ? (report.DeviceStatuses.Count * 100.0 / report.TotalDevicesChecked)
            : 0;

        private List<CardMismatchDto> FilteredMismatches
        {
            get
            {
                if (report == null || !report.Mismatches.Any())
                    return new List<CardMismatchDto>();

                var filtered = report.Mismatches.AsEnumerable();

                // Uyumsuzluk tipi filtresi
                if (!string.IsNullOrWhiteSpace(filterMismatchType))
                {
                    filtered = filtered.Where(m => m.MismatchType == filterMismatchType);
                }

                // Cihaz filtresi
                if (filterDeviceId > 0)
                {
                    filtered = filtered.Where(m => m.DeviceId == filterDeviceId);
                }

                // Kullanıcı tipi filtresi
                if (!string.IsNullOrWhiteSpace(filterUserType))
                {
                    filtered = filtered.Where(m => m.UserType == filterUserType);
                }

                // Arama filtresi
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var searchLower = searchTerm.ToLower();
                    filtered = filtered.Where(m =>
                        m.EnrollNumber.ToLower().Contains(searchLower) ||
                        m.Name.ToLower().Contains(searchLower) ||
                        (m.DeviceUserName?.ToLower().Contains(searchLower) ?? false) ||
                        (m.DatabaseUserName?.ToLower().Contains(searchLower) ?? false));
                }

                return filtered.ToList();
            }
        }

        private async Task GenerateReport()
        {
            isLoading = true;
            pageSize = 50;
            StateHasChanged();

            try
            {
                report = await CardSyncComparisonApiService.GenerateCardSyncReportAsync();

                if (report != null && report.TotalMismatches > 0)
                {
                    await ToastService.ShowInfoAsync($"{report.TotalMismatches} uyumsuzluk tespit edildi!");
                }
                else if (report != null)
                {
                    await ToastService.ShowSuccessAsync("Tüm kartlar senkronize!");
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rapor oluşturulurken hata: {ex.Message}");
                await ToastService.ShowErrorAsync("Rapor oluşturulurken hata oluştu: " + ex.Message);
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void LoadMore()
        {
            pageSize += 50;
        }

        private async Task ExportToExcel()
        {
            if (report == null || !report.Mismatches.Any())
                return;

            isExporting = true;
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Kart Senkronizasyon Raporu");

                // Headers
                worksheet.Cell(1, 1).Value = "EnrollNumber";
                worksheet.Cell(1, 2).Value = "İsim";
                worksheet.Cell(1, 3).Value = "Kart No";
                worksheet.Cell(1, 4).Value = "Uyumsuzluk Tipi";
                worksheet.Cell(1, 5).Value = "Cihaz";
                worksheet.Cell(1, 6).Value = "Cihaz IP";
                worksheet.Cell(1, 7).Value = "Kullanıcı Tipi";
                worksheet.Cell(1, 8).Value = "Detay";

                // Header formatting
                var headerRange = worksheet.Range(1, 1, 1, 8);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Data
                for (int i = 0; i < FilteredMismatches.Count; i++)
                {
                    var mismatch = FilteredMismatches[i];
                    var row = i + 2;

                    worksheet.Cell(row, 1).Value = mismatch.EnrollNumber;

                    // İsim
                    if (mismatch.MismatchType == "DataMismatch")
                    {
                        worksheet.Cell(row, 2).Value = $"Cihaz: {mismatch.DeviceUserName} / DB: {mismatch.DatabaseUserName}";
                    }
                    else
                    {
                        worksheet.Cell(row, 2).Value = mismatch.Name;
                    }

                    // Kart No
                    if (mismatch.MismatchType == "DataMismatch")
                    {
                        worksheet.Cell(row, 3).Value = $"Cihaz: {mismatch.DeviceCardNumber} / DB: {mismatch.DatabaseCardNumber}";
                    }
                    else
                    {
                        worksheet.Cell(row, 3).Value = mismatch.CardNumber?.ToString() ?? "";
                    }

                    // Uyumsuzluk Tipi
                    var mismatchTypeText = mismatch.MismatchType switch
                    {
                        "OnlyInDevice" => "Sadece Cihazda",
                        "OnlyInDatabase" => "Sadece Veritabanında",
                        "DataMismatch" => "Bilgi Uyumsuzluğu",
                        _ => mismatch.MismatchType
                    };
                    worksheet.Cell(row, 4).Value = mismatchTypeText;

                    worksheet.Cell(row, 5).Value = mismatch.DeviceName;
                    worksheet.Cell(row, 6).Value = mismatch.DeviceIp;
                    worksheet.Cell(row, 7).Value = mismatch.UserType ?? "";
                    worksheet.Cell(row, 8).Value = mismatch.Details;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save to stream
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"Kart_Senkronizasyon_Raporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                await ToastService.ShowSuccessAsync("Excel dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export hatası: {ex.Message}");
                await ToastService.ShowErrorAsync("Export işlemi başarısız: " + ex.Message);
            }
            finally
            {
                isExporting = false;
            }
        }
    }
}
