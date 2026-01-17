using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class OnayBekleyenler
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IIzinMazeretTalepApiService _izinMazeretTalepService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;
        [Inject] private IJSRuntime _js { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<IzinMazeretTalepListResponseDto> Talepler { get; set; } = new();
        private IzinMazeretTalepListResponseDto? SelectedTalep { get; set; }
        private string OnayNotu { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = false;
        private bool ShowApprovalModal { get; set; } = false;
        private bool ShowDetailModal { get; set; } = false;
        private bool IsApproving { get; set; } = false;
        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadTalepler();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING METHODS
        // ═══════════════════════════════════════════════════════

        private async Task LoadTalepler()
        {
            try
            {
                IsLoading = true;
                Talepler.Clear();

                var onayciTcKimlikNo = await GetCurrentUserTcKimlikNoAsync();
                if (string.IsNullOrEmpty(onayciTcKimlikNo))
                {
                    await ShowToast("error", "Kullanıcı bilgisi alınamadı");
                    return;
                }

                var result = await _izinMazeretTalepService.GetPendingApprovalsAsync(onayciTcKimlikNo);

                if (result.Success && result.Data != null)
                {
                    Talepler = result.Data.OrderByDescending(t => t.TalepTarihi).ToList();
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talepler yüklenemedi");
                }
            }
            catch
            {
                await ShowToast("error", "Talepler yüklenirken bir hata oluştu");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════════════════════════════

        private void OpenApprovalModal(IzinMazeretTalepListResponseDto talep, bool approve)
        {
            SelectedTalep = talep;
            IsApproving = approve;
            OnayNotu = string.Empty;
            ShowApprovalModal = true;
        }

        private void CloseApprovalModal()
        {
            ShowApprovalModal = false;
            SelectedTalep = null;
            OnayNotu = string.Empty;
        }

        private void OpenDetailModal(IzinMazeretTalepListResponseDto talep)
        {
            SelectedTalep = talep;
            ShowDetailModal = true;
        }

        private void CloseDetailModal()
        {
            ShowDetailModal = false;
            SelectedTalep = null;
        }

        private async Task SubmitApproval()
        {
            if (SelectedTalep == null) return;

            if (!IsApproving && string.IsNullOrWhiteSpace(OnayNotu))
            {
                await ShowToast("warning", "Red nedeni belirtmeniz zorunludur");
                return;
            }

            try
            {
                var onayciSeviyesi = SelectedTalep.BirinciOnayDurumu == SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri.OnayDurumu.Beklemede ? 1 : 2;

                var request = new IzinMazeretTalepOnayRequestDto
                {
                    OnayDurumu = IsApproving
                        ? SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri.OnayDurumu.Onaylandi
                        : SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri.OnayDurumu.Reddedildi,
                    Aciklama = OnayNotu,
                    OnayciSeviyesi = onayciSeviyesi
                };

                var result = await _izinMazeretTalepService.ApproveOrRejectAsync(SelectedTalep.IzinMazeretTalepId, request);

                if (result.Success)
                {
                    await ShowToast("success", IsApproving
                        ? "✅ Talep başarıyla onaylandı"
                        : "❌ Talep reddedildi");

                    CloseApprovalModal();
                    await LoadTalepler();
                }
                else
                {
                    await ShowToast("error", result.Message ?? "İşlem başarısız");
                }
            }
            catch
            {
                await ShowToast("error", "İşlem sırasında bir hata oluştu");
            }
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private async Task ShowToast(string type, string message)
        {
            try
            {
                await _js.InvokeVoidAsync("showToast", type, message);
            }
            catch
            {
                // Toast gösterilemezse sessizce devam et
            }
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

        // ═══════════════════════════════════════════════════════
        // EXPORT METHODS
        // ═══════════════════════════════════════════════════════

        private async Task ExportToExcel()
        {
            if (Talepler == null || !Talepler.Any())
            {
                await ShowToast("warning", "Dışa aktarılacak veri bulunamadı!");
                return;
            }

            IsExporting = true;
            ExportType = "excel";

            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Onay Bekleyen Talepler");

                // Header
                worksheet.Cell(1, 1).Value = "Talep No";
                worksheet.Cell(1, 2).Value = "Personel";
                worksheet.Cell(1, 3).Value = "TC Kimlik";
                worksheet.Cell(1, 4).Value = "Tür";
                worksheet.Cell(1, 5).Value = "Başlangıç";
                worksheet.Cell(1, 6).Value = "Bitiş";
                worksheet.Cell(1, 7).Value = "Toplam Gün";
                worksheet.Cell(1, 8).Value = "1. Onay";
                worksheet.Cell(1, 9).Value = "2. Onay";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 9);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var talep in Talepler)
                {
                    worksheet.Cell(row, 1).Value = talep.IzinMazeretTalepId;
                    worksheet.Cell(row, 2).Value = talep.AdSoyad;
                    worksheet.Cell(row, 3).Value = talep.TcKimlikNo;
                    worksheet.Cell(row, 4).Value = talep.TuruAdi;
                    worksheet.Cell(row, 5).Value = talep.BaslangicTarihi?.ToString("dd.MM.yyyy") ?? "-";
                    worksheet.Cell(row, 6).Value = talep.BitisTarihi?.ToString("dd.MM.yyyy") ?? "-";
                    worksheet.Cell(row, 7).Value = talep.ToplamGun?.ToString() ?? "-";
                    worksheet.Cell(row, 8).Value = talep.BirinciOnayDurumuAdi;
                    worksheet.Cell(row, 9).Value = talep.IkinciOnayDurumuAdi;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"OnayBekleyenTalepler_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                await _js.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                await ShowToast("success", "Excel dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await ShowToast("error", $"Excel oluşturulurken hata: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }

        private async Task ExportToPdf()
        {
            if (Talepler == null || !Talepler.Any())
            {
                await ShowToast("warning", "Dışa aktarılacak veri bulunamadı!");
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
                        page.DefaultTextStyle(x => x.FontSize(8));

                        page.Header().Text("Onay Bekleyen İzin/Mazeret Talepleri").FontSize(14).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1.5f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Personel").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("TC").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Tür").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Başlangıç").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Bitiş").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Gün").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("1. Onay").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("2. Onay").Bold();
                            });

                            foreach (var talep in Talepler)
                            {
                                table.Cell().Padding(3).Text(talep.IzinMazeretTalepId.ToString());
                                table.Cell().Padding(3).Text(talep.AdSoyad);
                                table.Cell().Padding(3).Text(talep.TcKimlikNo);
                                table.Cell().Padding(3).Text(talep.TuruAdi);
                                table.Cell().Padding(3).Text(talep.BaslangicTarihi?.ToString("dd.MM.yy") ?? "-");
                                table.Cell().Padding(3).Text(talep.BitisTarihi?.ToString("dd.MM.yy") ?? "-");
                                table.Cell().Padding(3).Text(talep.ToplamGun?.ToString() ?? "-");
                                table.Cell().Padding(3).Text(talep.BirinciOnayDurumuAdi);
                                table.Cell().Padding(3).Text(talep.IkinciOnayDurumuAdi);
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
                var fileName = $"OnayBekleyenTalepler_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                await _js.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(pdfBytes), "application/pdf");
                await ShowToast("success", "PDF dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await ShowToast("error", $"PDF oluşturulurken hata: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }
    }
}
