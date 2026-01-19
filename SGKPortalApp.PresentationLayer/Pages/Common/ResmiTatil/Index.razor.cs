using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
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
        [Inject] private IPersonelMesaiApiService _mesaiService { get; set; } = default!;
        [Inject] private IIzinMazeretTalepApiService _izinMazeretService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string? CurrentUserTc { get; set; }
        private List<ResmiTatilResponseDto> Tatiller { get; set; } = new();
        private List<PersonelMesaiListResponseDto> MesaiKayitlari { get; set; } = new();
        private List<IzinMazeretTalepListResponseDto> IzinMazeretTalepleri { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int _selectedYear = DateTime.Now.Year;
        private int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (_selectedYear != value)
                {
                    _selectedYear = value;
                    _ = OnYearChanged();
                }
            }
        }

        private bool _filterSabitTatil = true;
        private bool FilterSabitTatil
        {
            get => _filterSabitTatil;
            set
            {
                if (_filterSabitTatil != value)
                {
                    _filterSabitTatil = value;
                    _ = OnFilterChanged();
                }
            }
        }

        private bool _filterDiniTatil = true;
        private bool FilterDiniTatil
        {
            get => _filterDiniTatil;
            set
            {
                if (_filterDiniTatil != value)
                {
                    _filterDiniTatil = value;
                    _ = OnFilterChanged();
                }
            }
        }

        private bool _filterOzelTatil = true;
        private bool FilterOzelTatil
        {
            get => _filterOzelTatil;
            set
            {
                if (_filterOzelTatil != value)
                {
                    _filterOzelTatil = value;
                    _ = OnFilterChanged();
                }
            }
        }

        private bool _filterMesai = true;
        private bool FilterMesai
        {
            get => _filterMesai;
            set
            {
                if (_filterMesai != value)
                {
                    _filterMesai = value;
                    _ = OnFilterChanged();
                }
            }
        }

        private bool _filterIzinMazeret = true;
        private bool FilterIzinMazeret
        {
            get => _filterIzinMazeret;
            set
            {
                if (_filterIzinMazeret != value)
                {
                    _filterIzinMazeret = value;
                    _ = OnFilterChanged();
                }
            }
        }

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
            await base.OnInitializedAsync();

            // Get current user's TC
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            CurrentUserTc = authState.User.FindFirst("TcKimlikNo")?.Value;

            await LoadAllData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // İlk render'da calendar'ı başlatmaya gerek yok
            // LoadTatiller() içinde zaten RenderCalendar() çağrılıyor
        }

        private async Task LoadAllData()
        {
            IsLoading = true;
            StateHasChanged();

            try
            {
                // Load all data sources in parallel
                var tatillerTask = LoadTatiller();
                var mesaiTask = LoadMesaiKayitlari();
                var izinMazeretTask = LoadIzinMazeretTalepleri();

                await Task.WhenAll(tatillerTask, mesaiTask, izinMazeretTask);
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veriler yüklenirken hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();

                await Task.Delay(100);
                await RenderCalendar();
            }
        }

        private async Task LoadTatiller()
        {
            try
            {
                var result = await _resmiTatilService.GetByYearAsync(SelectedYear);

                if (result.Success && result.Data != null)
                {
                    Tatiller = result.Data;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Tatiller yüklenirken hata");
            }
        }

        private async Task LoadMesaiKayitlari()
        {
            if (string.IsNullOrEmpty(CurrentUserTc))
            {
                MesaiKayitlari.Clear();
                return;
            }

            try
            {
                var request = new PersonelMesaiFilterRequestDto
                {
                    TcKimlikNo = CurrentUserTc,
                    BaslangicTarihi = new DateTime(SelectedYear, 1, 1),
                    BitisTarihi = new DateTime(SelectedYear, 12, 31)
                };

                var result = await _mesaiService.GetListeAsync(request);

                if (result.Success && result.Data != null)
                {
                    MesaiKayitlari = result.Data;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Mesai kayıtları yüklenirken hata");
            }
        }

        private async Task LoadIzinMazeretTalepleri()
        {
            if (string.IsNullOrEmpty(CurrentUserTc))
            {
                IzinMazeretTalepleri.Clear();
                return;
            }

            try
            {
                var result = await _izinMazeretService.GetByPersonelAsync(CurrentUserTc);

                if (result.Success && result.Data != null)
                {
                    // Filter by year
                    IzinMazeretTalepleri = result.Data.Where(t =>
                    {
                        // Check if talep falls in selected year
                        if (t.BaslangicTarihi.HasValue)
                        {
                            return t.BaslangicTarihi.Value.Year == SelectedYear ||
                                   (t.BitisTarihi.HasValue && t.BitisTarihi.Value.Year == SelectedYear);
                        }
                        else if (t.MazeretTarihi.HasValue)
                        {
                            return t.MazeretTarihi.Value.Year == SelectedYear;
                        }
                        return false;
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "İzin/Mazeret talepleri yüklenirken hata");
            }
        }

        // ═══════════════════════════════════════════════════════
        // FILTER METHODS
        // ═══════════════════════════════════════════════════════

        private async Task OnYearChanged()
        {
            await LoadAllData();
        }

        private async Task OnFilterChanged()
        {
            StateHasChanged();
            await Task.Delay(50);
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
                var events = new List<object>();

                // 1. Resmi Tatiller
                if (FilterSabitTatil || FilterDiniTatil || FilterOzelTatil)
                {
                    var tatilEvents = Tatiller
                        .Where(t =>
                        {
                            if (t.TatilTipi == TatilTipi.SabitTatil && !FilterSabitTatil) return false;
                            if (t.TatilTipi == TatilTipi.DiniTatil && !FilterDiniTatil) return false;
                            if (t.TatilTipi == TatilTipi.OzelTatil && !FilterOzelTatil) return false;
                            return true;
                        })
                        .Select(t => new
                        {
                            id = $"tatil-{t.TatilId}",
                            title = t.TatilAdi,
                            start = t.Tarih.ToString("yyyy-MM-dd"),
                            allDay = true,
                            backgroundColor = GetTatilColor(t.TatilTipi),
                            borderColor = GetTatilColor(t.TatilTipi),
                            extendedProps = new
                            {
                                eventType = "tatil",
                                tatilTipi = t.TatilTipiText,
                                aciklama = t.Aciklama
                            }
                        });
                    events.AddRange(tatilEvents);
                }

                // 2. Mesai Kayıtları (Giriş/Çıkış)
                if (FilterMesai)
                {
                    var mesaiEvents = MesaiKayitlari
                        .Where(m => m.GirisSaati.HasValue || m.CikisSaati.HasValue)
                        .Select(m => new
                        {
                            id = $"mesai-{m.Tarih:yyyyMMdd}",
                            title = $"🕒 {(m.GirisSaati?.ToString(@"hh\:mm") ?? "?")} - {(m.CikisSaati?.ToString(@"hh\:mm") ?? "?")}",
                            start = m.Tarih.ToString("yyyy-MM-dd"),
                            allDay = false,
                            backgroundColor = GetMesaiColor(m),
                            borderColor = GetMesaiColor(m),
                            extendedProps = new
                            {
                                eventType = "mesai",
                                girisSaati = m.GirisSaati?.ToString(@"hh\:mm"),
                                cikisSaati = m.CikisSaati?.ToString(@"hh\:mm"),
                                mesaiSuresi = m.MesaiSuresi,
                                detay = m.Detay,
                                gecKalma = m.GecKalma
                            }
                        });
                    events.AddRange(mesaiEvents);
                }

                // 3. İzin/Mazeret Talepleri
                if (FilterIzinMazeret)
                {
                    var izinMazeretEvents = new List<object>();

                    foreach (var talep in IzinMazeretTalepleri)
                    {
                        if (talep.BaslangicTarihi.HasValue && talep.BitisTarihi.HasValue)
                        {
                            // İzin (date range)
                            izinMazeretEvents.Add(new
                            {
                                id = $"izin-{talep.IzinMazeretTalepId}",
                                title = $"📅 {talep.TuruAdi}",
                                start = talep.BaslangicTarihi.Value.ToString("yyyy-MM-dd"),
                                end = talep.BitisTarihi.Value.AddDays(1).ToString("yyyy-MM-dd"), // FullCalendar end is exclusive
                                allDay = true,
                                backgroundColor = GetIzinMazeretColor(talep),
                                borderColor = GetIzinMazeretColor(talep),
                                extendedProps = new
                                {
                                    eventType = "izin",
                                    tur = talep.TuruAdi,
                                    onayDurumu = talep.BirinciOnayDurumuAdi
                                }
                            });
                        }
                        else if (talep.MazeretTarihi.HasValue)
                        {
                            // Mazeret (single day)
                            izinMazeretEvents.Add(new
                            {
                                id = $"mazeret-{talep.IzinMazeretTalepId}",
                                title = $"⏰ {talep.TuruAdi}",
                                start = talep.MazeretTarihi.Value.ToString("yyyy-MM-dd"),
                                allDay = false,
                                backgroundColor = GetIzinMazeretColor(talep),
                                borderColor = GetIzinMazeretColor(talep),
                                extendedProps = new
                                {
                                    eventType = "mazeret",
                                    tur = talep.TuruAdi,
                                    saatDilimi = talep.SaatDilimi,
                                    onayDurumu = talep.BirinciOnayDurumuAdi
                                }
                            });
                        }
                    }
                    events.AddRange(izinMazeretEvents);
                }

                var eventsJson = JsonSerializer.Serialize(events);
                await JSRuntime.InvokeVoidAsync("initResmiTatilCalendar", eventsJson, SelectedYear);
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Takvim yüklenirken hata: {ex.Message}");
            }
        }

        private string GetTatilColor(TatilTipi tatilTipi)
        {
            return tatilTipi switch
            {
                TatilTipi.SabitTatil => "#696cff", // Primary blue
                TatilTipi.DiniTatil => "#71dd37",  // Success green
                TatilTipi.OzelTatil => "#ffab00",  // Warning orange
                _ => "#8592a3"
            };
        }

        private string GetMesaiColor(PersonelMesaiListResponseDto mesai)
        {
            // Geç kalma -> kırmızı
            if (mesai.GecKalma) return "#ff3e1d";

            // Hafta sonu -> açık mavi
            if (mesai.HaftaSonu) return "#03c3ec";

            // Normal mesai -> info blue
            return "#00cfe8";
        }

        private string GetIzinMazeretColor(IzinMazeretTalepListResponseDto talep)
        {
            // Onay durumuna göre renk
            return talep.BirinciOnayDurumu switch
            {
                BusinessObjectLayer.Enums.PdksIslemleri.OnayDurumu.Onaylandi => "#71dd37", // Green
                BusinessObjectLayer.Enums.PdksIslemleri.OnayDurumu.Reddedildi => "#ff3e1d", // Red
                _ => "#ffab00" // Orange (Beklemede)
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
                        await LoadAllData();
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
                    await LoadAllData();
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
