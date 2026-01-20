using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin.Takip
{
    public partial class Index
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IIzinMazeretTalepApiService _izinMazeretTalepService { get; set; } = default!;
        [Inject] private IPersonelListApiService _personelListApiService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PERMISSION
        // ═══════════════════════════════════════════════════════

        private string ResolvedPermissionKey => "PDKS.IZIN.TAKIP.INDEX";

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<IzinMazeretTalepListResponseDto> Talepler { get; set; } = new();
        private List<DepartmanDto> DepartmanList { get; set; } = new();
        private List<ServisDto> ServisList { get; set; } = new();
        private List<ServisDto> FilteredServisList { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int FilterDepartmanId { get; set; } = 0;
        private int FilterServisId { get; set; } = 0;
        private string FilterDurum { get; set; } = "";
        private string SearchTerm { get; set; } = "";
        private DateTime? FilterBaslangicMin { get; set; }
        private DateTime? FilterBaslangicMax { get; set; }
        private string FilterIsActive { get; set; } = "";

        private int? userDepartmanId = null;
        private int? userServisId = null;

        // ═══════════════════════════════════════════════════════
        // PAGINATION
        // ═══════════════════════════════════════════════════════

        private int CurrentPage { get; set; } = 1;
        private int PageSize { get; set; } = 50;
        private int TotalCount { get; set; } = 0;
        private int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = false;
        private bool ShowCancelModal { get; set; } = false;
        private bool IsCancelling { get; set; } = false;
        private bool ShowSgkIslemModal { get; set; } = false;
        private bool IsProcessingSgk { get; set; } = false;
        private bool SgkIslemTipi { get; set; } = false; // true=işle, false=geri al
        private string? ErrorMessage { get; set; } = null;
        private IzinMazeretTalepListResponseDto? SelectedTalep { get; set; } = null;
        private string CancelReason { get; set; } = "";
        private string SgkIslemNotlari { get; set; } = "";

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFilterData();
            await LoadTalepler();
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
                    FilterDepartmanId = deptId; // Set user's departman as default filter
                }

                if (int.TryParse(servisIdClaim, out var servId))
                {
                    userServisId = servId;
                    FilterServisId = servId; // Set user's servis as default filter
                }

                // Load departman list
                var deptResult = await _personelListApiService.GetAllDepartmanListeAsync();
                if (deptResult.Success && deptResult.Data != null)
                {
                    DepartmanList = deptResult.Data;
                }

                // Load servis list
                var servResult = await _personelListApiService.GetAllServisListeAsync();
                if (servResult.Success && servResult.Data != null)
                {
                    ServisList = servResult.Data;
                    UpdateFilteredServisList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Filtre verileri yüklenirken hata");
                await ToastService.ShowErrorAsync("Filtre verileri yüklenemedi");
            }
        }

        private async Task LoadTalepler()
        {
            try
            {
                IsLoading = true;
                Talepler.Clear();

                var filter = new IzinMazeretTalepFilterRequestDto
                {
                    DepartmanId = FilterDepartmanId > 0 ? FilterDepartmanId : null,
                    ServisId = FilterServisId > 0 ? FilterServisId : null,
                    BaslangicTarihiMin = FilterBaslangicMin,
                    BaslangicTarihiMax = FilterBaslangicMax,
                    IsActive = string.IsNullOrEmpty(FilterIsActive) ? null : bool.Parse(FilterIsActive),
                    PageNumber = CurrentPage,
                    PageSize = PageSize,
                    SortBy = "taleptarihi",
                    SortDescending = true
                };

                var result = await _izinMazeretTalepService.GetFilteredAsync(filter);

                if (result.Success)
                {
                    Talepler = result.Data.Items;
                    TotalCount = result.Data.TotalCount;

                    // Client-side filtering for search and durum
                    ApplyClientSideFilters();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Talepler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadTalepler exception");
                await ToastService.ShowErrorAsync("Talepler yüklenirken hata oluştu");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // FILTER METHODS
        // ═══════════════════════════════════════════════════════

        private void UpdateFilteredServisList()
        {
            if (FilterDepartmanId > 0)
            {
                FilteredServisList = ServisList.Where(s => s.DepartmanId == FilterDepartmanId).ToList();
            }
            else
            {
                FilteredServisList = ServisList;
            }
        }

        private async Task OnDepartmanChanged()
        {
            // Departman değiştiğinde servisi sıfırla
            FilterServisId = 0;
            UpdateFilteredServisList();
            await ApplyFilters();
        }

        private async Task ApplyFilters()
        {
            CurrentPage = 1; // Reset to first page
            await LoadTalepler();
        }

        private void ApplyClientSideFilters()
        {
            var filteredList = Talepler.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                filteredList = filteredList.Where(t =>
                    t.AdSoyad.ToLower().Contains(searchLower) ||
                    t.SicilNo.ToString().Contains(searchLower) ||
                    t.TcKimlikNo.Contains(searchLower)
                );
            }

            // Durum filter
            if (!string.IsNullOrEmpty(FilterDurum))
            {
                filteredList = filteredList.Where(t => t.GenelDurum == FilterDurum);
            }

            Talepler = filteredList.ToList();
        }

        private async Task OnSearchKeyUp()
        {
            ApplyClientSideFilters();
            await Task.CompletedTask;
        }

        private async Task ClearFilters()
        {
            FilterDepartmanId = userDepartmanId ?? 0;
            FilterServisId = userServisId ?? 0;
            FilterDurum = "";
            SearchTerm = "";
            FilterBaslangicMin = null;
            FilterBaslangicMax = null;
            FilterIsActive = "";
            CurrentPage = 1;

            UpdateFilteredServisList();
            await LoadTalepler();
        }

        // ═══════════════════════════════════════════════════════
        // PAGINATION METHODS
        // ═══════════════════════════════════════════════════════

        private async Task GoToPage(int pageNumber)
        {
            if (pageNumber < 1 || pageNumber > TotalPages || pageNumber == CurrentPage)
                return;

            CurrentPage = pageNumber;
            await LoadTalepler();
        }

        // ═══════════════════════════════════════════════════════
        // CANCEL METHODS
        // ═══════════════════════════════════════════════════════

        private bool CanCancelRequest(IzinMazeretTalepListResponseDto talep)
        {
            // İptal edilmiş veya reddedilmiş talepleri iptal edemez
            if (!talep.IsActive || talep.GenelDurum == "Reddedildi")
                return false;

            // İzin başlangıç tarihi ile talep tarihi arasında en az 3 gün olmalı
            var izinBaslangic = talep.BaslangicTarihi ?? talep.MazeretTarihi;
            if (!izinBaslangic.HasValue)
                return false;

            var daysDifference = (izinBaslangic.Value - talep.TalepTarihi).Days;
            return daysDifference >= 3;
        }

        private void OpenCancelModal(IzinMazeretTalepListResponseDto talep)
        {
            SelectedTalep = talep;
            CancelReason = "";
            ErrorMessage = null;
            ShowCancelModal = true;
        }

        private void CloseCancelModal()
        {
            ShowCancelModal = false;
            SelectedTalep = null;
            CancelReason = "";
            ErrorMessage = null;
        }

        private async Task ConfirmCancel()
        {
            if (SelectedTalep == null)
                return;

            if (string.IsNullOrWhiteSpace(CancelReason))
            {
                ErrorMessage = "İptal nedeni girilmelidir";
                return;
            }

            try
            {
                IsCancelling = true;
                ErrorMessage = null;

                var result = await _izinMazeretTalepService.CancelAsync(SelectedTalep.IzinMazeretTalepId, CancelReason);

                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync("Talep başarıyla iptal edildi");
                    CloseCancelModal();
                    await LoadTalepler();
                }
                else
                {
                    ErrorMessage = result.Message ?? "Talep iptal edilemedi";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConfirmCancel exception");
                ErrorMessage = "İptal işlemi sırasında hata oluştu";
            }
            finally
            {
                IsCancelling = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // NAVIGATION METHODS
        // ═══════════════════════════════════════════════════════

        private void ViewDetails(int talepId)
        {
            _navigationManager.NavigateTo($"/pdks/izin/detay/{talepId}");
        }

        // ═══════════════════════════════════════════════════════
        // UI HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private string GetOnayBadgeClass(string durum)
        {
            return durum switch
            {
                "Beklemede" => "bg-label-warning",
                "Onaylandı" => "bg-label-success",
                "Reddedildi" => "bg-label-danger",
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
                _ => "bg-label-warning"
            };
        }

        // ═══════════════════════════════════════════════════════
        // SGK İŞLEM METHODS
        // ═══════════════════════════════════════════════════════

        private void OpenSgkIslemModal(IzinMazeretTalepListResponseDto talep, bool islemTipi)
        {
            SelectedTalep = talep;
            SgkIslemTipi = islemTipi;
            SgkIslemNotlari = "";
            ErrorMessage = null;
            ShowSgkIslemModal = true;
        }

        private void CloseSgkIslemModal()
        {
            ShowSgkIslemModal = false;
            SelectedTalep = null;
            SgkIslemNotlari = "";
            ErrorMessage = null;
        }

        private async Task ConfirmSgkIslem()
        {
            if (SelectedTalep == null)
                return;

            try
            {
                IsProcessingSgk = true;
                ErrorMessage = null;

                var request = new IzinSgkIslemRequestDto
                {
                    IzinMazeretTalepId = SelectedTalep.IzinMazeretTalepId,
                    Isle = SgkIslemTipi,
                    Notlar = SgkIslemNotlari
                };

                var result = await _izinMazeretTalepService.ProcessSgkIslemAsync(request);

                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync(result.Message ?? "İşlem başarılı");
                    CloseSgkIslemModal();
                    await LoadTalepler();
                }
                else
                {
                    ErrorMessage = result.Message ?? "İşlem başarısız";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConfirmSgkIslem exception");
                ErrorMessage = "İşlem sırasında hata oluştu";
            }
            finally
            {
                IsProcessingSgk = false;
            }
        }
    }
}
