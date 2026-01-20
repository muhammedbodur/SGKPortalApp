using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
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
    public partial class Taleplerim
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IIzinMazeretTalepApiService _izinMazeretTalepService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;
        [Inject] private IJSRuntime _js { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<IzinMazeretTalepListResponseDto> Talepler { get; set; } = new();
        private List<IzinMazeretTalepListResponseDto> FilteredTalepler { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string SearchTerm { get; set; } = string.Empty;
        private string DurumFilter { get; set; } = string.Empty;
        private bool ShowInactive { get; set; } = false;

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

                var userTc = await GetCurrentUserTcKimlikNoAsync();
                if (string.IsNullOrEmpty(userTc))
                {
                    await ShowToast("error", "Kullanıcı bilgisi alınamadı");
                    return;
                }

                var result = await _izinMazeretTalepService.GetByPersonelAsync(userTc);

                if (result.Success && result.Data != null)
                {
                    Talepler = result.Data;
                    FilterTalepler();
                    await ShowToast("success", result.Message ?? "Talepler yüklendi");
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

        private void FilterTalepler()
        {
            var query = Talepler.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(t =>
                    t.TuruAdi.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.AdSoyad.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.IzinMazeretTalepId.ToString().Contains(SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(DurumFilter))
            {
                query = query.Where(t => t.GenelDurum == DurumFilter);
            }

            FilteredTalepler = query.ToList();
        }

        // ═══════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════════════════════════════

        private void ViewDetails(int id)
        {
            _navigationManager.NavigateTo($"/pdks/izin/detay/{id}");
        }

        private void EditTalep(int id)
        {
            _navigationManager.NavigateTo($"/pdks/izin/duzenle/{id}");
        }

        private async Task CancelTalep(int id)
        {
            var confirmed = await _js.InvokeAsync<bool>("confirm", "Bu talebi iptal etmek istediğinizden emin misiniz?");
            if (!confirmed) return;

            try
            {
                var iptalNedeni = await _js.InvokeAsync<string>("prompt", "İptal nedeni:");
                if (string.IsNullOrWhiteSpace(iptalNedeni)) return;

                var result = await _izinMazeretTalepService.CancelAsync(id, iptalNedeni);

                if (result.Success)
                {
                    await ShowToast("success", result.Message ?? "Talep iptal edildi");
                    await LoadTalepler();
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talep iptal edilemedi");
                }
            }
            catch
            {
                await ShowToast("error", "Bir hata oluştu");
            }
        }

        private async Task DeleteTalep(int id)
        {
            var confirmed = await _js.InvokeAsync<bool>("confirm",
                "Bu talebi kalıcı olarak silmek istediğinizden emin misiniz?\n\nBu işlem geri alınamaz!");
            if (!confirmed) return;

            try
            {
                var result = await _izinMazeretTalepService.DeleteAsync(id);

                if (result.Success)
                {
                    await ShowToast("success", result.Message ?? "Talep başarıyla silindi");
                    await LoadTalepler();
                }
                else
                {
                    await ShowToast("error", result.Message ?? "Talep silinemedi");
                }
            }
            catch
            {
                await ShowToast("error", "Talep silinirken bir hata oluştu");
            }
        }

        private bool CanDeleteTalep(IzinMazeretTalepListResponseDto talep)
        {
            // Tek onaycı: BirinciOnay Beklemede ise silebilir
            // İki onaycı: Hem BirinciOnay hem de IkinciOnay Beklemede ise silebilir

            // Eğer birinci onay onaylandı veya reddedildi ise silinemez
            if (talep.BirinciOnayDurumuAdi != "Beklemede")
                return false;

            // Eğer ikinci onay var ve onaylandı/reddedildi ise silinemez
            if (!string.IsNullOrEmpty(talep.IkinciOnayDurumuAdi) &&
                talep.IkinciOnayDurumuAdi != "Beklemede")
                return false;

            return true;
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private string GetOnayBadgeClass(string onayDurumu)
        {
            return onayDurumu switch
            {
                "Beklemede" => "bg-label-warning",
                "Onaylandı" => "bg-label-success",
                "Reddedildi" => "bg-label-danger",
                "İptal Edildi" => "bg-label-secondary",
                _ => "bg-label-secondary"
            };
        }

        private string GetDurumBadgeClass(string durum)
        {
            return durum switch
            {
                "Onaylandı" => "bg-label-success",
                "Reddedildi" => "bg-label-danger",
                "İptal" => "bg-label-secondary",
                _ => "bg-label-warning" // Beklemede
            };
        }

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
            if (FilteredTalepler == null || !FilteredTalepler.Any())
            {
                await ShowToast("warning", "Dışa aktarılacak veri bulunamadı!");
                return;
            }

            IsExporting = true;
            ExportType = "excel";

            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("İzin Taleplerim");

                // Header
                worksheet.Cell(1, 1).Value = "Talep No";
                worksheet.Cell(1, 2).Value = "Tür";
                worksheet.Cell(1, 3).Value = "Başlangıç";
                worksheet.Cell(1, 4).Value = "Bitiş";
                worksheet.Cell(1, 5).Value = "Toplam Gün";
                worksheet.Cell(1, 6).Value = "Talep Tarihi";
                worksheet.Cell(1, 7).Value = "Durum";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var talep in FilteredTalepler)
                {
                    worksheet.Cell(row, 1).Value = talep.IzinMazeretTalepId;
                    worksheet.Cell(row, 2).Value = talep.TuruAdi;
                    worksheet.Cell(row, 3).Value = talep.BaslangicTarihi?.ToString("dd.MM.yyyy") ?? "-";
                    worksheet.Cell(row, 4).Value = talep.BitisTarihi?.ToString("dd.MM.yyyy") ?? "-";
                    worksheet.Cell(row, 5).Value = talep.ToplamGun?.ToString() ?? "-";
                    worksheet.Cell(row, 6).Value = talep.TalepTarihi.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(row, 7).Value = talep.GenelDurum;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"IzinTaleplerim_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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
            if (FilteredTalepler == null || !FilteredTalepler.Any())
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
                        page.DefaultTextStyle(x => x.FontSize(9));

                        page.Header().Text("İzin/Mazeret Taleplerim").FontSize(16).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Talep No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tür").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Başlangıç").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Bitiş").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Gün").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Talep Tarihi").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").Bold();
                            });

                            foreach (var talep in FilteredTalepler)
                            {
                                table.Cell().Padding(5).Text(talep.IzinMazeretTalepId.ToString());
                                table.Cell().Padding(5).Text(talep.TuruAdi);
                                table.Cell().Padding(5).Text(talep.BaslangicTarihi?.ToString("dd.MM.yyyy") ?? "-");
                                table.Cell().Padding(5).Text(talep.BitisTarihi?.ToString("dd.MM.yyyy") ?? "-");
                                table.Cell().Padding(5).Text(talep.ToplamGun?.ToString() ?? "-");
                                table.Cell().Padding(5).Text(talep.TalepTarihi.ToString("dd.MM.yyyy"));
                                table.Cell().Padding(5).Text(talep.GenelDurum);
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
                var fileName = $"IzinTaleplerim_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

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
