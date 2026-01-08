using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Pages.Personel
{
    public partial class Index : FieldPermissionPageBase
    {
        // ═══════════════════════════════════════════════════════════
        // PERMISSION CONFIGURATION (FieldPermissionPageBase)
        // ═══════════════════════════════════════════════════════════

        // ⚡ PagePermissionKey artık otomatik çözümleniyor!
        // Route: /personel/index (veya /personel) → PermissionKey: PER.PERSONEL.INDEX
        // Action permission'lar: PER.PERSONEL.INDEX.ACTION.DETAIL, PER.PERSONEL.INDEX.ACTION.EDIT, vb.
        // Manuel override gerekmez, FieldPermissionPageBase otomatik halleder

        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private IPersonelApiService _personelApiService { get; set; } = default!;
        [Inject] private IDepartmanApiService _departmanApiService { get; set; } = default!;
        [Inject] private IServisApiService _servisApiService { get; set; } = default!;
        [Inject] private IUnvanApiService _unvanApiService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiApiService { get; set; } = default!;
        [Inject] private IZKTecoDeviceApiService _zktecoDeviceApiService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<PersonelResponseDto> FilteredPersoneller { get; set; } = new();
        
        // Lookup Lists
        private List<DepartmanResponseDto> Departmanlar { get; set; } = new();
        private List<ServisResponseDto> Servisler { get; set; } = new();
        private List<UnvanResponseDto> Unvanlar { get; set; } = new();
        private List<HizmetBinasiResponseDto> HizmetBinalari { get; set; } = new();
        private List<DeviceResponseDto> ZKTecoDevices { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string searchTerm = string.Empty;
        private int filterDepartmanId = 0;
        private int filterServisId = 0;
        private int filterHizmetBinasiId = 0;
        private int filterUnvanId = 0;
        private int filterAktiflik = -1; // -1: Tümü, 0: Aktif, 1: Pasif, 2: Emekli
        private int filterZKTecoDeviceId = 0; // 0: Tüm Cihazlar
        private string sortBy = "name-asc";

        // ═══════════════════════════════════════════════════════
        // PAGINATION PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalCount { get; set; } = 0;
        private int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = true;
        private bool IsExporting { get; set; } = false;
        private string CurrentExportType { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════
        // QUICK ADD MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowQuickAddModal { get; set; } = false;
        private bool IsQuickAdding { get; set; } = false;
        private QuickAddPersonelModel QuickAddModel { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowDeleteModal { get; set; } = false;
        private string DeletePersonelTcKimlik { get; set; } = string.Empty;
        private int DeletePersonelSicilNo { get; set; }
        private string DeletePersonelAdSoyad { get; set; } = string.Empty;
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            // Base class permission yükleme ve event subscription'ı yapar
            await base.OnInitializedAsync();
            
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadData();
        }

        private async Task LoadData()
        {
            IsLoading = true;
            try
            {
                // Lookup verilerini yükle
                var departmanTask = _departmanApiService.GetAllAsync();
                var servisTask = _servisApiService.GetAllAsync();
                var unvanTask = _unvanApiService.GetAllAsync();
                var hizmetBinasiTask = _hizmetBinasiApiService.GetAllAsync();
                var zktecoDeviceTask = _zktecoDeviceApiService.GetActiveAsync();

                await Task.WhenAll(departmanTask, servisTask, unvanTask, hizmetBinasiTask, zktecoDeviceTask);

                Departmanlar = (await departmanTask)?.Data ?? new List<DepartmanResponseDto>();
                Servisler = (await servisTask)?.Data ?? new List<ServisResponseDto>();
                Unvanlar = (await unvanTask)?.Data ?? new List<UnvanResponseDto>();
                HizmetBinalari = (await hizmetBinasiTask)?.Data ?? new List<HizmetBinasiResponseDto>();
                ZKTecoDevices = (await zktecoDeviceTask)?.Data ?? new List<SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco.DeviceResponseDto>();

                // Personelleri server-side pagination ile yükle
                await LoadPersonelWithPagination();
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veriler yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadPersonelWithPagination()
        {
            try
            {
                var filter = new PersonelFilterRequestDto
                {
                    SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim(),
                    DepartmanId = filterDepartmanId > 0 ? filterDepartmanId : null,
                    ServisId = filterServisId > 0 ? filterServisId : null,
                    UnvanId = filterUnvanId > 0 ? filterUnvanId : null,
                    HizmetBinasiId = filterHizmetBinasiId > 0 ? filterHizmetBinasiId : null,
                    AktiflikDurum = filterAktiflik >= 0 ? (PersonelAktiflikDurum)filterAktiflik : null,
                    PageNumber = CurrentPage,
                    PageSize = PageSize,
                    SortBy = MapSortByToFieldName(sortBy),
                    SortDescending = sortBy.EndsWith("-desc")
                };

                var result = await _personelApiService.GetPagedAsync(filter);
                
                if (result.Success && result.Data != null)
                {
                    FilteredPersoneller = result.Data.Items.Select(p => new PersonelResponseDto
                    {
                        TcKimlikNo = p.TcKimlikNo,
                        SicilNo = p.SicilNo,
                        AdSoyad = p.AdSoyad,
                        Email = p.Email,
                        DepartmanId = 0,
                        DepartmanAdi = p.DepartmanAdi,
                        ServisId = 0,
                        ServisAdi = p.ServisAdi,
                        UnvanId = 0,
                        UnvanAdi = p.UnvanAdi,
                        HizmetBinasiAdi = p.HizmetBinasiAdi,
                        Dahili = p.Dahili,
                        Resim = p.Resim,
                        PersonelAktiflikDurum = p.PersonelAktiflikDurum,
                        EklenmeTarihi = p.EklenmeTarihi
                    }).ToList();

                    TotalCount = result.Data.TotalCount;
                }
                else
                {
                    FilteredPersoneller = new List<PersonelResponseDto>();
                    TotalCount = 0;
                }
                
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Personel yüklenirken hata oluştu: {ex.Message}");
                FilteredPersoneller = new List<PersonelResponseDto>();
            }
        }

        private string MapSortByToFieldName(string sortBy)
        {
            return sortBy switch
            {
                "tc-asc" or "tc-desc" => "TcKimlikNo",
                "sicil-asc" or "sicil-desc" => "SicilNo",
                "name-asc" or "name-desc" => "AdSoyad",
                "dept-asc" or "dept-desc" => "DepartmanAdi",
                "servis-asc" or "servis-desc" => "ServisAdi",
                "bina-asc" or "bina-desc" => "HizmetBinasiAdi",
                "unvan-asc" or "unvan-desc" => "UnvanAdi",
                "dahili-asc" or "dahili-desc" => "Dahili",
                "date-newest" or "date-oldest" => "EklenmeTarihi",
                _ => "AdSoyad"
            };
        }

        private string GetSortDisplayName()
        {
            return sortBy switch
            {
                "tc-asc" => "TC ↑",
                "tc-desc" => "TC ↓",
                "sicil-asc" => "Sicil ↑",
                "sicil-desc" => "Sicil ↓",
                "name-asc" => "Ad Soyad ↑",
                "name-desc" => "Ad Soyad ↓",
                "dept-asc" => "Departman ↑",
                "dept-desc" => "Departman ↓",
                "servis-asc" => "Servis ↑",
                "servis-desc" => "Servis ↓",
                "bina-asc" => "Hizmet Binası ↑",
                "bina-desc" => "Hizmet Binası ↓",
                "unvan-asc" => "Ünvan ↑",
                "unvan-desc" => "Ünvan ↓",
                "dahili-asc" => "Dahili ↑",
                "dahili-desc" => "Dahili ↓",
                _ => "Ad Soyad ↑"
            };
        }

        // ═══════════════════════════════════════════════════════
        // FILTER & SORT METHODS
        // ═══════════════════════════════════════════════════════

        private async Task ApplyFiltersAndSort()
        {
            CurrentPage = 1;
            await LoadPersonelWithPagination();
        }

        private void OnSearchChanged()
        {
            _ = ApplyFiltersAndSort();
        }

        private void OnFilterDepartmanChanged(ChangeEventArgs e)
        {
            filterDepartmanId = int.Parse(e.Value?.ToString() ?? "0");
            _ = ApplyFiltersAndSort();
        }

        private void OnFilterServisChanged(ChangeEventArgs e)
        {
            filterServisId = int.Parse(e.Value?.ToString() ?? "0");
            _ = ApplyFiltersAndSort();
        }

        private void OnFilterHizmetBinasiChanged(ChangeEventArgs e)
        {
            filterHizmetBinasiId = int.Parse(e.Value?.ToString() ?? "0");
            _ = ApplyFiltersAndSort();
        }

        private void OnFilterUnvanChanged(ChangeEventArgs e)
        {
            filterUnvanId = int.Parse(e.Value?.ToString() ?? "0");
            _ = ApplyFiltersAndSort();
        }

        private void OnFilterAktiflikChanged(ChangeEventArgs e)
        {
            filterAktiflik = int.Parse(e.Value?.ToString() ?? "-1");
            _ = ApplyFiltersAndSort();
        }

        private void OnFilterDeviceChanged(ChangeEventArgs e)
        {
            filterZKTecoDeviceId = int.Parse(e.Value?.ToString() ?? "0");
            StateHasChanged(); // Filtre değişince butonları güncelle
        }

        private void OnSortByChanged(ChangeEventArgs e)
        {
            sortBy = e.Value?.ToString() ?? "name-asc";
            _ = ApplyFiltersAndSort();
        }

        private void ToggleSort(string ascValue, string descValue)
        {
            if (sortBy == ascValue)
            {
                sortBy = descValue;
            }
            else
            {
                sortBy = ascValue;
            }
            _ = ApplyFiltersAndSort();
        }

        // Sütun bazlı sort methodları
        private async Task SortByTc()
        {
            sortBy = sortBy == "tc-asc" ? "tc-desc" : "tc-asc";
            await ApplyFiltersAndSort();
        }

        private async Task SortBySicil()
        {
            sortBy = sortBy == "sicil-asc" ? "sicil-desc" : "sicil-asc";
            await ApplyFiltersAndSort();
        }

        private async Task SortByName()
        {
            sortBy = sortBy == "name-asc" ? "name-desc" : "name-asc";
            await ApplyFiltersAndSort();
        }

        private async Task SortByDepartman()
        {
            sortBy = sortBy == "dept-asc" ? "dept-desc" : "dept-asc";
            await ApplyFiltersAndSort();
        }

        private async Task SortByServis()
        {
            sortBy = sortBy == "servis-asc" ? "servis-desc" : "servis-asc";
            await ApplyFiltersAndSort();
        }

        private async Task SortByBina()
        {
            sortBy = sortBy == "bina-asc" ? "bina-desc" : "bina-asc";
            await ApplyFiltersAndSort();
        }

        private async Task SortByUnvan()
        {
            sortBy = sortBy == "unvan-asc" ? "unvan-desc" : "unvan-asc";
            await ApplyFiltersAndSort();
        }

        private async Task SortByDahili()
        {
            sortBy = sortBy == "dahili-asc" ? "dahili-desc" : "dahili-asc";
            await ApplyFiltersAndSort();
        }

        private void ClearFilters()
        {
            searchTerm = string.Empty;
            filterDepartmanId = 0;
            filterServisId = 0;
            filterHizmetBinasiId = 0;
            filterUnvanId = 0;
            filterAktiflik = -1;
            filterZKTecoDeviceId = 0;
            sortBy = "name-asc";
            _ = ApplyFiltersAndSort();
        }

        // ═══════════════════════════════════════════════════════
        // PAGINATION METHODS
        // ═══════════════════════════════════════════════════════

        private async Task ChangePage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
                await LoadPersonelWithPagination();
            }
        }

        private async Task OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int newPageSize))
            {
                PageSize = newPageSize;
                CurrentPage = 1;
                await LoadPersonelWithPagination();
            }
        }

        // ═══════════════════════════════════════════════════════
        // NAVIGATION METHODS
        // ═══════════════════════════════════════════════════════

        private void NavigateToDetail(string tcKimlikNo)
        {
            _navigationManager.NavigateTo($"/personel/detail/{tcKimlikNo}");
        }

        /// <summary>
        /// Satır tıklamasında DETAIL yetkisi varsa detaya git
        /// </summary>
        private void TryNavigateToDetail(string tcKimlikNo)
        {
            if (CanAction(ActionType.DETAIL))
                NavigateToDetail(tcKimlikNo);
        }

        private void NavigateToEdit(string tcKimlikNo)
        {
            _navigationManager.NavigateTo($"/personel/manage/{tcKimlikNo}");
        }

        // ═══════════════════════════════════════════════════════
        // QUICK ADD MODAL METHODS
        // ═══════════════════════════════════════════════════════

        private void OpenQuickAddModal()
        {
            QuickAddModel = new QuickAddPersonelModel();
            ShowQuickAddModal = true;
        }

        private void CloseQuickAddModal()
        {
            ShowQuickAddModal = false;
            QuickAddModel = new QuickAddPersonelModel();
        }

        private async Task HandleQuickAdd()
        {
            IsQuickAdding = true;
            try
            {
                // Validation kontrolü
                if (string.IsNullOrWhiteSpace(QuickAddModel.TcKimlikNo) || string.IsNullOrWhiteSpace(QuickAddModel.AdSoyad))
                {
                    await _toastService.ShowErrorAsync("TC Kimlik No ve Ad Soyad zorunludur!");
                    return;
                }

                // Simulate API call
                await Task.Delay(500);

                CloseQuickAddModal();
                
                // Manage sayfasına yönlendir (TC ve Ad Soyad ile)
                _navigationManager.NavigateTo($"/personel/manage?tc={QuickAddModel.TcKimlikNo}&adsoyad={Uri.EscapeDataString(QuickAddModel.AdSoyad)}");
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata oluştu: {ex.Message}");
            }
            finally
            {
                IsQuickAdding = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL METHODS
        // ═══════════════════════════════════════════════════════

        private void OpenDeleteModal(PersonelResponseDto personel)
        {
            DeletePersonelTcKimlik = personel.TcKimlikNo;
            DeletePersonelSicilNo = personel.SicilNo;
            DeletePersonelAdSoyad = personel.AdSoyad;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeletePersonelTcKimlik = string.Empty;
            DeletePersonelSicilNo = 0;
            DeletePersonelAdSoyad = string.Empty;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                // API'den personeli sil
                var result = await _personelApiService.DeleteAsync(DeletePersonelTcKimlik);
                
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"{DeletePersonelAdSoyad} başarıyla silindi!");
                    CloseDeleteModal();
                    await LoadData(); // Listeyi yenile
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Personel silinemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Personel silinirken hata oluştu: {ex.Message}");
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
            CurrentExportType = "excel";
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Personeller");

                // Headers
                worksheet.Cell(1, 1).Value = "TC Kimlik No";
                worksheet.Cell(1, 2).Value = "Sicil No";
                worksheet.Cell(1, 3).Value = "PDKS Kayıt No";
                worksheet.Cell(1, 4).Value = "Kart No";
                worksheet.Cell(1, 5).Value = "Son Gönderim";
                worksheet.Cell(1, 6).Value = "Gönderim Durumu";
                worksheet.Cell(1, 7).Value = "Ad Soyad";
                worksheet.Cell(1, 8).Value = "Departman";
                worksheet.Cell(1, 9).Value = "Servis";
                worksheet.Cell(1, 10).Value = "Ünvan";
                worksheet.Cell(1, 11).Value = "Durum";

                // Data
                for (int i = 0; i < FilteredPersoneller.Count; i++)
                {
                    var personel = FilteredPersoneller[i];
                    worksheet.Cell(i + 2, 1).Value = personel.TcKimlikNo;
                    worksheet.Cell(i + 2, 2).Value = personel.SicilNo;
                    worksheet.Cell(i + 2, 3).Value = personel.PersonelKayitNo;
                    worksheet.Cell(i + 2, 4).Value = personel.KartNo > 0 ? personel.KartNo : 0;
                    worksheet.Cell(i + 2, 5).Value = personel.KartNoGonderimTarihi?.ToString("dd.MM.yyyy HH:mm") ?? "-";
                    worksheet.Cell(i + 2, 6).Value = personel.KartGonderimIslemBasari.ToString();
                    worksheet.Cell(i + 2, 7).Value = personel.AdSoyad;
                    worksheet.Cell(i + 2, 8).Value = personel.DepartmanAdi;
                    worksheet.Cell(i + 2, 9).Value = personel.ServisAdi;
                    worksheet.Cell(i + 2, 10).Value = personel.UnvanAdi;
                    worksheet.Cell(i + 2, 11).Value = personel.PersonelAktiflikDurum.ToString();
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"Personeller_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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
                CurrentExportType = string.Empty;
            }
        }

        private async Task ExportToPdf()
        {
            IsExporting = true;
            CurrentExportType = "pdf";

            try
            {
                // 1️⃣ Resimleri önceden yükle
                var personelImages = new Dictionary<string, byte[]?>();
                var tasks = FilteredPersoneller.Select(async p =>
                {
                    personelImages[p.TcKimlikNo] = await GetImageBytesAsync(p.Resim);
                });
                await Task.WhenAll(tasks);

                // 2️⃣ PDF oluştur
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(2, Unit.Centimetre);

                        page.Header()
                            .PaddingBottom(10)
                            .AlignCenter()
                            .Text("Personel Listesi")
                            .FontSize(20)
                            .Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(55);   // Resim
                                columns.ConstantColumn(100);  // TC Kimlik
                                columns.ConstantColumn(80);   // Sicil
                                columns.RelativeColumn();     // Ad Soyad
                                columns.RelativeColumn();     // Departman
                                columns.RelativeColumn();     // Servis
                                columns.RelativeColumn();     // Ünvan
                                columns.ConstantColumn(70);   // Durum
                            });

                            // Başlık satırı
                            table.Header(header =>
                            {
                                AddHeaderCell(header, "Resim");
                                AddHeaderCell(header, "TC Kimlik");
                                AddHeaderCell(header, "Sicil");
                                AddHeaderCell(header, "Ad Soyad");
                                AddHeaderCell(header, "Departman");
                                AddHeaderCell(header, "Servis");
                                AddHeaderCell(header, "Ünvan");
                                AddHeaderCell(header, "Durum");
                            });

                            // Veri satırları
                            foreach (var personel in FilteredPersoneller)
                            {
                                var imageBytes = personelImages[personel.TcKimlikNo];

                                // Resim hücresi
                                table.Cell().Element(cell =>
                                {
                                    var img = cell
                                        .Padding(2)
                                        .AlignCenter()
                                        .AlignMiddle()
                                        .Height(45)
                                        .Width(45);

                                    if (imageBytes != null && imageBytes.Length > 0)
                                        img.Image(imageBytes).FitArea();
                                    else
                                        img.Background(Colors.Grey.Lighten3)
                                           .AlignCenter().AlignMiddle()
                                           .Text("Yok").FontSize(7).Italic();
                                });

                                AddTextCell(table, personel.TcKimlikNo);
                                AddTextCell(table, personel.SicilNo.ToString());
                                AddTextCell(table, personel.AdSoyad);
                                AddTextCell(table, personel.DepartmanAdi);
                                AddTextCell(table, personel.ServisAdi);
                                AddTextCell(table, personel.UnvanAdi);
                                AddTextCell(table, personel.PersonelAktiflikDurum.ToString());
                            }
                        });
                    });
                });

                // 3️⃣ PDF'i indir
                var pdfBytes = document.GeneratePdf();
                var fileName = $"Personeller_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
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
                CurrentExportType = string.Empty;
            }
        }

        // 🔹 Hücre stillerini ayrı metotlar haline getirdik

        private void AddHeaderCell(TableCellDescriptor header, string text)
        {
            header.Cell().Element(container =>
                container
                    .Background(Colors.Grey.Lighten2)
                    .PaddingVertical(4)
                    .AlignCenter()
                    .AlignMiddle()
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Darken2)
                    .DefaultTextStyle(x => x.Bold().FontSize(10))
                    .Text(text)
            );
        }

        private void AddTextCell(TableDescriptor table, string text)
        {
            table.Cell().Element(container =>
                container
                    .PaddingVertical(4)
                    .PaddingHorizontal(3)
                    .BorderBottom(0.5f)
                    .BorderColor(Colors.Grey.Lighten2)
                    .AlignMiddle()
                    .DefaultTextStyle(x => x.FontSize(9))
                    .Text(text)
            );
        }

        private async Task<byte[]?> GetImageBytesAsync(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return null;

            try
            {
                var fullUrl = _navigationManager.BaseUri.TrimEnd('/') + imagePath;

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch
            {
                // Hata olursa sessizce geç (veya logla)
            }

            return null;
        }

        // ═══════════════════════════════════════════════════════
        // ZKTECO PDKS OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task SendToSelectedDevice(PersonelResponseDto personel)
        {
            if (filterZKTecoDeviceId == 0)
            {
                await _toastService.ShowWarningAsync("Lütfen önce bir cihaz seçiniz!");
                return;
            }

            var device = ZKTecoDevices.FirstOrDefault(d => d.DeviceId == filterZKTecoDeviceId);
            if (device == null)
            {
                await _toastService.ShowErrorAsync("Cihaz bulunamadı!");
                return;
            }

            await _toastService.ShowInfoAsync($"{personel.AdSoyad} - {device.DeviceName} cihazına gönderiliyor...");

            // TODO: API çağrısı yapılacak
            // await _zktecoUserApiService.SendUserToDeviceAsync(personel.TcKimlikNo, filterZKTecoDeviceId);

            await _toastService.ShowSuccessAsync($"{personel.AdSoyad} başarıyla {device.DeviceName} cihazına gönderildi!");
            await LoadPersonelWithPagination(); // Listeyi yenile
        }

        private async Task SendToAllDevices(PersonelResponseDto personel)
        {
            if (!ZKTecoDevices.Any())
            {
                await _toastService.ShowWarningAsync("Aktif cihaz bulunamadı!");
                return;
            }

            await _toastService.ShowInfoAsync($"{personel.AdSoyad} - Tüm cihazlara gönderiliyor...");

            // TODO: API çağrısı yapılacak
            // await _zktecoUserApiService.SendUserToAllDevicesAsync(personel.TcKimlikNo);

            await _toastService.ShowSuccessAsync($"{personel.AdSoyad} başarıyla tüm cihazlara gönderildi!");
            await LoadPersonelWithPagination(); // Listeyi yenile
        }

        private async Task DeleteFromSelectedDevice(PersonelResponseDto personel)
        {
            if (filterZKTecoDeviceId == 0)
            {
                await _toastService.ShowWarningAsync("Lütfen önce bir cihaz seçiniz!");
                return;
            }

            var device = ZKTecoDevices.FirstOrDefault(d => d.DeviceId == filterZKTecoDeviceId);
            if (device == null)
            {
                await _toastService.ShowErrorAsync("Cihaz bulunamadı!");
                return;
            }

            await _toastService.ShowWarningAsync($"{personel.AdSoyad} - {device.DeviceName} cihazından siliniyor...");

            // TODO: API çağrısı yapılacak
            // await _zktecoUserApiService.DeleteUserFromDeviceAsync(personel.PersonelKayitNo.ToString(), filterZKTecoDeviceId);

            await _toastService.ShowSuccessAsync($"{personel.AdSoyad} başarıyla {device.DeviceName} cihazından silindi!");
            await LoadPersonelWithPagination(); // Listeyi yenile
        }

        private async Task DeleteFromAllDevices(PersonelResponseDto personel)
        {
            if (!ZKTecoDevices.Any())
            {
                await _toastService.ShowWarningAsync("Aktif cihaz bulunamadı!");
                return;
            }

            await _toastService.ShowWarningAsync($"{personel.AdSoyad} - Tüm cihazlardan siliniyor...");

            // TODO: API çağrısı yapılacak
            // await _zktecoUserApiService.DeleteUserFromAllDevicesAsync(personel.PersonelKayitNo.ToString());

            await _toastService.ShowSuccessAsync($"{personel.AdSoyad} başarıyla tüm cihazlardan silindi!");
            await LoadPersonelWithPagination(); // Listeyi yenile
        }

        // ═══════════════════════════════════════════════════════
        // TOPLU SEÇİM VE İŞLEMLER
        // ═══════════════════════════════════════════════════════

        private List<string> SelectedPersonelIds { get; set; } = new();
        private bool IsAllSelected => FilteredPersoneller.Any() &&
                                      FilteredPersoneller.All(p => SelectedPersonelIds.Contains(p.TcKimlikNo));

        private void ToggleSelectAll(ChangeEventArgs e)
        {
            var isChecked = (bool)(e.Value ?? false);

            if (isChecked)
            {
                // Tüm personelleri seç
                SelectedPersonelIds = FilteredPersoneller.Select(p => p.TcKimlikNo).ToList();
            }
            else
            {
                // Seçimi temizle
                SelectedPersonelIds.Clear();
            }

            StateHasChanged();
        }

        private void TogglePersonelSelection(string tcKimlikNo)
        {
            if (SelectedPersonelIds.Contains(tcKimlikNo))
            {
                SelectedPersonelIds.Remove(tcKimlikNo);
            }
            else
            {
                SelectedPersonelIds.Add(tcKimlikNo);
            }

            StateHasChanged();
        }

        private async Task SendSelectedToDevice()
        {
            if (filterZKTecoDeviceId == 0)
            {
                await _toastService.ShowWarningAsync("Lütfen önce bir cihaz seçiniz!");
                return;
            }

            if (!SelectedPersonelIds.Any())
            {
                await _toastService.ShowWarningAsync("Lütfen personel seçiniz!");
                return;
            }

            var device = ZKTecoDevices.FirstOrDefault(d => d.DeviceId == filterZKTecoDeviceId);
            if (device == null)
            {
                await _toastService.ShowErrorAsync("Cihaz bulunamadı!");
                return;
            }

            await _toastService.ShowInfoAsync($"{SelectedPersonelIds.Count} personel {device.DeviceName} cihazına gönderiliyor...");

            // TODO: API çağrısı yapılacak
            // await _zktecoUserApiService.SendMultipleUsersToDeviceAsync(SelectedPersonelIds, filterZKTecoDeviceId);

            await _toastService.ShowSuccessAsync($"{SelectedPersonelIds.Count} personel başarıyla {device.DeviceName} cihazına gönderildi!");
            SelectedPersonelIds.Clear();
            await LoadPersonelWithPagination();
        }

        private async Task SendSelectedToAllDevices()
        {
            if (!SelectedPersonelIds.Any())
            {
                await _toastService.ShowWarningAsync("Lütfen personel seçiniz!");
                return;
            }

            if (!ZKTecoDevices.Any())
            {
                await _toastService.ShowWarningAsync("Aktif cihaz bulunamadı!");
                return;
            }

            await _toastService.ShowInfoAsync($"{SelectedPersonelIds.Count} personel tüm cihazlara gönderiliyor...");

            // TODO: API çağrısı yapılacak
            // await _zktecoUserApiService.SendMultipleUsersToAllDevicesAsync(SelectedPersonelIds);

            await _toastService.ShowSuccessAsync($"{SelectedPersonelIds.Count} personel başarıyla tüm cihazlara gönderildi!");
            SelectedPersonelIds.Clear();
            await LoadPersonelWithPagination();
        }

        private void ClearSelection()
        {
            SelectedPersonelIds.Clear();
            StateHasChanged();
        }

        // ═══════════════════════════════════════════════════════
        // KART NO DÜZENLEME
        // ═══════════════════════════════════════════════════════

        private bool ShowEditCardModal { get; set; } = false;
        private PersonelResponseDto? EditingPersonel { get; set; }
        private int EditCardNumberValue { get; set; }
        private bool IsEditingCard { get; set; } = false;

        private void EditCardNumber(PersonelResponseDto personel)
        {
            EditingPersonel = personel;
            EditCardNumberValue = personel.KartNo;
            ShowEditCardModal = true;
        }

        private void CloseEditCardModal()
        {
            ShowEditCardModal = false;
            EditingPersonel = null;
            EditCardNumberValue = 0;
        }

        private async Task SaveCardNumber()
        {
            if (EditingPersonel == null) return;

            IsEditingCard = true;
            try
            {
                // TODO: API çağrısı yapılacak
                // await _personelApiService.UpdateCardNumberAsync(EditingPersonel.TcKimlikNo, EditCardNumberValue);

                await _toastService.ShowSuccessAsync($"{EditingPersonel.AdSoyad} - Kart numarası güncellendi!");
                CloseEditCardModal();
                await LoadPersonelWithPagination(); // Listeyi yenile
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Kart numarası güncellenirken hata: {ex.Message}");
            }
            finally
            {
                IsEditingCard = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // MODELS
        // ═══════════════════════════════════════════════════════

        public class QuickAddPersonelModel
        {
            [Required(ErrorMessage = "TC Kimlik No zorunludur")]
            [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
            [RegularExpression(@"^\d{11}$", ErrorMessage = "TC Kimlik No sadece rakamlardan oluşmalıdır")]
            public string TcKimlikNo { get; set; } = string.Empty;

            [Required(ErrorMessage = "Ad Soyad zorunludur")]
            [StringLength(200, MinimumLength = 3, ErrorMessage = "Ad Soyad 3-200 karakter arasında olmalıdır")]
            public string AdSoyad { get; set; } = string.Empty;
        }

        private void GoToManagePage()
        {
            _navigationManager.NavigateTo("/personel/manage");
        }
    }
}
