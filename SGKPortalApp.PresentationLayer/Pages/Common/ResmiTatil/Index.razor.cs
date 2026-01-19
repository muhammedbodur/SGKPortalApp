using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.Text.Json;

namespace SGKPortalApp.PresentationLayer.Pages.Common.ResmiTatil
{
    public partial class Index
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IResmiTatilApiService _resmiTatilService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<ResmiTatilResponseDto> Tatiller { get; set; } = new();
        private List<ResmiTatilResponseDto> FilteredTatiller { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int SelectedYear { get; set; } = DateTime.Now.Year;
        private bool FilterSabitTatil { get; set; } = true;
        private bool FilterDiniTatil { get; set; } = true;
        private bool FilterOzelTatil { get; set; } = true;

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = true;
        private bool IsSyncing { get; set; } = false;
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // SYNC MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowSyncModalFlag { get; set; } = false;
        private int SyncYear { get; set; } = DateTime.Now.Year;
        private bool DeleteExistingHolidays { get; set; } = true;

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool ShowDeleteModalFlag { get; set; } = false;
        private int DeleteTatilId { get; set; }
        private string DeleteTatilAdi { get; set; } = string.Empty;
        private DateTime DeleteTatilTarih { get; set; }

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await LoadTatiller();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !IsLoading)
            {
                await RenderCalendar();
            }
        }

        private async Task LoadTatiller()
        {
            IsLoading = true;
            try
            {
                var result = await _resmiTatilService.GetByYearAsync(SelectedYear);

                if (result.Success && result.Data != null)
                {
                    Tatiller = result.Data;
                    ApplyFilters();
                    await RenderCalendar();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Tatiller yüklenemedi!");
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

        // ═══════════════════════════════════════════════════════
        // FILTER METHODS
        // ═══════════════════════════════════════════════════════

        private void ApplyFilters()
        {
            FilteredTatiller = Tatiller.Where(t =>
            {
                if (t.TatilTipi == TatilTipi.SabitTatil && !FilterSabitTatil) return false;
                if (t.TatilTipi == TatilTipi.DiniTatil && !FilterDiniTatil) return false;
                if (t.TatilTipi == TatilTipi.OzelTatil && !FilterOzelTatil) return false;
                return true;
            }).ToList();
        }

        private async Task OnYearChanged()
        {
            await LoadTatiller();
        }

        private async Task OnFilterChanged()
        {
            ApplyFilters();
            await RenderCalendar();
        }

        // ═══════════════════════════════════════════════════════
        // CALENDAR RENDERING
        // ═══════════════════════════════════════════════════════

        private async Task RenderCalendar()
        {
            if (IsLoading) return;

            try
            {
                var events = FilteredTatiller.Select(t => new
                {
                    id = t.TatilId,
                    title = t.TatilAdi,
                    start = t.Tarih.ToString("yyyy-MM-dd"),
                    allDay = true,
                    backgroundColor = GetEventColor(t.TatilTipi),
                    borderColor = GetEventColor(t.TatilTipi),
                    extendedProps = new
                    {
                        tatilTipi = t.TatilTipiText,
                        aciklama = t.Aciklama,
                        yariGun = t.YariGun,
                        otomatikSenkronize = t.OtomatikSenkronize
                    }
                }).ToList();

                var eventsJson = JsonSerializer.Serialize(events);
                await JSRuntime.InvokeVoidAsync("initResmiTatilCalendar", eventsJson, SelectedYear);
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Takvim yüklenirken hata: {ex.Message}");
            }
        }

        private string GetEventColor(TatilTipi tatilTipi)
        {
            return tatilTipi switch
            {
                TatilTipi.SabitTatil => "#696cff",
                TatilTipi.DiniTatil => "#71dd37",
                TatilTipi.OzelTatil => "#ffab00",
                _ => "#8592a3"
            };
        }

        // ═══════════════════════════════════════════════════════
        // NAVIGATION METHODS
        // ═══════════════════════════════════════════════════════

        private void NavigateToCreate()
        {
            _navigationManager.NavigateTo("/common/resmitatil/manage");
        }

        private void NavigateToEdit(int id)
        {
            _navigationManager.NavigateTo($"/common/resmitatil/manage/{id}");
        }

        // ═══════════════════════════════════════════════════════
        // SYNC METHODS
        // ═══════════════════════════════════════════════════════

        private void ShowSyncModal()
        {
            SyncYear = SelectedYear;
            DeleteExistingHolidays = true;
            ShowSyncModalFlag = true;
        }

        private void CloseSyncModal()
        {
            ShowSyncModalFlag = false;
        }

        private async Task SyncHolidays()
        {
            IsSyncing = true;
            try
            {
                var request = new ResmiTatilSyncRequestDto
                {
                    Yil = SyncYear,
                    MevcutlariSil = DeleteExistingHolidays
                };

                var result = await _resmiTatilService.SyncHolidaysFromNagerDateAsync(request);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"{result.Data} tatil başarıyla senkronize edildi!");
                    CloseSyncModal();
                    
                    if (SyncYear == SelectedYear)
                    {
                        await LoadTatiller();
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Senkronizasyon başarısız!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsSyncing = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // DELETE METHODS
        // ═══════════════════════════════════════════════════════

        private void ShowDeleteModal(int id, string tatilAdi, DateTime tarih)
        {
            DeleteTatilId = id;
            DeleteTatilAdi = tatilAdi;
            DeleteTatilTarih = tarih;
            ShowDeleteModalFlag = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModalFlag = false;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _resmiTatilService.DeleteAsync(DeleteTatilId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Tatil başarıyla silindi!");
                    CloseDeleteModal();
                    await LoadTatiller();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Tatil silinemedi!");
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

        // ═══════════════════════════════════════════════════════
        // JS INTEROP CALLBACKS
        // ═══════════════════════════════════════════════════════

        [JSInvokable]
        public void OnEventClick(int eventId)
        {
            NavigateToEdit(eventId);
        }

        [JSInvokable]
        public void OnEventDelete(int eventId)
        {
            var tatil = Tatiller.FirstOrDefault(t => t.TatilId == eventId);
            if (tatil != null)
            {
                ShowDeleteModal(tatil.TatilId, tatil.TatilAdi, tatil.Tarih);
                StateHasChanged();
            }
        }
    }
}
