using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.OnemliLink
{
    public partial class Add
    {
        protected override string? PagePermissionKey => "COM.ONEMLILINK.INDEX";

        [Inject] private IOnemliLinkApiService _onemliLinkService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private OnemliLinkFormModel FormModel { get; set; } = new();

        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            FormModel = new OnemliLinkFormModel();
            IsLoading = false;
            await Task.CompletedTask;
        }

        private async Task HandleSubmit()
        {
            IsSaving = true;

            try
            {
                var request = new OnemliLinkCreateRequestDto
                {
                    LinkAdi = FormModel.LinkAdi,
                    Url = FormModel.Url,
                    Sira = FormModel.Sira,
                    Aktiflik = FormModel.Aktiflik
                };

                var result = await _onemliLinkService.CreateAsync(request);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Link başarıyla oluşturuldu!");
                    NavigateToList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Link oluşturulamadı!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        private void NavigateToList()
        {
            _navigationManager.NavigateTo("/common/onemli-link");
        }
    }
}
