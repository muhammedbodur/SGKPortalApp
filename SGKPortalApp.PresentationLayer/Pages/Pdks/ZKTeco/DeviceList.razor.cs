using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class DeviceList
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IZKTecoDeviceApiService DeviceApiService { get; set; } = default!;
        [Inject] private IDepartmanApiService DepartmanApiService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService HizmetBinasiApiService { get; set; } = default!;
        [Inject] private IPersonelApiService PersonelApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<DeviceResponseDto>? devices;
        private bool showAddForm = false;
        private DeviceResponseDto newDevice = new DeviceResponseDto { Port = "4370", IsActive = true };
        
        // Loading states for operations
        private Dictionary<int, bool> loadingStates = new Dictionary<int, bool>();

        // Departman ve Hizmet Binası dropdown için
        private List<DepartmanResponseDto> departmanlar = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private int selectedDepartmanId = 0;

        // Personel Gönderme Modal
        private bool showSendPersonelModal = false;
        private int selectedDeviceId = 0;
        private string selectedDeviceName = "";
        private bool isLoadingPersonel = false;
        private bool isSendingPersonel = false;
        private List<PersonelDto> personelList = new List<PersonelDto>();
        private List<PersonelDto> filteredPersonelList = new List<PersonelDto>();
        private List<string> selectedPersonelIds = new List<string>();
        
        private string _personelSearchTerm = "";
        private string personelSearchTerm
        {
            get => _personelSearchTerm;
            set
            {
                _personelSearchTerm = value;
                FilterPersonel();
            }
        }
        
        // Cihaz Personel Listesi Modal
        private bool showDevicePersonelModal = false;
        private List<DeviceUserMatch> devicePersonelList = new List<DeviceUserMatch>();
        private List<DeviceUserMatch> filteredDevicePersonelList = new List<DeviceUserMatch>();
        
        private string _devicePersonelSearchTerm = "";
        private string devicePersonelSearchTerm
        {
            get => _devicePersonelSearchTerm;
            set
            {
                _devicePersonelSearchTerm = value;
                FilterDevicePersonel();
            }
        }
        
        private bool isLoadingDevicePersonel = false;
        
        // Cihaz Saati Modal
        private bool showDeviceTimeModal = false;
        private DeviceTimeDto? selectedDeviceTimeInfo = null;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDepartmanlar();
            await LoadDevices();
        }

        // ═══════════════════════════════════════════════════════
        // DEVICE OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task LoadDevices()
        {
            try
            {
                var result = await DeviceApiService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    devices = result.Data;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihazlar yüklenemedi");
                    devices = new List<DeviceResponseDto>();
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
                devices = new List<DeviceResponseDto>();
            }
        }

        private async Task LoadDepartmanlar()
        {
            try
            {
                var result = await DepartmanApiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    departmanlar = result.Data;
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Departmanlar yüklenemedi: {ex.Message}");
            }
        }

        private async Task OnDepartmanChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var departmanId))
            {
                selectedDepartmanId = departmanId;
                await LoadHizmetBinalari(departmanId);
                newDevice.HizmetBinasiId = 0; // Hizmet binası seçimini sıfırla
            }
        }

        private async Task LoadHizmetBinalari(int departmanId)
        {
            try
            {
                var result = await HizmetBinasiApiService.GetByDepartmanAsync(departmanId);
                if (result.Success && result.Data != null)
                {
                    hizmetBinalari = result.Data;
                }
                else
                {
                    hizmetBinalari = new();
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hizmet binaları yüklenemedi: {ex.Message}");
                hizmetBinalari = new();
            }
        }

        private void ToggleAddForm()
        {
            showAddForm = !showAddForm;
            if (showAddForm)
            {
                newDevice = new DeviceResponseDto { Port = "4370", IsActive = true };
                selectedDepartmanId = 0;
                hizmetBinalari = new();
            }
        }

        private async Task SaveDevice()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(newDevice.DeviceName))
            {
                await ToastService.ShowWarningAsync("Cihaz adı zorunludur!");
                return;
            }

            if (string.IsNullOrWhiteSpace(newDevice.IpAddress))
            {
                await ToastService.ShowWarningAsync("IP adresi zorunludur!");
                return;
            }

            if (newDevice.HizmetBinasiId == 0)
            {
                await ToastService.ShowWarningAsync("Hizmet binası seçimi zorunludur!");
                return;
            }

            try
            {
                var result = await DeviceApiService.CreateAsync(newDevice);
                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync("Cihaz başarıyla eklendi!");
                    showAddForm = false;
                    await LoadDevices();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz eklenemedi!");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task TestConnection(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;
            
            SetDeviceLoading(deviceId, true);
            try
            {
                var result = await DeviceApiService.TestConnectionAsync(deviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync(result.Message ?? "Bağlantı başarılı!");
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Bağlantı başarısız! Cihaza erişilemiyor.");
                }
                await LoadDevices();
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Bağlantı testi hatası: {ex.Message}");
            }
            finally
            {
                SetDeviceLoading(deviceId, false);
            }
        }

        private async Task GetStatus(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;
            
            SetDeviceLoading(deviceId, true);
            try
            {
                var result = await DeviceApiService.GetStatusAsync(deviceId);
                if (result.Success && result.Data != null)
                {
                    var status = result.Data;
                    var message = $"📊 Cihaz Durum Bilgisi:\n\n" +
                                 $"Firmware: {status.FirmwareVersion}\n" +
                                 $"Seri No: {status.SerialNumber}\n" +
                                 $"Platform: {status.Platform}\n" +
                                 $"Kullanıcı: {status.UserCount} / {status.UserCapacity}\n" +
                                 $"Kayıt: {status.AttendanceLogCount} / {status.AttLogCapacity}\n" +
                                 $"Parmak İzi: {status.FingerPrintCount} / {status.FingerPrintCapacity}";
                    await JS.InvokeVoidAsync("alert", message);
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz durumu alınamadı! Cihaza erişilemiyor.");
                }
            }
            finally
            {
                SetDeviceLoading(deviceId, false);
            }
        }

        private async Task GetDeviceTime(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;
            
            SetDeviceLoading(deviceId, true);
            try
            {
                var result = await DeviceApiService.GetDeviceTimeAsync(deviceId);
                if (result.Success && result.Data != null)
                {
                    selectedDeviceTimeInfo = result.Data;
                    var device = devices?.FirstOrDefault(d => d.DeviceId == deviceId);
                    selectedDeviceName = device?.DeviceName ?? "Cihaz";
                    showDeviceTimeModal = true;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz saati alınamadı! Cihaza erişilemiyor.");
                }
            }
            finally
            {
                SetDeviceLoading(deviceId, false);
                StateHasChanged();
            }
        }

        private async Task SyncDeviceTime(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;
            
            if (await JS.InvokeAsync<bool>("confirm", "Cihaz saatini şu anki sunucu saatiyle senkronize etmek istediğinize emin misiniz?"))
            {
                SetDeviceLoading(deviceId, true);
                try
                {
                    var result = await DeviceApiService.SynchronizeDeviceTimeAsync(deviceId);
                    if (result.Success && result.Data)
                    {
                        await ToastService.ShowSuccessAsync(result.Message ?? "Cihaz saati senkronize edildi!");
                    }
                    else
                    {
                        await ToastService.ShowErrorAsync(result.Message ?? "Cihaz saati senkronize edilemedi! Cihaza erişilemiyor.");
                    }
                }
                catch (Exception ex)
                {
                    await ToastService.ShowErrorAsync($"Saat senkronizasyon hatası: {ex.Message}");
                }
                finally
                {
                    SetDeviceLoading(deviceId, false);
                }
            }
        }

        private async Task ShowDeviceUsers(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;
            
            var device = devices?.FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null) return;

            selectedDeviceId = deviceId;
            selectedDeviceName = device.DeviceName ?? "";
            showDevicePersonelModal = true;
            isLoadingDevicePersonel = true;
            StateHasChanged();
            
            SetDeviceLoading(deviceId, true);
            try
            {
                // Business layer'daki profesyonel uyumsuzluk tespit sistemini kullan
                var result = await DeviceApiService.GetDeviceUsersWithMismatchesAsync(deviceId);
                if (result.Success && result.Data != null)
                {
                    devicePersonelList = result.Data;
                    filteredDevicePersonelList = result.Data;
                    
                    if (!result.Data.Any())
                    {
                        await ToastService.ShowInfoAsync("Cihazda kayıtlı personel bulunamadı.");
                    }
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaza erişilemiyor! Personel listesi alınamadı.");
                    devicePersonelList = new List<DeviceUserMatch>();
                    filteredDevicePersonelList = new List<DeviceUserMatch>();
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Personel listesi alınamadı: {ex.Message}");
                devicePersonelList = new List<DeviceUserMatch>();
                filteredDevicePersonelList = new List<DeviceUserMatch>();
            }
            finally
            {
                isLoadingDevicePersonel = false;
                SetDeviceLoading(deviceId, false);
                StateHasChanged();
            }
        }

        private async Task EnableDevice(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;
            
            SetDeviceLoading(deviceId, true);
            try
            {
                var result = await DeviceApiService.EnableDeviceAsync(deviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync(result.Message ?? "Cihaz etkinleştirildi!");
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz etkinleştirilemedi! Cihaza erişilemiyor.");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Cihaz etkinleştirme hatası: {ex.Message}");
            }
            finally
            {
                SetDeviceLoading(deviceId, false);
            }
        }

        private async Task DisableDevice(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;
            
            if (await JS.InvokeAsync<bool>("confirm", "⚠️ Cihazı devre dışı bırakmak istediğinize emin misiniz?\n\nKullanıcılar kart geçemez."))
            {
                SetDeviceLoading(deviceId, true);
                try
                {
                    var result = await DeviceApiService.DisableDeviceAsync(deviceId);
                    if (result.Success && result.Data)
                    {
                        await ToastService.ShowSuccessAsync(result.Message ?? "Cihaz devre dışı bırakıldı!");
                    }
                    else
                    {
                        await ToastService.ShowErrorAsync(result.Message ?? "Cihaz devre dışı bırakılamadı! Cihaza erişilemiyor.");
                    }
                }
                catch (Exception ex)
                {
                    await ToastService.ShowErrorAsync($"Cihaz devre dışı bırakma hatası: {ex.Message}");
                }
                finally
                {
                    SetDeviceLoading(deviceId, false);
                }
            }
        }

        private async Task RestartDevice(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;
            
            if (await JS.InvokeAsync<bool>("confirm", "⚠️ Cihazı yeniden başlatmak istediğinize emin misiniz?\n\nCihaz yaklaşık 30 saniye offline olacak."))
            {
                SetDeviceLoading(deviceId, true);
                try
                {
                    var result = await DeviceApiService.RestartDeviceAsync(deviceId);
                    if (result.Success && result.Data)
                    {
                        await ToastService.ShowSuccessAsync(result.Message ?? "Cihaz yeniden başlatılıyor...");
                    }
                    else
                    {
                        await ToastService.ShowErrorAsync(result.Message ?? "Cihaz yeniden başlatılamadı! Cihaza erişilemiyor.");
                    }
                }
                catch (Exception ex)
                {
                    await ToastService.ShowErrorAsync($"Cihaz yeniden başlatma hatası: {ex.Message}");
                }
                finally
                {
                    SetDeviceLoading(deviceId, false);
                }
            }
        }

        private async Task PowerOffDevice(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;
            
            if (await JS.InvokeAsync<bool>("confirm", "🚨 DİKKAT! Cihazı kapatmak istediğinize emin misiniz?\n\nCihazı tekrar açmak için fiziksel müdahale gerekebilir!"))
            {
                SetDeviceLoading(deviceId, true);
                try
                {
                    var result = await DeviceApiService.PowerOffDeviceAsync(deviceId);
                    if (result.Success && result.Data)
                    {
                        await ToastService.ShowSuccessAsync(result.Message ?? "Cihaz kapatılıyor...");
                    }
                    else
                    {
                        await ToastService.ShowErrorAsync(result.Message ?? "Cihaz kapatılamadı! Cihaza erişilemiyor.");
                    }
                }
                catch (Exception ex)
                {
                    await ToastService.ShowErrorAsync($"Cihaz kapatma hatası: {ex.Message}");
                }
                finally
                {
                    SetDeviceLoading(deviceId, false);
                }
            }
        }

        private async Task StartRealtimeMonitoring(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;

            SetDeviceLoading(deviceId, true);
            try
            {
                var result = await DeviceApiService.StartRealtimeMonitoringAsync(deviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync(result.Message ?? "Canlı izleme başlatıldı!");
                    // Cihaz listesini yenile (monitoring durumunu güncellemek için)
                    await LoadDevices();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Canlı izleme başlatılamadı!");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Canlı izleme hatası: {ex.Message}");
            }
            finally
            {
                SetDeviceLoading(deviceId, false);
            }
        }

        private async Task StopRealtimeMonitoring(int deviceId)
        {
            if (IsDeviceLoading(deviceId)) return;

            SetDeviceLoading(deviceId, true);
            try
            {
                var result = await DeviceApiService.StopRealtimeMonitoringAsync(deviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync(result.Message ?? "Canlı izleme durduruldu!");
                    // Cihaz listesini yenile (monitoring durumunu güncellemek için)
                    await LoadDevices();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Canlı izleme durdurulamadı!");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Canlı izleme hatası: {ex.Message}");
            }
            finally
            {
                SetDeviceLoading(deviceId, false);
            }
        }

        private async Task Delete(int deviceId)
        {
            if (await JS.InvokeAsync<bool>("confirm", "⚠️ Cihazı silmek istediğinize emin misiniz?\n\nBu işlem geri alınamaz!"))
            {
                var result = await DeviceApiService.DeleteAsync(deviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync("Cihaz silindi!");
                    await LoadDevices();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz silinemedi!");
                }
            }
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL GÖNDERME MODALİ
        // ═══════════════════════════════════════════════════════

        private async Task OpenSendPersonelModal(int deviceId)
        {
            var device = devices?.FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null) return;

            selectedDeviceId = deviceId;
            selectedDeviceName = device.DeviceName ?? "";
            showSendPersonelModal = true;
            isLoadingPersonel = true;
            StateHasChanged();

            try
            {
                var result = await PersonelApiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    personelList = result.Data.Select(p => new PersonelDto
                    {
                        TcKimlikNo = p.TcKimlikNo,
                        SicilNo = p.SicilNo,
                        AdSoyad = p.AdSoyad,
                        DepartmanAdi = p.DepartmanAdi ?? "-",
                        PersonelKayitNo = p.PersonelKayitNo,
                        KartNo = p.KartNo
                    }).ToList();
                    filteredPersonelList = personelList;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Personeller yüklenemedi!");
                    personelList = new List<PersonelDto>();
                    filteredPersonelList = new List<PersonelDto>();
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Personel yükleme hatası: {ex.Message}");
                personelList = new List<PersonelDto>();
                filteredPersonelList = new List<PersonelDto>();
            }
            finally
            {
                isLoadingPersonel = false;
                StateHasChanged();
            }
        }

        private void CloseSendPersonelModal()
        {
            showSendPersonelModal = false;
            selectedDeviceId = 0;
            selectedDeviceName = "";
            personelList.Clear();
            filteredPersonelList.Clear();
            selectedPersonelIds.Clear();
            personelSearchTerm = "";
        }

        private void FilterPersonel()
        {
            if (string.IsNullOrWhiteSpace(personelSearchTerm))
            {
                filteredPersonelList = personelList;
            }
            else
            {
                var searchLower = personelSearchTerm.ToLower();
                filteredPersonelList = personelList
                    .Where(p =>
                        p.AdSoyad.ToLower().Contains(searchLower) ||
                        p.TcKimlikNo.Contains(searchLower) ||
                        p.SicilNo.ToString().Contains(searchLower))
                    .ToList();
            }

            StateHasChanged();
        }

        private void TogglePersonelSelection(string tcKimlikNo)
        {
            if (selectedPersonelIds.Contains(tcKimlikNo))
            {
                selectedPersonelIds.Remove(tcKimlikNo);
            }
            else
            {
                selectedPersonelIds.Add(tcKimlikNo);
            }

            StateHasChanged();
        }

        private void ToggleSelectAllPersonel()
        {
            if (IsAllPersonelSelected())
            {
                selectedPersonelIds.Clear();
            }
            else
            {
                selectedPersonelIds = filteredPersonelList.Select(p => p.TcKimlikNo).ToList();
            }

            StateHasChanged();
        }

        private bool IsAllPersonelSelected()
        {
            return filteredPersonelList.Any() &&
                   filteredPersonelList.All(p => selectedPersonelIds.Contains(p.TcKimlikNo));
        }
        
        private void CloseDevicePersonelModal()
        {
            showDevicePersonelModal = false;
            selectedDeviceId = 0;
            selectedDeviceName = "";
            devicePersonelList.Clear();
            filteredDevicePersonelList.Clear();
            devicePersonelSearchTerm = "";
        }
        
        private void FilterDevicePersonel()
        {
            if (string.IsNullOrWhiteSpace(devicePersonelSearchTerm))
            {
                filteredDevicePersonelList = devicePersonelList;
            }
            else
            {
                var searchLower = devicePersonelSearchTerm.ToLower();
                filteredDevicePersonelList = devicePersonelList
                    .Where(p =>
                        (p.DeviceUser?.Name?.ToLower().Contains(searchLower) ?? false) ||
                        (p.DeviceUser?.EnrollNumber?.ToLower().Contains(searchLower) ?? false) ||
                        (p.DeviceUser?.CardNumber?.ToString().Contains(searchLower) ?? false) ||
                        (p.PersonelInfo?.AdSoyad?.ToLower().Contains(searchLower) ?? false))
                    .ToList();
            }

            StateHasChanged();
        }

        private async Task SendSelectedPersonelToDevice()
        {
            if (!selectedPersonelIds.Any())
            {
                await JS.InvokeVoidAsync("alert", "⚠️ Lütfen en az bir personel seçin!");
                return;
            }

            if (!await JS.InvokeAsync<bool>("confirm", $"Seçili {selectedPersonelIds.Count} personeli {selectedDeviceName} cihazına göndermek istediğinize emin misiniz?"))
            {
                return;
            }

            isSendingPersonel = true;

            try
            {
                // TODO: Toplu personel gönderme API endpoint'i kullanılacak
                // Şimdilik tek tek gönderelim
                int successCount = 0;
                int failCount = 0;

                foreach (var tcKimlikNo in selectedPersonelIds)
                {
                    try
                    {
                        // TODO: ZKTeco User API Service kullanılmalı
                        // Şimdilik başarısız say
                        failCount++;
                    }
                    catch
                    {
                        failCount++;
                    }
                }

                var message = $"✅ İşlem tamamlandı!\n\n" +
                             $"Başarılı: {successCount}\n" +
                             $"Başarısız: {failCount}";

                await JS.InvokeVoidAsync("alert", message);

                if (successCount > 0)
                {
                    CloseSendPersonelModal();
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", $"❌ Hata: {ex.Message}");
            }
            finally
            {
                isSendingPersonel = false;
                StateHasChanged();
            }
        }

        // ═══════════════════════════════════════════════════════
        // LOADING STATE HELPERS
        // ═══════════════════════════════════════════════════════

        private bool IsDeviceLoading(int deviceId)
        {
            return loadingStates.ContainsKey(deviceId) && loadingStates[deviceId];
        }

        private void SetDeviceLoading(int deviceId, bool isLoading)
        {
            loadingStates[deviceId] = isLoading;
            StateHasChanged();
        }

        // ═══════════════════════════════════════════════════════
        // HELPER CLASS
        // ═══════════════════════════════════════════════════════

        public class PersonelDto
        {
            public string TcKimlikNo { get; set; } = "";
            public int SicilNo { get; set; }
            public string AdSoyad { get; set; } = "";
            public string DepartmanAdi { get; set; } = "";
            public int PersonelKayitNo { get; set; }
            public int KartNo { get; set; }
        }
    }
}
