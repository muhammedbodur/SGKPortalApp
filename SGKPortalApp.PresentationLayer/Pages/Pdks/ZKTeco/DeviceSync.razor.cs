using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class DeviceSync
    {
        [Inject] private IZKTecoDeviceApiService _deviceApiService { get; set; } = default!;
        [Inject] private IZKTecoUserApiService _userApiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<Device> Devices { get; set; } = new();
        private int SelectedDeviceId { get; set; } = 0;
        private bool IsLoading { get; set; } = false;
        private bool IsSyncing { get; set; } = false;
        private bool HasLoadedData { get; set; } = false;

        // Sync Data
        private List<ZKTecoApiUserDto> DeviceUsers { get; set; } = new();
        private List<ZKTecoUserDto> DbUsers { get; set; } = new();

        // Comparison Results
        private List<ZKTecoApiUserDto> OrphanedDeviceUsers { get; set; } = new();
        private List<SyncedUserPair> SyncedUsers { get; set; } = new();
        private List<ZKTecoUserDto> OnlyInDbUsers { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await LoadDevices();
        }

        // ═══════════════════════════════════════════════════════
        // METHODS
        // ═══════════════════════════════════════════════════════

        private async Task LoadDevices()
        {
            try
            {
                var result = await _deviceApiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    Devices = result.Data;
                }
                else
                {
                    await _toastService.ShowErrorAsync("Cihazlar yüklenemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Cihazlar yüklenirken hata: {ex.Message}");
            }
        }

        private async Task LoadSyncData()
        {
            if (SelectedDeviceId == 0)
            {
                await _toastService.ShowWarningAsync("Lütfen bir cihaz seçin!");
                return;
            }

            IsLoading = true;
            HasLoadedData = false;

            try
            {
                // Cihazdan kullanıcıları çek
                var deviceUsersTask = _userApiService.GetUsersFromDeviceAsync(SelectedDeviceId);

                // Veritabanından bu cihaza ait kullanıcıları çek
                var dbUsersTask = _userApiService.GetByDeviceIdAsync(SelectedDeviceId);

                await Task.WhenAll(deviceUsersTask, dbUsersTask);

                var deviceUsersResult = await deviceUsersTask;
                var dbUsersResult = await dbUsersTask;

                if (!deviceUsersResult.Success)
                {
                    await _toastService.ShowErrorAsync($"Cihaz kullanıcıları alınamadı: {deviceUsersResult.Message}");
                    return;
                }

                if (!dbUsersResult.Success)
                {
                    await _toastService.ShowErrorAsync($"Veritabanı kullanıcıları alınamadı: {dbUsersResult.Message}");
                    return;
                }

                DeviceUsers = deviceUsersResult.Data ?? new List<ZKTecoApiUserDto>();
                DbUsers = dbUsersResult.Data ?? new List<ZKTecoUserDto>();

                // Karşılaştırma yap
                CompareUsers();

                HasLoadedData = true;
                await _toastService.ShowSuccessAsync($"Toplam {DeviceUsers.Count} cihaz kaydı ve {DbUsers.Count} veritabanı kaydı yüklendi.");
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veriler yüklenirken hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CompareUsers()
        {
            // EnrollNumber bazlı karşılaştırma yap
            var deviceEnrollNumbers = DeviceUsers.Select(u => u.EnrollNumber).ToHashSet();
            var dbEnrollNumbers = DbUsers.Select(u => u.EnrollNumber).ToHashSet();

            // 1. Sadece cihazda olanlar (Orphaned)
            OrphanedDeviceUsers = DeviceUsers
                .Where(du => !dbEnrollNumbers.Contains(du.EnrollNumber))
                .ToList();

            // 2. Her iki yerde de olanlar (Synced)
            SyncedUsers = DeviceUsers
                .Where(du => dbEnrollNumbers.Contains(du.EnrollNumber))
                .Select(du => new SyncedUserPair
                {
                    DeviceUser = du,
                    DbUser = DbUsers.First(dbu => dbu.EnrollNumber == du.EnrollNumber)
                })
                .ToList();

            // 3. Sadece veritabanında olanlar
            OnlyInDbUsers = DbUsers
                .Where(dbu => !deviceEnrollNumbers.Contains(dbu.EnrollNumber))
                .ToList();
        }

        private async Task SyncAllOrphanedToDb()
        {
            if (!OrphanedDeviceUsers.Any())
            {
                await _toastService.ShowWarningAsync("Senkronize edilecek kayıt yok!");
                return;
            }

            IsSyncing = true;

            try
            {
                var result = await _userApiService.SyncUsersFromDeviceToDbAsync(SelectedDeviceId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"{OrphanedDeviceUsers.Count} kayıt veritabanına aktarıldı!");

                    // Verileri yeniden yükle
                    await LoadSyncData();
                }
                else
                {
                    await _toastService.ShowErrorAsync($"Senkronizasyon başarısız: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Senkronizasyon hatası: {ex.Message}");
            }
            finally
            {
                IsSyncing = false;
            }
        }

        private async Task ShowUserDetails(ZKTecoApiUserDto user)
        {
            var details = $"Enroll Number: {user.EnrollNumber}\n" +
                         $"Ad Soyad: {user.Name}\n" +
                         $"Kart No: {user.CardNumber ?? 0}\n" +
                         $"Yetki: {GetPrivilegeText(user.Privilege)}\n" +
                         $"Durum: {(user.Enabled ? "Aktif" : "Pasif")}\n" +
                         $"Şifre: {(string.IsNullOrEmpty(user.Password) ? "Yok" : "Var")}";

            await _toastService.ShowInfoAsync(details);
        }

        private string GetPrivilegeText(int privilege)
        {
            return privilege switch
            {
                0 => "Kullanıcı",
                1 => "Kayıt Yetkilisi",
                2 => "Yönetici",
                3 => "Süper Admin",
                _ => "Bilinmiyor"
            };
        }

        // ═══════════════════════════════════════════════════════
        // HELPER CLASSES
        // ═══════════════════════════════════════════════════════

        public class SyncedUserPair
        {
            public ZKTecoApiUserDto DeviceUser { get; set; } = default!;
            public ZKTecoUserDto DbUser { get; set; } = default!;
        }
    }
}
