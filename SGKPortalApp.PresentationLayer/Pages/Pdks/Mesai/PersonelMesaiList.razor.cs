using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
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
    public partial class PersonelMesaiList
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IPersonelListApiService _personelListService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime _jsRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<PersonelListResponseDto> Personeller { get; set; } = new();
        private List<DepartmanDto> Departmanlar { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int? filterDepartmanId = null;
        private int? userDepartmanId = null;
        private string searchTerm = string.Empty;

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
            await LoadPersonelList();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING METHODS
        // ═══════════════════════════════════════════════════════

        private async Task LoadFilterData()
        {
            try
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var departmanIdClaim = authState.User.FindFirst("DepartmanId")?.Value;
                if (int.TryParse(departmanIdClaim, out var deptId))
                {
                    userDepartmanId = deptId;
                    filterDepartmanId = deptId; // Set user's departman as default filter
                }

                var deptResult = await _personelListService.GetDepartmanListeAsync();
                if (deptResult.Success && deptResult.Data != null)
                {
                    Departmanlar = deptResult.Data;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Filtre verileri yüklenirken hata");
            }
        }

        private async Task LoadPersonelList()
        {
            try
            {
                IsLoading = true;
                Personeller.Clear();

                var request = new
                {
                    DepartmanId = filterDepartmanId,
                    SadeceAktifler = true,
                    AramaMetni = searchTerm
                };

                var result = await _personelListService.GetPersonelListeAsync(request);

                if (result.Success && result.Data != null)
                {
                    Personeller = result.Data;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Personel listesi yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Personel listesi yüklenirken hata");
            }
            finally
            {
                IsLoading = false;
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
                await LoadPersonelList();
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

        private int GetCurrentUserDepartmanId()
        {
            return userDepartmanId ?? 0;
        }

        private async Task<string?> GetCurrentUserTcKimlikNoAsync()
        {
            try
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                return authState.User.FindFirst("TcKimlikNo")?.Value;
            }
            catch
            {
                return null;
            }
        }

        private async Task<int> GetCurrentUserHizmetBinasiIdAsync()
        {
            try
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var hizmetBinasiId = authState.User.FindFirst("HizmetBinasiId")?.Value;
                return int.TryParse(hizmetBinasiId, out var id) ? id : 0;
            }
            catch
            {
                return 0;
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
                var worksheet = workbook.Worksheets.Add("Personel Listesi");

                // Header
                worksheet.Cell(1, 1).Value = "Sicil No";
                worksheet.Cell(1, 2).Value = "TC Kimlik No";
                worksheet.Cell(1, 3).Value = "Ad Soyad";
                worksheet.Cell(1, 4).Value = "Departman";
                worksheet.Cell(1, 5).Value = "Servis";
                worksheet.Cell(1, 6).Value = "Ünvan";
                worksheet.Cell(1, 7).Value = "Durum";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var personel in Personeller)
                {
                    worksheet.Cell(row, 1).Value = personel.SicilNo;
                    worksheet.Cell(row, 2).Value = personel.TcKimlikNo;
                    worksheet.Cell(row, 3).Value = personel.AdSoyad;
                    worksheet.Cell(row, 4).Value = personel.DepartmanAdi;
                    worksheet.Cell(row, 5).Value = personel.ServisAdi;
                    worksheet.Cell(row, 6).Value = personel.UnvanAdi;
                    worksheet.Cell(row, 7).Value = personel.PersonelAktiflikDurum.ToString();
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"PersonelListesi_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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

                        page.Header().Text("Personel Listesi").FontSize(20).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Sicil No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("TC Kimlik").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ad Soyad").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Departman").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Servis").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ünvan").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").Bold();
                            });

                            foreach (var personel in Personeller)
                            {
                                table.Cell().Padding(5).Text(personel.SicilNo.ToString());
                                table.Cell().Padding(5).Text(personel.TcKimlikNo);
                                table.Cell().Padding(5).Text(personel.AdSoyad);
                                table.Cell().Padding(5).Text(personel.DepartmanAdi);
                                table.Cell().Padding(5).Text(personel.ServisAdi);
                                table.Cell().Padding(5).Text(personel.UnvanAdi);
                                table.Cell().Padding(5).Text(personel.PersonelAktiflikDurum.ToString());
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
                var fileName = $"PersonelListesi_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

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
