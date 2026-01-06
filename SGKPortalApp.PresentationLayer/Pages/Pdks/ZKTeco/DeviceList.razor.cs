using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class DeviceList
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private IConfiguration Configuration { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<Device>? devices;
        private string apiBaseUrl = "";
        private bool showAddForm = false;
        private Device newDevice = new Device { Port = "4370", IsActive = true };

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
            apiBaseUrl = Configuration["AppSettings:ApiUrl"] ?? "https://localhost:9080";
            await LoadDevices();
        }

        // ═══════════════════════════════════════════════════════
        // DEVICE OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task LoadDevices()
        {
            try
            {
                devices = await Http.GetFromJsonAsync<List<Device>>($"{apiBaseUrl}/api/Device");
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("console.error", $"Cihazlar yüklenirken hata oluştu: {ex.Message}");
                devices = new List<Device>();
            }
        }

        private void ToggleAddForm()
        {
            showAddForm = !showAddForm;
            if (showAddForm)
            {
                newDevice = new Device { Port = "4370", IsActive = true };
            }
        }

        private async Task SaveDevice()
        {
            if (string.IsNullOrWhiteSpace(newDevice.DeviceName) || string.IsNullOrWhiteSpace(newDevice.IpAddress))
            {
                await JS.InvokeVoidAsync("alert", "Cihaz adı ve IP adresi zorunludur!");
                return;
            }

            try
            {
                var response = await Http.PostAsJsonAsync($"{apiBaseUrl}/api/Device", newDevice);
                if (response.IsSuccessStatusCode)
                {
                    await JS.InvokeVoidAsync("alert", "✅ Cihaz başarıyla eklendi!");
                    showAddForm = false;
                    await LoadDevices();
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "❌ Cihaz eklenemedi!");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", $"❌ Hata: {ex.Message}");
            }
        }

        private async Task TestConnection(int deviceId)
        {
            var response = await Http.PostAsync($"{apiBaseUrl}/api/Device/{deviceId}/test", null);
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            await JS.InvokeVoidAsync("alert", result?.Success == true ? "✅ Bağlantı başarılı!" : "❌ Bağlantı başarısız!");
            await LoadDevices();
        }

        private async Task GetStatus(int deviceId)
        {
            try
            {
                var status = await Http.GetFromJsonAsync<dynamic>($"{apiBaseUrl}/api/Device/{deviceId}/status");
                var message = $"📊 Cihaz Durum Bilgisi:\n\n" +
                             $"Firmware: {status?.FirmwareVersion}\n" +
                             $"Seri No: {status?.SerialNumber}\n" +
                             $"Platform: {status?.Platform}\n" +
                             $"Kullanıcı: {status?.UserCount} / {status?.UserCapacity}\n" +
                             $"Kayıt: {status?.AttendanceLogCount} / {status?.AttLogCapacity}\n" +
                             $"Parmak İzi: {status?.FingerPrintCount} / {status?.FingerPrintCapacity}";
                await JS.InvokeVoidAsync("alert", message);
            }
            catch
            {
                await JS.InvokeVoidAsync("alert", "❌ Cihaz durumu alınamadı!");
            }
        }

        private async Task GetDeviceTime(int deviceId)
        {
            try
            {
                var timeDto = await Http.GetFromJsonAsync<dynamic>($"{apiBaseUrl}/api/Device/{deviceId}/time");
                var deviceTime = DateTime.Parse(timeDto?.DeviceTime.ToString());
                var diff = timeDto?.TimeDifferenceSeconds;
                await JS.InvokeVoidAsync("alert", $"🕐 Cihaz Saati:\n{deviceTime:dd.MM.yyyy HH:mm:ss}\n\nSunucu ile fark: {diff} saniye");
            }
            catch
            {
                await JS.InvokeVoidAsync("alert", "❌ Cihaz saati alınamadı!");
            }
        }

        private async Task SyncDeviceTime(int deviceId)
        {
            if (await JS.InvokeAsync<bool>("confirm", "Cihaz saatini şu anki sunucu saatiyle senkronize etmek istediğinize emin misiniz?"))
            {
                try
                {
                    await Http.PostAsync($"{apiBaseUrl}/api/Device/{deviceId}/time/sync", null);
                    await JS.InvokeVoidAsync("alert", "✅ Cihaz saati senkronize edildi!");
                }
                catch
                {
                    await JS.InvokeVoidAsync("alert", "❌ Cihaz saati senkronize edilemedi!");
                }
            }
        }

        private async Task ShowDeviceUsers(int deviceId)
        {
            try
            {
                var users = await Http.GetFromJsonAsync<List<dynamic>>($"{apiBaseUrl}/api/ZKTecoUser/device/{deviceId}/from-device");
                if (users != null && users.Any())
                {
                    await JS.InvokeVoidAsync("alert", $"👥 Cihazdaki Personel Sayısı: {users.Count}\n\nDetaylı liste için 'Kullanıcı Yönetimi' sayfasını ziyaret edin.");
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "ℹ️ Cihazda kayıtlı personel bulunamadı.");
                }
            }
            catch
            {
                await JS.InvokeVoidAsync("alert", "❌ Personel listesi alınamadı!");
            }
        }

        private async Task RestartDevice(int deviceId)
        {
            if (await JS.InvokeAsync<bool>("confirm", "⚠️ Cihazı yeniden başlatmak istediğinize emin misiniz?\n\nCihaz yaklaşık 30 saniye offline olacak."))
            {
                try
                {
                    await Http.PostAsync($"{apiBaseUrl}/api/Device/{deviceId}/restart", null);
                    await JS.InvokeVoidAsync("alert", "✅ Cihaz yeniden başlatılıyor...");
                }
                catch
                {
                    await JS.InvokeVoidAsync("alert", "❌ Cihaz yeniden başlatılamadı!");
                }
            }
        }

        private async Task Delete(int deviceId)
        {
            if (await JS.InvokeAsync<bool>("confirm", "⚠️ Cihazı silmek istediğinize emin misiniz?\n\nBu işlem geri alınamaz!"))
            {
                try
                {
                    await Http.DeleteAsync($"{apiBaseUrl}/api/Device/{deviceId}");
                    await JS.InvokeVoidAsync("alert", "✅ Cihaz silindi!");
                    await LoadDevices();
                }
                catch
                {
                    await JS.InvokeVoidAsync("alert", "❌ Cihaz silinemedi!");
                }
            }
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL GÖNDERME MODALİ
        // ═══════════════════════════════════════════════════════

        private async Task OpenSendPersonelModal(int deviceId)
        {
            var device = devices?.FirstOrDefault(d => d.Id == deviceId);
            if (device == null) return;

            selectedDeviceId = deviceId;
            selectedDeviceName = device.DeviceName ?? "";
            showSendPersonelModal = true;
            isLoadingPersonel = true;

            try
            {
                // Personelleri yükle
                var result = await Http.GetFromJsonAsync<List<PersonelDto>>($"{apiBaseUrl}/api/Personel");
                personelList = result ?? new List<PersonelDto>();
                filteredPersonelList = personelList;
            }
            catch
            {
                await JS.InvokeVoidAsync("alert", "❌ Personeller yüklenemedi!");
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
                        var personel = personelList.FirstOrDefault(p => p.TcKimlikNo == tcKimlikNo);
                        if (personel != null)
                        {
                            // POST /api/ZKTecoUser/{userId}/sync-to-device/{deviceId} endpoint'ini kullan
                            var response = await Http.PostAsync(
                                $"{apiBaseUrl}/api/ZKTecoUser/{personel.PersonelKayitNo}/sync-to-device/{selectedDeviceId}",
                                null);

                            if (response.IsSuccessStatusCode)
                            {
                                successCount++;
                            }
                            else
                            {
                                failCount++;
                            }
                        }
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
