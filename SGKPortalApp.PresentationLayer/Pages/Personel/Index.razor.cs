using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SGKPortalApp.PresentationLayer.Components.Base;

namespace SGKPortalApp.PresentationLayer.Pages.Personel
{
    public partial class Index : FieldPermissionPageBase
    {
        // ═══════════════════════════════════════════════════════════
        // PERMISSION CONFIGURATION (FieldPermissionPageBase)
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Sayfa permission key: PER.PERSONEL.INDEX
        /// Action permission'lar: PER.PERSONEL.INDEX.ACTION.DETAIL, PER.PERSONEL.INDEX.ACTION.EDIT, vb.
        /// </summary>
        protected override string PagePermissionKey => "PER.PERSONEL.INDEX";
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

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<PersonelResponseDto> Personeller { get; set; } = new();
        private List<PersonelResponseDto> FilteredPersoneller { get; set; } = new();
        
        // Lookup Lists
        private List<DepartmanResponseDto> Departmanlar { get; set; } = new();
        private List<ServisResponseDto> Servisler { get; set; } = new();
        private List<UnvanResponseDto> Unvanlar { get; set; } = new();
        private List<HizmetBinasiResponseDto> HizmetBinalari { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string searchTerm = string.Empty;
        private int filterDepartmanId = 0;
        private int filterServisId = 0;
        private int filterHizmetBinasiId = 0;
        private int filterUnvanId = 0;
        private int filterAktiflik = -1; // -1: Tümü, 0: Aktif, 1: Pasif, 2: Emekli
        private string sortBy = "name-asc";

        // ═══════════════════════════════════════════════════════
        // PAGINATION PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => (int)Math.Ceiling(FilteredPersoneller.Count / (double)PageSize);

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
                // Tüm verileri paralel olarak yükle
                var personelTask = _personelApiService.GetAllAsync();
                var departmanTask = _departmanApiService.GetAllAsync();
                var servisTask = _servisApiService.GetAllAsync();
                var unvanTask = _unvanApiService.GetAllAsync();
                var hizmetBinasiTask = _hizmetBinasiApiService.GetAllAsync();

                await Task.WhenAll(personelTask, departmanTask, servisTask, unvanTask, hizmetBinasiTask);

                var personelResult = await personelTask;
                Personeller = personelResult.Success ? personelResult.Data ?? new List<PersonelResponseDto>() : new List<PersonelResponseDto>();
                
                Departmanlar = (await departmanTask)?.Data ?? new List<DepartmanResponseDto>();
                Servisler = (await servisTask)?.Data ?? new List<ServisResponseDto>();
                Unvanlar = (await unvanTask)?.Data ?? new List<UnvanResponseDto>();
                HizmetBinalari = (await hizmetBinasiTask)?.Data ?? new List<HizmetBinasiResponseDto>();

                // Filtreleri uygula
                ApplyFiltersAndSort();
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

        // ═══════════════════════════════════════════════════════
        // FILTER & SORT METHODS
        // ═══════════════════════════════════════════════════════

        private void ApplyFiltersAndSort()
        {
            FilteredPersoneller = Personeller.Where(p =>
            {
                // TC / Sicil / Ad Soyad arama
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.ToLower();
                    if (!p.TcKimlikNo.Contains(term) &&
                        !p.SicilNo.ToString().Contains(term) &&
                        !p.AdSoyad.ToLower().Contains(term))
                    {
                        return false;
                    }
                }

                // Departman filtresi
                if (filterDepartmanId > 0 && p.DepartmanId != filterDepartmanId)
                    return false;

                // Servis filtresi
                if (filterServisId > 0 && p.ServisId != filterServisId)
                    return false;

                // Ünvan filtresi
                if (filterUnvanId > 0 && p.UnvanId != filterUnvanId)
                    return false;

                // Aktiflik filtresi
                if (filterAktiflik >= 0)
                {
                    var aktiflik = (PersonelAktiflikDurum)filterAktiflik;
                    if (p.PersonelAktiflikDurum != aktiflik)
                        return false;
                }

                return true;
            }).ToList();

            // Sıralama
            FilteredPersoneller = sortBy switch
            {
                "name-asc" => FilteredPersoneller.OrderBy(p => p.AdSoyad).ToList(),
                "name-desc" => FilteredPersoneller.OrderByDescending(p => p.AdSoyad).ToList(),
                "sicil-asc" => FilteredPersoneller.OrderBy(p => p.SicilNo).ToList(),
                "sicil-desc" => FilteredPersoneller.OrderByDescending(p => p.SicilNo).ToList(),
                "date-newest" => FilteredPersoneller.OrderByDescending(p => p.EklenmeTarihi).ToList(),
                "date-oldest" => FilteredPersoneller.OrderBy(p => p.EklenmeTarihi).ToList(),
                _ => FilteredPersoneller.OrderBy(p => p.AdSoyad).ToList()
            };

            CurrentPage = 1;
            StateHasChanged();
        }

        private void OnSearchChanged()
        {
            ApplyFiltersAndSort();
        }

        private void OnFilterDepartmanChanged(ChangeEventArgs e)
        {
            filterDepartmanId = int.Parse(e.Value?.ToString() ?? "0");
            ApplyFiltersAndSort();
        }

        private void OnFilterServisChanged(ChangeEventArgs e)
        {
            filterServisId = int.Parse(e.Value?.ToString() ?? "0");
            ApplyFiltersAndSort();
        }

        private void OnFilterHizmetBinasiChanged(ChangeEventArgs e)
        {
            filterHizmetBinasiId = int.Parse(e.Value?.ToString() ?? "0");
            ApplyFiltersAndSort();
        }

        private void OnFilterUnvanChanged(ChangeEventArgs e)
        {
            filterUnvanId = int.Parse(e.Value?.ToString() ?? "0");
            ApplyFiltersAndSort();
        }

        private void OnFilterAktiflikChanged(ChangeEventArgs e)
        {
            filterAktiflik = int.Parse(e.Value?.ToString() ?? "-1");
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
            filterDepartmanId = 0;
            filterServisId = 0;
            filterHizmetBinasiId = 0;
            filterUnvanId = 0;
            filterAktiflik = -1;
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
                StateHasChanged();
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
                worksheet.Cell(1, 3).Value = "Ad Soyad";
                worksheet.Cell(1, 4).Value = "Departman";
                worksheet.Cell(1, 5).Value = "Servis";
                worksheet.Cell(1, 6).Value = "Ünvan";
                worksheet.Cell(1, 7).Value = "Durum";

                // Data
                for (int i = 0; i < FilteredPersoneller.Count; i++)
                {
                    var personel = FilteredPersoneller[i];
                    worksheet.Cell(i + 2, 1).Value = personel.TcKimlikNo;
                    worksheet.Cell(i + 2, 2).Value = personel.SicilNo;
                    worksheet.Cell(i + 2, 3).Value = personel.AdSoyad;
                    worksheet.Cell(i + 2, 4).Value = personel.DepartmanAdi;
                    worksheet.Cell(i + 2, 5).Value = personel.ServisAdi;
                    worksheet.Cell(i + 2, 6).Value = personel.UnvanAdi;
                    worksheet.Cell(i + 2, 7).Value = personel.PersonelAktiflikDurum.ToString();
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
