using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Mesai
{
    public partial class MesaiListele
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IPersonelMesaiApiService _personelMesaiService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;
        [Inject] private IJSRuntime _jsRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter]
        [SupplyParameterFromQuery(Name = "tc")]
        public string? TcKimlikNo { get; set; }

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private PersonelMesaiBaslikDto? BaslikBilgi { get; set; }
        private List<PersonelMesaiListResponseDto> MesaiListesi { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

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

            if (string.IsNullOrEmpty(TcKimlikNo))
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                TcKimlikNo = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            if (string.IsNullOrEmpty(TcKimlikNo))
            {
                Logger.LogWarning("Kullanıcı TC kimlik no bulunamadı");
                return;
            }

            await LoadBaslikBilgi();
            await LoadMesaiList();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING METHODS
        // ═══════════════════════════════════════════════════════

        private async Task LoadBaslikBilgi()
        {
            try
            {
                var result = await _personelMesaiService.GetBaslikAsync(TcKimlikNo);

                if (result.Success && result.Data != null)
                {
                    BaslikBilgi = result.Data;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Başlık bilgisi yüklenirken hata");
            }
        }

        private async Task LoadMesaiList()
        {
            try
            {
                IsLoading = true;
                MesaiListesi.Clear();

                var request = new PersonelMesaiFilterRequestDto
                {
                    TcKimlikNo = TcKimlikNo,
                    BaslangicTarihi = baslangicTarihi,
                    BitisTarihi = bitisTarihi
                };

                var result = await _personelMesaiService.GetListeAsync(request);

                if (result.Success && result.Data != null)
                {
                    MesaiListesi = result.Data.OrderByDescending(m => m.Tarih).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Mesai listesi yüklenirken hata");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // EXPORT METHODS
        // ═══════════════════════════════════════════════════════

        private async Task ExportToExcel()
        {
            if (MesaiListesi == null || !MesaiListesi.Any())
            {
                await _jsRuntime.InvokeVoidAsync("showToast", "warning", "Dışa aktarılacak veri bulunamadı!");
                return;
            }

            IsExporting = true;
            ExportType = "excel";

            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Mesai Listesi");

                // Header
                worksheet.Cell(1, 1).Value = "Tarih";
                worksheet.Cell(1, 2).Value = "Giriş Saati";
                worksheet.Cell(1, 3).Value = "Çıkış Saati";
                worksheet.Cell(1, 4).Value = "Mesai Süresi";
                worksheet.Cell(1, 5).Value = "Detay";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var mesai in MesaiListesi)
                {
                    worksheet.Cell(row, 1).Value = mesai.Tarih.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 2).Value = mesai.GirisSaati?.ToString(@"hh\:mm") ?? "-";
                    worksheet.Cell(row, 3).Value = mesai.CikisSaati?.ToString(@"hh\:mm") ?? "-";
                    worksheet.Cell(row, 4).Value = mesai.MesaiSuresi ?? "-";
                    worksheet.Cell(row, 5).Value = mesai.Detay ?? "-";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"MesaiListesi_{BaslikBilgi?.AdSoyad?.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                await _jsRuntime.InvokeVoidAsync("showToast", "success", "Excel dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await _jsRuntime.InvokeVoidAsync("showToast", "error", $"Excel oluşturulurken hata: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }

        private async Task ExportToPdf()
        {
            if (MesaiListesi == null || !MesaiListesi.Any())
            {
                await _jsRuntime.InvokeVoidAsync("showToast", "warning", "Dışa aktarılacak veri bulunamadı!");
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
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Column(column =>
                        {
                            column.Item().Text($"Mesai Listesi - {BaslikBilgi?.AdSoyad}").FontSize(16).Bold();
                            column.Item().Text($"{baslangicTarihi:dd.MM.yyyy} - {bitisTarihi:dd.MM.yyyy}").FontSize(10);
                        });

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tarih").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Giriş").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Çıkış").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Mesai").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Detay").Bold();
                            });

                            foreach (var mesai in MesaiListesi)
                            {
                                table.Cell().Padding(5).Text(mesai.Tarih.ToString("dd.MM.yyyy"));
                                table.Cell().Padding(5).Text(mesai.GirisSaati?.ToString(@"hh\:mm") ?? "-");
                                table.Cell().Padding(5).Text(mesai.CikisSaati?.ToString(@"hh\:mm") ?? "-");
                                table.Cell().Padding(5).Text(mesai.MesaiSuresi ?? "-");
                                table.Cell().Padding(5).Text(mesai.Detay ?? "-");
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
                var fileName = $"MesaiListesi_{BaslikBilgi?.AdSoyad?.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(pdfBytes), "application/pdf");
                await _jsRuntime.InvokeVoidAsync("showToast", "success", "PDF dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await _jsRuntime.InvokeVoidAsync("showToast", "error", $"PDF oluşturulurken hata: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }
    }
}
