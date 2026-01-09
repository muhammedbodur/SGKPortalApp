using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class DeviceList : FieldPermissionPageBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IZKTecoDeviceApiService DeviceApiService { get; set; } = default!;
        [Inject] private IDepartmanApiService DepartmanApiService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService HizmetBinasiApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<Device>? devices;
        private bool showAddForm = false;
        private Device newDevice = new Device { Port = "4370", IsActive = true };

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
        private string personelSearchTerm = "";

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
                    devices = result.Data.Select(d => new Device 
                    { 
                        Id = d.DeviceId,
                        DeviceName = d.DeviceName,
                        IpAddress = d.IpAddress,
                        Port = d.Port,
                        IsActive = d.IsActive,
                        LastHealthCheckTime = d.LastHealthCheckTime,
                        LastHealthCheckSuccess = d.LastHealthCheckSuccess
                    }).ToList();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihazlar yüklenemedi");
                    devices = new List<Device>();
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
                devices = new List<Device>();
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
                newDevice = new Device { Port = "4370", IsActive = true };
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
            var result = await DeviceApiService.TestConnectionAsync(deviceId);
            if (result.Success && result.Data)
            {
                await ToastService.ShowSuccessAsync("Bağlantı başarılı!");
            }
            else
            {
                await ToastService.ShowErrorAsync("Bağlantı başarısız!");
            }
            await LoadDevices();
        }

        private async Task GetStatus(int deviceId)
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
                await ToastService.ShowErrorAsync(result.Message ?? "Cihaz durumu alınamadı!");
            }
        }

        private async Task GetDeviceTime(int deviceId)
        {
            var result = await DeviceApiService.GetDeviceTimeAsync(deviceId);
            if (result.Success && result.Data != null)
            {
                var timeDto = result.Data;
                var message = $"🕐 Cihaz Saati:\n{timeDto.DeviceTime:dd.MM.yyyy HH:mm:ss}\n\nSunucu ile fark: {timeDto.TimeDifferenceSeconds} saniye";
                await JS.InvokeVoidAsync("alert", message);
            }
            else
            {
                await ToastService.ShowErrorAsync(result.Message ?? "Cihaz saati alınamadı!");
            }
        }

        private async Task SyncDeviceTime(int deviceId)
        {
            if (await JS.InvokeAsync<bool>("confirm", "Cihaz saatini şu anki sunucu saatiyle senkronize etmek istediğinize emin misiniz?"))
            {
                var result = await DeviceApiService.SynchronizeDeviceTimeAsync(deviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync("Cihaz saati senkronize edildi!");
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz saati senkronize edilemedi!");
                }
            }
        }

        private async Task ShowDeviceUsers(int deviceId)
        {
            var result = await DeviceApiService.GetDeviceUsersAsync(deviceId);
            if (result.Success && result.Data != null && result.Data.Any())
            {
                await JS.InvokeVoidAsync("alert", $"👥 Cihazdaki Personel Sayısı: {result.Data.Count}\n\nDetaylı liste için 'Kullanıcı Yönetimi' sayfasını ziyaret edin.");
            }
            else
            {
                await ToastService.ShowInfoAsync("Cihazda kayıtlı personel bulunamadı.");
            }
        }

        private async Task EnableDevice(int deviceId)
        {
            var result = await DeviceApiService.EnableDeviceAsync(deviceId);
            if (result.Success && result.Data)
            {
                await ToastService.ShowSuccessAsync("Cihaz etkinleştirildi!");
            }
            else
            {
                await ToastService.ShowErrorAsync(result.Message ?? "Cihaz etkinleştirilemedi!");
            }
        }

        private async Task DisableDevice(int deviceId)
        {
            if (await JS.InvokeAsync<bool>("confirm", "⚠️ Cihazı devre dışı bırakmak istediğinize emin misiniz?\n\nKullanıcılar parmak izi okutamaz veya kart geçemez."))
            {
                var result = await DeviceApiService.DisableDeviceAsync(deviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync("Cihaz devre dışı bırakıldı!");
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz devre dışı bırakılamadı!");
                }
            }
        }

        private async Task RestartDevice(int deviceId)
        {
            if (await JS.InvokeAsync<bool>("confirm", "⚠️ Cihazı yeniden başlatmak istediğinize emin misiniz?\n\nCihaz yaklaşık 30 saniye offline olacak."))
            {
                var result = await DeviceApiService.RestartDeviceAsync(deviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync("Cihaz yeniden başlatılıyor...");
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz yeniden başlatılamadı!");
                }
            }
        }

        private async Task PowerOffDevice(int deviceId)
        {
            if (await JS.InvokeAsync<bool>("confirm", "🚨 DİKKAT! Cihazı kapatmak istediğinize emin misiniz?\n\nCihazı tekrar açmak için fiziksel müdahale gerekebilir!"))
            {
                var result = await DeviceApiService.PowerOffDeviceAsync(deviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync("Cihaz kapatılıyor...");
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz kapatılamadı!");
                }
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

            try
            {
                // TODO: Personel listesi için PersonelApiService kullanılmalı
                // Şimdilik boş liste
                personelList = new List<PersonelDto>();
                filteredPersonelList = personelList;
                await ToastService.ShowInfoAsync("Personel listesi yükleme özelliği henüz aktif değil");
            }
            catch
            {
                await ToastService.ShowErrorAsync("Personeller yüklenemedi!");
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
