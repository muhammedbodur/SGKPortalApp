using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Shared.ZKTeco;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class DeviceUsers : FieldPermissionPageBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IZKTecoDeviceApiService DeviceApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter] public int DeviceId { get; set; }

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string deviceName = "";
        private string deviceIp = "";
        private bool isLoading = false;

        // User List
        private List<ApiUserDto> users = new List<ApiUserDto>();
        private List<ApiUserDto> filteredUsers = new List<ApiUserDto>();
        private string searchTerm = "";
        private int privilegeFilter = -1;

        // Stats
        private int userCount => users.Count;
        private int usersWithCard => users.Count(u => u.CardNumber.HasValue && u.CardNumber.Value > 0);
        private int usersWithFingerprint => users.Count(u => u.FingerCount > 0);
        private int usersWithFace => users.Count(u => u.HasFace);

        // User Modal
        private bool showUserModal = false;
        private bool isEditMode = false;
        private bool isSaving = false;
        private bool forceCardUpdate = false;
        private UserCreateUpdateDto userForm = new UserCreateUpdateDto();

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDeviceInfo();
            await LoadUsers();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING
        // ═══════════════════════════════════════════════════════

        private async Task LoadDeviceInfo()
        {
            try
            {
                var result = await DeviceApiService.GetByIdAsync(DeviceId);
                if (result.Success && result.Data != null)
                {
                    deviceName = result.Data.DeviceName ?? "";
                    deviceIp = result.Data.IpAddress ?? "";
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihaz bilgisi yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task LoadUsers()
        {
            isLoading = true;
            try
            {
                var result = await DeviceApiService.GetDeviceUsersAsync(DeviceId);
                if (result.Success && result.Data != null)
                {
                    users = result.Data;
                    filteredUsers = users;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kullanıcılar yüklenemedi");
                    users = new List<ApiUserDto>();
                    filteredUsers = new List<ApiUserDto>();
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
                users = new List<ApiUserDto>();
                filteredUsers = new List<ApiUserDto>();
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task RefreshUsers()
        {
            await LoadUsers();
            await ToastService.ShowSuccessAsync("Kullanıcı listesi yenilendi");
        }

        // ═══════════════════════════════════════════════════════
        // FILTERING
        // ═══════════════════════════════════════════════════════

        private void FilterUsers()
        {
            filteredUsers = users;

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                filteredUsers = filteredUsers.Where(u =>
                    u.EnrollNumber.ToLower().Contains(searchLower) ||
                    u.NickName.ToLower().Contains(searchLower) ||
                    (u.CardNumber.HasValue && u.CardNumber.Value.ToString().Contains(searchTerm))
                ).ToList();
            }

            // Privilege filter
            if (privilegeFilter >= 0)
            {
                filteredUsers = filteredUsers.Where(u => u.Privilege == privilegeFilter).ToList();
            }

            StateHasChanged();
        }

        // ═══════════════════════════════════════════════════════
        // USER OPERATIONS
        // ═══════════════════════════════════════════════════════

        private void OpenAddUserModal()
        {
            isEditMode = false;
            forceCardUpdate = false;
            userForm = new UserCreateUpdateDto
            {
                Privilege = 0,
                Enabled = true
            };
            showUserModal = true;
        }

        private void OpenEditUserModal(ApiUserDto user)
        {
            isEditMode = true;
            forceCardUpdate = false;
            userForm = new UserCreateUpdateDto
            {
                EnrollNumber = user.EnrollNumber,
                NickName = user.NickName,
                Password = user.Password ?? "",
                CardNumber = user.CardNumber,
                Privilege = user.Privilege,
                Enabled = user.Enabled
            };
            showUserModal = true;
        }

        private void CloseUserModal()
        {
            showUserModal = false;
            isEditMode = false;
            forceCardUpdate = false;
            userForm = new UserCreateUpdateDto();
        }

        private async Task SaveUser()
        {
            if (string.IsNullOrWhiteSpace(userForm.EnrollNumber) || string.IsNullOrWhiteSpace(userForm.NickName))
            {
                await ToastService.ShowWarningAsync("Kayıt numarası ve NickName zorunludur!");
                return;
            }

            isSaving = true;
            try
            {
                var result = isEditMode
                    ? await DeviceApiService.UpdateDeviceUserAsync(DeviceId, userForm.EnrollNumber, userForm, forceCardUpdate)
                    : await DeviceApiService.CreateDeviceUserAsync(DeviceId, userForm, forceCardUpdate);

                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync(isEditMode ? "Kullanıcı güncellendi" : "Kullanıcı eklendi");
                    CloseUserModal();
                    await LoadUsers();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "İşlem başarısız");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isSaving = false;
                StateHasChanged();
            }
        }

        private async Task DeleteUser(string enrollNumber)
        {
            if (!await JS.InvokeAsync<bool>("confirm", $"'{enrollNumber}' kayıt numaralı kullanıcıyı silmek istediğinize emin misiniz?\n\nBu işlem geri alınamaz!"))
            {
                return;
            }

            try
            {
                var result = await DeviceApiService.DeleteDeviceUserAsync(DeviceId, enrollNumber);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync("Kullanıcı silindi");
                    await LoadUsers();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kullanıcı silinemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task RemoveCard(string enrollNumber)
        {
            if (!await JS.InvokeAsync<bool>("confirm", $"'{enrollNumber}' kayıt numaralı kullanıcının kartını kaldırmak istediğinize emin misiniz?"))
            {
                return;
            }

            try
            {
                var result = await DeviceApiService.RemoveCardFromUserAsync(DeviceId, enrollNumber);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync("Kart kaldırıldı");
                    await LoadUsers();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kart kaldırılamadı");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task ClearAllUsers()
        {
            if (!await JS.InvokeAsync<bool>("confirm", "⚠️ DİKKAT! Cihazdaki TÜM kullanıcıları silmek istediğinize emin misiniz?\n\nBu işlem geri alınamaz!"))
            {
                return;
            }

            if (!await JS.InvokeAsync<bool>("confirm", "🚨 SON UYARI! Bu işlem cihazdaki tüm kullanıcıları, parmak izlerini ve kartları kalıcı olarak silecektir.\n\nDevam etmek istediğinize EMİN MİSİNİZ?"))
            {
                return;
            }

            try
            {
                var result = await DeviceApiService.ClearAllDeviceUsersAsync(DeviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync("Tüm kullanıcılar silindi");
                    await LoadUsers();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kullanıcılar silinemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private string GetPrivilegeText(int privilege)
        {
            return privilege switch
            {
                0 => "Normal",
                2 => "Yönetici",
                14 => "Süper Admin",
                _ => "Bilinmiyor"
            };
        }

        private string GetPrivilegeBadgeClass(int privilege)
        {
            return privilege switch
            {
                0 => "bg-label-secondary",
                2 => "bg-label-warning",
                14 => "bg-label-danger",
                _ => "bg-label-secondary"
            };
        }
    }
}
