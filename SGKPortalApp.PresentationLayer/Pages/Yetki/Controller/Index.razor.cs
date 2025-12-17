using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Yetki.Controller
{
    public partial class Index : ComponentBase
    {
        [Inject] private IModulControllerApiService ControllerApiService { get; set; } = default!;
        [Inject] private IModulApiService ModulApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        private List<ModulControllerResponseDto> Controllers = new();
        private List<DropdownItemDto> Moduller = new();
        private bool IsLoading = true;
        private bool IsSaving = false;
        private bool ShowModal = false;
        private int? EditingId = null;
        private int SelectedModulId = 0;
        private string ControllerAdi = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadModulDropdown();
            await LoadData();
        }

        private async Task LoadModulDropdown()
        {
            var result = await ModulApiService.GetDropdownAsync();
            if (result.Success && result.Data != null)
                Moduller = result.Data;
        }

        private async Task LoadData()
        {
            IsLoading = true;
            var result = await ControllerApiService.GetAllAsync();
            if (result.Success && result.Data != null)
                Controllers = result.Data;
            IsLoading = false;
        }

        private void ShowAddModal()
        {
            EditingId = null;
            SelectedModulId = 0;
            ControllerAdi = string.Empty;
            ShowModal = true;
        }

        private void StartEdit(ModulControllerResponseDto c)
        {
            EditingId = c.ModulControllerId;
            SelectedModulId = c.ModulId;
            ControllerAdi = c.ModulControllerAdi;
            ShowModal = true;
        }

        private void CloseModal()
        {
            ShowModal = false;
            EditingId = null;
        }

        private async Task SaveController()
        {
            if (SelectedModulId <= 0 || string.IsNullOrWhiteSpace(ControllerAdi))
            {
                await ToastService.ShowWarningAsync("Modül ve Controller adı zorunludur");
                return;
            }

            IsSaving = true;
            StateHasChanged();

            if (EditingId.HasValue)
            {
                var request = new ModulControllerUpdateRequestDto { ModulId = SelectedModulId, ModulControllerAdi = ControllerAdi.Trim() };
                var result = await ControllerApiService.UpdateAsync(EditingId.Value, request);
                if (result.Success)
                    await ToastService.ShowSuccessAsync(result.Message ?? "Controller güncellendi");
                else
                    await ToastService.ShowErrorAsync(result.Message ?? "Controller güncellenemedi");
            }
            else
            {
                var request = new ModulControllerCreateRequestDto { ModulId = SelectedModulId, ModulControllerAdi = ControllerAdi.Trim() };
                var result = await ControllerApiService.CreateAsync(request);
                if (result.Success)
                    await ToastService.ShowSuccessAsync(result.Message ?? "Controller eklendi");
                else
                    await ToastService.ShowErrorAsync(result.Message ?? "Controller eklenemedi");
            }

            IsSaving = false;
            CloseModal();
            await LoadData();
        }

        private async Task DeleteController(ModulControllerResponseDto c)
        {
            if (c.IslemCount > 0)
            {
                await ToastService.ShowWarningAsync($"Bu controller'a bağlı {c.IslemCount} işlem var. Önce işlemleri silin.");
                return;
            }

            var confirmed = await JS.InvokeAsync<bool>("confirm", $"'{c.ModulControllerAdi}' silinsin mi?");
            if (!confirmed) return;

            var result = await ControllerApiService.DeleteAsync(c.ModulControllerId);
            if (result.Success)
                await ToastService.ShowSuccessAsync(result.Message ?? "Controller silindi");
            else
                await ToastService.ShowErrorAsync(result.Message ?? "Controller silinemedi");
            await LoadData();
        }
    }
}
