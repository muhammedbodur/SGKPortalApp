using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Yetki.Modul
{
    public partial class Index
    {
        [Inject] private IModulApiService ModulApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        /// <summary>
        /// Permission Key: YET.YETKIMODUL.INDEX
        /// Route: /yetki-modul
        /// Convention: {MODUL_KODU}.{URL_TIRESIZ}.{ACTION}
        /// </summary>
        protected override string PagePermissionKey => "YET.YETKIMODUL.INDEX";

        private List<ModulResponseDto> Moduller = new();
        private bool IsLoading = true;
        private bool IsSaving = false;
        private bool ShowModal = false;

        // Add Modal
        private string NewModulAdi = string.Empty;
        private string NewModulKodu = string.Empty;

        // Edit Mode
        private int? EditingId = null;
        private string EditingModulAdi = string.Empty;
        private string EditingModulKodu = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadData();
        }

        private async Task LoadData()
        {
            IsLoading = true;
            StateHasChanged();

            var result = await ModulApiService.GetAllAsync();
            if (result.Success && result.Data != null)
            {
                Moduller = result.Data;
            }
            else
            {
                await ToastService.ShowErrorAsync(result.Message ?? "Modüller yüklenemedi");
            }

            IsLoading = false;
            StateHasChanged();
        }

        private void ShowAddModal()
        {
            NewModulAdi = string.Empty;
            NewModulKodu = string.Empty;
            ShowModal = true;
        }

        private void CloseModal()
        {
            ShowModal = false;
            NewModulAdi = string.Empty;
            NewModulKodu = string.Empty;
        }

        private async Task AddModul()
        {
            if (string.IsNullOrWhiteSpace(NewModulAdi))
            {
                await ToastService.ShowWarningAsync("Modül adı boş olamaz");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewModulKodu))
            {
                await ToastService.ShowWarningAsync("Modül kodu boş olamaz");
                return;
            }

            IsSaving = true;
            StateHasChanged();

            var request = new ModulCreateRequestDto
            {
                ModulAdi = NewModulAdi.Trim(),
                ModulKodu = NewModulKodu.Trim().ToUpperInvariant()
            };

            var result = await ModulApiService.CreateAsync(request);

            if (result.Success)
            {
                await ToastService.ShowSuccessAsync(result.Message ?? "Modül başarıyla eklendi");
                CloseModal();
                await LoadData();
            }
            else
            {
                await ToastService.ShowErrorAsync(result.Message ?? "Modül eklenemedi");
            }

            IsSaving = false;
            StateHasChanged();
        }

        private void StartEdit(ModulResponseDto modul)
        {
            EditingId = modul.ModulId;
            EditingModulAdi = modul.ModulAdi;
            EditingModulKodu = modul.ModulKodu;
        }

        private void CancelEdit()
        {
            EditingId = null;
            EditingModulAdi = string.Empty;
            EditingModulKodu = string.Empty;
        }

        private async Task SaveEdit(int id)
        {
            if (string.IsNullOrWhiteSpace(EditingModulAdi))
            {
                await ToastService.ShowWarningAsync("Modül adı boş olamaz");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingModulKodu))
            {
                await ToastService.ShowWarningAsync("Modül kodu boş olamaz");
                return;
            }

            IsSaving = true;
            StateHasChanged();

            var request = new ModulUpdateRequestDto
            {
                ModulAdi = EditingModulAdi.Trim(),
                ModulKodu = EditingModulKodu.Trim().ToUpperInvariant()
            };

            var result = await ModulApiService.UpdateAsync(id, request);

            if (result.Success)
            {
                await ToastService.ShowSuccessAsync(result.Message ?? "Modül başarıyla güncellendi");
                CancelEdit();
                await LoadData();
            }
            else
            {
                await ToastService.ShowErrorAsync(result.Message ?? "Modül güncellenemedi");
            }

            IsSaving = false;
            StateHasChanged();
        }

        private async Task DeleteModul(ModulResponseDto modul)
        {
            if (modul.ControllerCount > 0)
            {
                await ToastService.ShowWarningAsync($"Bu modüle bağlı {modul.ControllerCount} controller bulunmaktadır. Önce onları silmelisiniz.");
                return;
            }

            var confirmed = await JS.InvokeAsync<bool>("confirm",
                $"'{modul.ModulAdi}' modülünü silmek istediğinizden emin misiniz?");

            if (!confirmed) return;

            IsSaving = true;
            StateHasChanged();

            var result = await ModulApiService.DeleteAsync(modul.ModulId);

            if (result.Success)
            {
                await ToastService.ShowSuccessAsync(result.Message ?? "Modül başarıyla silindi");
                await LoadData();
            }
            else
            {
                await ToastService.ShowErrorAsync(result.Message ?? "Modül silinemedi");
            }

            IsSaving = false;
            StateHasChanged();
        }
    }
}
