using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SGKPortalApp.PresentationLayer.Components.Base;


namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Banko
{
    public partial class Index
    {
        protected override string PagePermissionKey => "SIRAMATIK.BANKO.INDEX";

        [Inject] private IBankoApiService _bankoService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IPersonelApiService _personelService { get; set; } = default!;
        [Inject] private IDepartmanApiService _departmanService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool isDeleting = false;
        private bool showManageModal = false;
        private bool showPersonelAtamaModal = false;
        private bool showDeleteModal = false;
        private bool isEditMode = false;

        // Data
        private List<BankoResponseDto> allBankolar = new();
        private List<BankoResponseDto> filteredBankolar = new();
        private List<BankoKatGrupluResponseDto> groupedBankolar = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<DepartmanResponseDto> departmanlar = new();
        private List<HizmetBinasiResponseDto> modalHizmetBinalari = new();
        private List<PersonelResponseDto> availablePersoneller = new();
        private BankoResponseDto? selectedBanko;
        private BankoFormModel currentBanko = new();

        // Filters
        private string searchText = string.Empty;
        private int selectedHizmetBinasiId = 0;
        private KatTipi? selectedKat = null;
        private BankoTipi? selectedBankoTipi = null;
        private Aktiflik? selectedAktiflik = null;
        private string selectedPersonelTcKimlikNo = string.Empty;
        
        // Modal Filters
        private int modalSelectedDepartmanId = 0;

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadDropdownData();

            // İlk hizmet binasını seç
            if (hizmetBinalari.Any())
            {
                selectedHizmetBinasiId = hizmetBinalari.First().HizmetBinasiId;
                await LoadData();
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Hizmet binalarını yükle
                var binaResult = await _hizmetBinasiService.GetActiveAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data;
                }
                else
                {
                    hizmetBinalari = new List<HizmetBinasiResponseDto>();
                    await _toastService.ShowWarningAsync("Aktif hizmet binası bulunamadı");
                }

                // Departmanları yükle
                var deptResult = await _departmanService.GetActiveAsync();
                if (deptResult.Success && deptResult.Data != null)
                {
                    departmanlar = deptResult.Data;
                }
                else
                {
                    departmanlar = new List<DepartmanResponseDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
        }

        private async Task LoadData()
        {
            if (selectedHizmetBinasiId <= 0)
            {
                await _toastService.ShowWarningAsync("Lütfen bir hizmet binası seçiniz");
                return;
            }

            try
            {
                isLoading = true;

                // Bankoları yükle
                var result = await _bankoService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                if (result.Success && result.Data != null)
                {
                    allBankolar = result.Data;
                    ApplyFilters();
                }
                else
                {
                    allBankolar = new List<BankoResponseDto>();
                    filteredBankolar = new List<BankoResponseDto>();
                    groupedBankolar = new List<BankoKatGrupluResponseDto>();
                    await _toastService.ShowWarningAsync(result.Message ?? "Banko bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bankolar yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Bankolar yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ApplyFilters()
        {
            filteredBankolar = allBankolar.ToList();

            // Kat filtresi
            if (selectedKat.HasValue)
            {
                filteredBankolar = filteredBankolar.Where(b => b.KatTipi == selectedKat.Value).ToList();
            }

            // Banko tipi filtresi
            if (selectedBankoTipi.HasValue)
            {
                filteredBankolar = filteredBankolar.Where(b => b.BankoTipi == selectedBankoTipi.Value).ToList();
            }

            // Aktiflik filtresi
            if (selectedAktiflik.HasValue)
            {
                filteredBankolar = filteredBankolar.Where(b => b.Aktiflik == selectedAktiflik.Value).ToList();
            }

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredBankolar = filteredBankolar
                    .Where(b => b.BankoNo.ToString().ContainsTurkish(searchText) ||
                               (b.BankoAciklama?.ContainsTurkish(searchText, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            // Kat bazlı gruplama
            groupedBankolar = filteredBankolar
                .GroupBy(b => b.KatTipi)
                .Select(g => new BankoKatGrupluResponseDto
                {
                    KatTipi = g.Key,
                    KatTipiAdi = g.Key.GetDisplayName(),
                    Bankolar = g.OrderBy(b => b.BankoNo).ToList()
                })
                .OrderBy(g => (int)g.KatTipi)
                .ToList();
        }

        // ═══════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════════════════════════════

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int binaId))
            {
                selectedHizmetBinasiId = binaId;
                await LoadData();
            }
        }

        private void OnKatChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                selectedKat = null;
            }
            else if (int.TryParse(e.Value.ToString(), out int katValue))
            {
                selectedKat = (KatTipi)katValue;
            }
            ApplyFilters();
        }

        private void OnBankoTipiChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                selectedBankoTipi = null;
            }
            else if (int.TryParse(e.Value.ToString(), out int tipValue))
            {
                selectedBankoTipi = (BankoTipi)tipValue;
            }
            ApplyFilters();
        }

        private void OnAktiflikChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                selectedAktiflik = null;
            }
            else if (int.TryParse(e.Value.ToString(), out int aktiflikValue))
            {
                selectedAktiflik = (Aktiflik)aktiflikValue;
            }
            ApplyFilters();
        }

        private void OnSearchChanged()
        {
            ApplyFilters();
        }

        private void ClearFilters()
        {
            searchText = string.Empty;
            selectedKat = null;
            selectedBankoTipi = null;
            selectedAktiflik = null;
            ApplyFilters();
        }

        private async Task OnModalDepartmanChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int deptId))
            {
                modalSelectedDepartmanId = deptId;
                
                // Departmana göre hizmet binalarını filtrele
                if (deptId > 0)
                {
                    modalHizmetBinalari = hizmetBinalari
                        .Where(b => b.DepartmanId == deptId)
                        .ToList();
                }
                else
                {
                    modalHizmetBinalari = new List<HizmetBinasiResponseDto>();
                }
                
                // Hizmet binası seçimini sıfırla
                currentBanko.HizmetBinasiId = 0;
            }
        }

        private void OnModalHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int binaId))
            {
                currentBanko.HizmetBinasiId = binaId;
            }
        }

        // ═══════════════════════════════════════════════════════
        // MODAL OPERATIONS
        // ═══════════════════════════════════════════════════════

        private void ShowAddModal()
        {
            isEditMode = false;
            modalSelectedDepartmanId = 0;
            modalHizmetBinalari = new List<HizmetBinasiResponseDto>();
            currentBanko = new BankoFormModel
            {
                HizmetBinasiId = 0,
                BankoNo = 0,
                KatTipi = KatTipi.zemin,
                BankoTipi = BankoTipi.Normal
            };
            showManageModal = true;
        }

        private void ShowEditModal(BankoResponseDto banko)
        {
            isEditMode = true;
            currentBanko = new BankoFormModel
            {
                BankoId = banko.BankoId,
                HizmetBinasiId = banko.HizmetBinasiId,
                BankoNo = banko.BankoNo,
                KatTipi = banko.KatTipi,
                BankoTipi = banko.BankoTipi,
                BankoAciklama = banko.BankoAciklama
            };
            showManageModal = true;
        }

        private void HideManageModal()
        {
            showManageModal = false;
            currentBanko = new();
        }

        private async Task ShowPersonelAtamaModal(BankoResponseDto banko)
        {
            selectedBanko = banko;
            selectedPersonelTcKimlikNo = string.Empty;

            // Hizmet binası kontrolü
            if (selectedHizmetBinasiId <= 0)
            {
                await _toastService.ShowWarningAsync("Lütfen önce hizmet binası seçiniz");
                return;
            }

            // Müsait personelleri yükle
            try
            {
                // Hizmet binasına göre personelleri getir (API'den filtrelenmiş)
                var result = await _personelService.GetPersonellerByHizmetBinasiIdAsync(selectedHizmetBinasiId);
                if (result.Success && result.Data != null)
                {
                    // Sadece başka bankoya atanmamış personelleri filtrele
                    var atananPersonelTcler = allBankolar
                        .Where(b => b.AtananPersonel != null)
                        .Select(b => b.AtananPersonel!.TcKimlikNo)
                        .ToHashSet();

                    availablePersoneller = result.Data
                        .Where(p => !atananPersonelTcler.Contains(p.TcKimlikNo))
                        .ToList();

                    if (!availablePersoneller.Any())
                    {
                        var selectedBina = hizmetBinalari.FirstOrDefault(h => h.HizmetBinasiId == selectedHizmetBinasiId);
                        await _toastService.ShowWarningAsync($"{selectedBina?.HizmetBinasiAdi ?? "Bu hizmet binasında"} müsait personel bulunamadı");
                    }
                }
                else
                {
                    availablePersoneller = new List<PersonelResponseDto>();
                    await _toastService.ShowWarningAsync("Personel bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personeller yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Personeller yüklenirken hata oluştu");
            }

            showPersonelAtamaModal = true;
        }

        private void HidePersonelAtamaModal()
        {
            showPersonelAtamaModal = false;
            selectedBanko = null;
            selectedPersonelTcKimlikNo = string.Empty;
        }

        private void ShowDeleteConfirm(BankoResponseDto banko)
        {
            selectedBanko = banko;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            showDeleteModal = false;
            selectedBanko = null;
        }

        // ═══════════════════════════════════════════════════════
        // CRUD OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task SaveBanko()
        {
            try
            {
                isSaving = true;

                if (isEditMode)
                {
                    var updateDto = new BankoUpdateRequestDto
                    {
                        BankoTipi = currentBanko.BankoTipi,
                        BankoAciklama = currentBanko.BankoAciklama
                    };

                    var result = await _bankoService.UpdateAsync(currentBanko.BankoId, updateDto);
                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Banko başarıyla güncellendi");
                        HideManageModal();
                        await LoadData();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Banko güncellenemedi");
                    }
                }
                else
                {
                    var createDto = new BankoCreateRequestDto
                    {
                        HizmetBinasiId = currentBanko.HizmetBinasiId,
                        BankoNo = currentBanko.BankoNo,
                        KatTipi = currentBanko.KatTipi,
                        BankoTipi = currentBanko.BankoTipi,
                        BankoAciklama = currentBanko.BankoAciklama
                    };

                    var result = await _bankoService.CreateAsync(createDto);
                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Banko başarıyla eklendi");
                        HideManageModal();
                        await LoadData();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Banko eklenemedi");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko kaydedilirken hata oluştu");
                await _toastService.ShowErrorAsync("Banko kaydedilirken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task DeleteBanko()
        {
            if (selectedBanko == null) return;

            try
            {
                isDeleting = true;

                var result = await _bankoService.DeleteAsync(selectedBanko.BankoId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Banko başarıyla silindi");
                    HideDeleteConfirm();
                    await LoadData();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Banko silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banko silinirken hata oluştu");
                await _toastService.ShowErrorAsync("Banko silinirken bir hata oluştu");
            }
            finally
            {
                isDeleting = false;
            }
        }

        private async Task ToggleAktiflik(BankoResponseDto banko)
        {
            try
            {
                var result = await _bankoService.ToggleAktiflikAsync(banko.BankoId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"Banko {(banko.Aktiflik == Aktiflik.Aktif ? "pasif" : "aktif")} yapıldı");
                    await LoadData();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Aktiflik durumu değiştirilemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktiflik durumu değiştirilirken hata oluştu");
                await _toastService.ShowErrorAsync("Aktiflik durumu değiştirilirken bir hata oluştu");
            }
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL ATAMA OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task PersonelAta()
        {
            if (selectedBanko == null || string.IsNullOrEmpty(selectedPersonelTcKimlikNo)) return;

            try
            {
                isSaving = true;

                var atamaDto = new BankoPersonelAtaDto
                {
                    BankoId = selectedBanko.BankoId,
                    TcKimlikNo = selectedPersonelTcKimlikNo
                };

                var result = await _bankoService.PersonelAtaAsync(atamaDto);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Personel başarıyla atandı");
                    HidePersonelAtamaModal();
                    await LoadData();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Personel atanamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel atanırken hata oluştu");
                await _toastService.ShowErrorAsync("Personel atanırken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task PersonelCikar()
        {
            if (selectedBanko?.AtananPersonel == null) return;

            try
            {
                isSaving = true;

                var result = await _bankoService.PersonelCikarAsync(selectedBanko.AtananPersonel.TcKimlikNo);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Personel başarıyla çıkarıldı");
                    HidePersonelAtamaModal();
                    await LoadData();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Personel çıkarılamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel çıkarılırken hata oluştu");
                await _toastService.ShowErrorAsync("Personel çıkarılırken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // EXPORT OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task ExportToExcel()
        {
            try
            {
                isLoading = true;

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Bankolar");

                // Header
                worksheet.Cell(1, 1).Value = "Banko No";
                worksheet.Cell(1, 2).Value = "Kat";
                worksheet.Cell(1, 3).Value = "Banko Tipi";
                worksheet.Cell(1, 4).Value = "Açıklama";
                worksheet.Cell(1, 5).Value = "Atanan Personel";
                worksheet.Cell(1, 6).Value = "Durum";

                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var banko in filteredBankolar.OrderBy(b => b.KatTipi).ThenBy(b => b.BankoNo))
                {
                    worksheet.Cell(row, 1).Value = banko.BankoNo;
                    worksheet.Cell(row, 2).Value = banko.KatTipiAdi;
                    worksheet.Cell(row, 3).Value = banko.BankoTipiAdi;
                    worksheet.Cell(row, 4).Value = banko.BankoAciklama ?? "-";
                    worksheet.Cell(row, 5).Value = banko.AtananPersonel?.AdSoyad ?? "Atanmamış";
                    worksheet.Cell(row, 6).Value = banko.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                await JSRuntime.InvokeVoidAsync("downloadFile", "Bankolar.xlsx", Convert.ToBase64String(content));
                await _toastService.ShowSuccessAsync("Excel dosyası başarıyla indirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel export hatası");
                await _toastService.ShowErrorAsync("Excel dosyası oluşturulamadı");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task ExportToPdf()
        {
            try
            {
                isLoading = true;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Text("Banko Listesi").FontSize(20).Bold().AlignCenter();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(60);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(60);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Banko No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kat").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Banko Tipi").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Açıklama").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Atanan Personel").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").Bold();
                            });

                            foreach (var banko in filteredBankolar.OrderBy(b => b.KatTipi).ThenBy(b => b.BankoNo))
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(banko.BankoNo.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(banko.KatTipiAdi);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(banko.BankoTipiAdi);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(banko.BankoAciklama ?? "-");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(banko.AtananPersonel?.AdSoyad ?? "Atanmamış");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(banko.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif");
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Sayfa ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                await JSRuntime.InvokeVoidAsync("downloadFile", "Bankolar.pdf", Convert.ToBase64String(pdfBytes));
                await _toastService.ShowSuccessAsync("PDF dosyası başarıyla indirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF export hatası");
                await _toastService.ShowErrorAsync("PDF dosyası oluşturulamadı");
            }
            finally
            {
                isLoading = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private string GetBankoTipiBadgeClass(BankoTipi tip)
        {
            return tip switch
            {
                BankoTipi.Normal => "bg-label-primary",
                BankoTipi.Oncelikli => "bg-label-warning",
                BankoTipi.Engelli => "bg-label-info",
                BankoTipi.SefMasasi => "bg-label-success",
                _ => "bg-label-secondary"
            };
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private string GetAddBankoUrl()
        {
            if (selectedHizmetBinasiId > 0)
            {
                return $"/siramatik/banko/manage?hizmetBinasiId={selectedHizmetBinasiId}";
            }
            return "/siramatik/banko/manage";
        }
    }
}
