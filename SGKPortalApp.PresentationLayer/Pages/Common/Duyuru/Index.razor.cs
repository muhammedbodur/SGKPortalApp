using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.Duyuru
{
    public partial class Index
    {
        [Inject] private IDuyuruApiService _duyuruService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private List<DuyuruResponseDto> Duyurular { get; set; } = new();
        private List<DuyuruResponseDto> FilteredDuyurular { get; set; } = new();

        private string searchTerm = string.Empty;
        private string statusFilter = string.Empty;
        private string sortBy = "date-newest";

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private int TotalPages => (int)Math.Ceiling(FilteredDuyurular.Count / (double)PageSize);

        private bool IsLoading { get; set; } = true;
        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = string.Empty;

        private bool ShowDeleteModal { get; set; } = false;
        private int DeleteDuyuruId { get; set; }
        private string DeleteDuyuruBaslik { get; set; } = string.Empty;
        private bool IsDeleting { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadDuyurular();
        }

        private async Task LoadDuyurular()
        {
            IsLoading = true;
            try
            {
                var result = await _duyuruService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    Duyurular = result.Data;
                    ApplyFiltersAndSort();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Duyurular yüklenemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFiltersAndSort()
        {
            var filtered = Duyurular.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.Where(d =>
                    d.Baslik.ContainsTurkish(searchTerm) ||
                    (d.Icerik?.ContainsTurkish(searchTerm) ?? false));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                filtered = statusFilter switch
                {
                    "aktif" => filtered.Where(d => d.Aktiflik == Aktiflik.Aktif),
                    "pasif" => filtered.Where(d => d.Aktiflik == Aktiflik.Pasif),
                    _ => filtered
                };
            }

            filtered = sortBy switch
            {
                "date-newest" => filtered.OrderByDescending(d => d.YayinTarihi),
                "date-oldest" => filtered.OrderBy(d => d.YayinTarihi),
                "sira-asc" => filtered.OrderBy(d => d.Sira),
                "sira-desc" => filtered.OrderByDescending(d => d.Sira),
                "baslik-asc" => filtered.OrderBy(d => d.Baslik),
                _ => filtered.OrderByDescending(d => d.YayinTarihi)
            };

            FilteredDuyurular = filtered.ToList();
            CurrentPage = 1;
        }

        private void OnSearchChanged()
        {
            ApplyFiltersAndSort();
        }

        private void OnStatusFilterChanged(ChangeEventArgs e)
        {
            statusFilter = e.Value?.ToString() ?? string.Empty;
            ApplyFiltersAndSort();
        }

        private void OnSortByChanged(ChangeEventArgs e)
        {
            sortBy = e.Value?.ToString() ?? "date-newest";
            ApplyFiltersAndSort();
        }

        private void ClearFilters()
        {
            searchTerm = string.Empty;
            statusFilter = string.Empty;
            sortBy = "date-newest";
            ApplyFiltersAndSort();
        }

        private void ChangePage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
            }
        }

        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/common/duyuru/manage");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/common/duyuru/manage/{id}");
        }

        private void ShowDeleteConfirmation(int id, string baslik)
        {
            DeleteDuyuruId = id;
            DeleteDuyuruBaslik = baslik;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            DeleteDuyuruId = 0;
            DeleteDuyuruBaslik = string.Empty;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _duyuruService.DeleteAsync(DeleteDuyuruId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Duyuru başarıyla silindi!");
                    CloseDeleteModal();
                    await LoadDuyurular();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Duyuru silinemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsDeleting = false;
            }
        }

        private async Task ExportToExcel()
        {
            IsExporting = true;
            ExportType = "excel";

            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Duyurular");

                worksheet.Cell(1, 1).Value = "Başlık";
                worksheet.Cell(1, 2).Value = "İçerik";
                worksheet.Cell(1, 3).Value = "Sıra";
                worksheet.Cell(1, 4).Value = "Durum";
                worksheet.Cell(1, 5).Value = "Yayın Tarihi";
                worksheet.Cell(1, 6).Value = "Bitiş Tarihi";
                worksheet.Cell(1, 7).Value = "Görsel";

                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                int row = 2;
                foreach (var duyuru in FilteredDuyurular)
                {
                    worksheet.Cell(row, 1).Value = duyuru.Baslik;
                    worksheet.Cell(row, 2).Value = duyuru.Icerik;
                    worksheet.Cell(row, 3).Value = duyuru.Sira;
                    worksheet.Cell(row, 4).Value = duyuru.Aktiflik == Aktiflik.Aktif ? "Aktif" : "Pasif";
                    worksheet.Cell(row, 5).Value = duyuru.YayinTarihi.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 6).Value = duyuru.BitisTarihi?.ToString("dd.MM.yyyy") ?? "-";
                    worksheet.Cell(row, 7).Value = string.IsNullOrEmpty(duyuru.GorselUrl) ? "Yok" : "Var";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"Duyurular_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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
                ExportType = string.Empty;
            }
        }
    }
}
