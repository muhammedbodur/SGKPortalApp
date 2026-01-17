using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Mesai
{
    public partial class DepartmanMesaiList
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IPersonelListApiService _personelListService { get; set; } = default!;
        [Inject] private IDepartmanMesaiApiService _departmanMesaiService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime _jsRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private DepartmanMesaiReportDto? Report { get; set; }
        private List<DepartmanDto> Departmanlar { get; set; } = new();
        private List<ServisDto> Servisler { get; set; } = new();
        private HashSet<string> ExpandedRows { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int? filterDepartmanId = null;
        private int? filterServisId = null;
        private int? userDepartmanId = null;
        private int? userServisId = null;
        private DateTime baslangicTarihi = DateTime.Now.AddDays(-30);
        private DateTime bitisTarihi = DateTime.Now;

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = false;
        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFilterData();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING METHODS
        // ═══════════════════════════════════════════════════════

        private async Task LoadFilterData()
        {
            try
            {
                // Get user's departman and servis from claims
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var departmanIdClaim = authState.User.FindFirst("DepartmanId")?.Value;
                var servisIdClaim = authState.User.FindFirst("ServisId")?.Value;

                if (int.TryParse(departmanIdClaim, out var deptId))
                {
                    userDepartmanId = deptId;
                    filterDepartmanId = deptId; // Set user's departman as default filter
                }

                if (int.TryParse(servisIdClaim, out var servId))
                {
                    userServisId = servId;
                    filterServisId = servId; // Set user's servis as default filter
                }

                var departmanResult = await _personelListService.GetDepartmanListeAsync();
                if (departmanResult.Success && departmanResult.Data != null)
                {
                    Departmanlar = departmanResult.Data;
                }

                var servisResult = await _personelListService.GetServisListeAsync();
                if (servisResult.Success && servisResult.Data != null)
                {
                    Servisler = servisResult.Data;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Filtre verileri yüklenirken hata");
            }
        }

        private async Task LoadReport()
        {
            try
            {
                if (!filterDepartmanId.HasValue)
                {
                    Logger.LogWarning("Departman seçilmeden rapor oluşturulamaz");
                    return;
                }

                IsLoading = true;
                Report = null;
                ExpandedRows.Clear();

                var request = new DepartmanMesaiFilterRequestDto
                {
                    DepartmanId = filterDepartmanId.Value,
                    ServisId = filterServisId,
                    BaslangicTarihi = baslangicTarihi,
                    BitisTarihi = bitisTarihi
                };

                var result = await _departmanMesaiService.GetRaporAsync(request);

                if (result.Success && result.Data != null)
                {
                    Report = result.Data;
                    // Personel listesini tarihe göre sırala (günlük detaylar için)
                    if (Report.Personeller != null && Report.Personeller.Any())
                    {
                        foreach (var personel in Report.Personeller)
                        {
                            if (personel.GunlukDetay != null && personel.GunlukDetay.Any())
                            {
                                personel.GunlukDetay = personel.GunlukDetay.OrderByDescending(g => g.Tarih).ToList();
                            }
                        }
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Rapor yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Departman mesai raporu yüklenirken hata");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ToggleRow(string tcKimlikNo)
        {
            if (ExpandedRows.Contains(tcKimlikNo))
            {
                ExpandedRows.Remove(tcKimlikNo);
            }
            else
            {
                ExpandedRows.Add(tcKimlikNo);
            }
        }

        // ═══════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════════════════════════════

        private async Task OnDepartmanChangedEvent(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int deptId))
            {
                // ✅ ERİŞİM KONTROLÜ: Kullanıcı bu departmanı görüntüleyebilir mi?
                if (deptId > 0 && !CanAccessDepartman(deptId))
                {
                    await _toastService.ShowWarningAsync("Bu departmanı görüntüleme yetkiniz yok!");
                    // Kullanıcının kendi departmanına geri dön
                    filterDepartmanId = userDepartmanId;
                    return;
                }

                filterDepartmanId = deptId > 0 ? deptId : null;
                // Departman değiştiğinde servis listesini temizle
                filterServisId = null;
            }
        }

        private async Task OnServisChangedEvent(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int servId))
            {
                // ✅ ERİŞİM KONTROLÜ: Kullanıcı bu servisi görüntüleyebilir mi?
                if (servId > 0 && !CanAccessServis(servId))
                {
                    await _toastService.ShowWarningAsync("Bu servisi görüntüleme yetkiniz yok!");
                    // Kullanıcının kendi servisine geri dön
                    filterServisId = userServisId;
                    return;
                }

                filterServisId = servId > 0 ? servId : null;
            }
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private bool CanAccessDepartman(int departmanId)
        {
            var userDeptId = GetCurrentUserDepartmanId();

            // Kullanıcının departmanı yoksa (admin vs) tüm departmanlara erişebilir
            if (userDeptId == 0)
                return true;

            // Kullanıcı sadece kendi departmanını görebilir
            return userDeptId == departmanId;
        }

        private bool CanAccessServis(int servisId)
        {
            var userServId = GetCurrentUserServisId();

            // Kullanıcının servisi yoksa departman bazlı kontrol yap
            if (userServId == 0)
            {
                // Kullanıcının departmanındaki tüm servislere erişebilir
                return true;
            }

            // Kullanıcı sadece kendi servisini görebilir
            return userServId == servisId;
        }

        private int GetCurrentUserDepartmanId()
        {
            return userDepartmanId ?? 0;
        }

        private int GetCurrentUserServisId()
        {
            return userServisId ?? 0;
        }

        // ═══════════════════════════════════════════════════════
        // EXPORT METHODS
        // ═══════════════════════════════════════════════════════

        private async Task ExportToExcel()
        {
            if (Report == null || Report.Personeller == null || !Report.Personeller.Any())
            {
                await _toastService.ShowWarningAsync("Dışa aktarılacak veri bulunamadı!");
                return;
            }

            IsExporting = true;
            ExportType = "excel";

            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Departman Mesai Raporu");

                // Header
                worksheet.Cell(1, 1).Value = "Sicil No";
                worksheet.Cell(1, 2).Value = "Ad Soyad";
                worksheet.Cell(1, 3).Value = "Departman";
                worksheet.Cell(1, 4).Value = "Servis";
                worksheet.Cell(1, 5).Value = "Çalıştığı Gün";
                worksheet.Cell(1, 6).Value = "Toplam Mesai";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var personel in Report.Personeller)
                {
                    worksheet.Cell(row, 1).Value = personel.SicilNo;
                    worksheet.Cell(row, 2).Value = personel.AdSoyad;
                    worksheet.Cell(row, 3).Value = personel.DepartmanAdi;
                    worksheet.Cell(row, 4).Value = personel.ServisAdi ?? "-";
                    worksheet.Cell(row, 5).Value = personel.CalistigiGun;
                    worksheet.Cell(row, 6).Value = personel.ToplamMesaiSuresi;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"DepartmanMesaiRaporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
            if (Report == null || Report.Personeller == null || !Report.Personeller.Any())
            {
                await _toastService.ShowWarningAsync("Dışa aktarılacak veri bulunamadı!");
                return;
            }

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

                        page.Header().Text($"Departman Mesai Raporu ({baslangicTarihi:dd.MM.yyyy} - {bitisTarihi:dd.MM.yyyy})").FontSize(14).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Sicil").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ad Soyad").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Departman").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Servis").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Çalışma").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Mesai").Bold();
                            });

                            foreach (var personel in Report.Personeller)
                            {
                                table.Cell().Padding(5).Text(personel.SicilNo.ToString());
                                table.Cell().Padding(5).Text(personel.AdSoyad);
                                table.Cell().Padding(5).Text(personel.DepartmanAdi);
                                table.Cell().Padding(5).Text(personel.ServisAdi ?? "-");
                                table.Cell().Padding(5).Text(personel.CalistigiGun.ToString());
                                table.Cell().Padding(5).Text(personel.ToplamMesaiSuresi);
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
                var fileName = $"DepartmanMesaiRaporu_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(pdfBytes), "application/pdf");
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
