using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.OnemliLink
{
    public partial class Manage
    {
        protected override string? PagePermissionKey => "COM.ONEMLILINK.INDEX";

        [Inject] private IOnemliLinkApiService _onemliLinkService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        private OnemliLinkFormModel FormModel { get; set; } = new();

        protected override bool IsEditMode => Id.HasValue && Id.Value > 0;

        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private bool NotFound { get; set; } = false;

        private bool ShowDeleteModal { get; set; } = false;
        private bool IsDeleting { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            IsLoading = true;
            NotFound = false;

            try
            {
                if (IsEditMode)
                {
                    var result = await _onemliLinkService.GetByIdAsync(Id!.Value);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "Kayıt bulunamadı!");
                    }
                    else
                    {
                        var item = result.Data;
                        FormModel = new OnemliLinkFormModel
                        {
                            LinkAdi = item.LinkAdi,
                            Url = item.Url,
                            Sira = item.Sira,
                            Aktiflik = item.Aktiflik
                        };
                    }
                }
                else
                {
                    FormModel = new OnemliLinkFormModel();
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
                NotFound = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            IsSaving = true;

            try
            {
                if (!IsEditMode)
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
                else
                {
                    var request = new OnemliLinkUpdateRequestDto
                    {
                        LinkAdi = FormModel.LinkAdi,
                        Url = FormModel.Url,
                        Sira = FormModel.Sira,
                        Aktiflik = FormModel.Aktiflik
                    };

                    var result = await _onemliLinkService.UpdateAsync(Id!.Value, request);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Link başarıyla güncellendi!");
                        NavigateToList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Link güncellenemedi!");
                    }
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

        private void OpenDeleteModal()
        {
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
        }

        private async Task ConfirmDelete()
        {
            IsDeleting = true;
            try
            {
                var result = await _onemliLinkService.DeleteAsync(Id!.Value);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Link başarıyla silindi!");
                    CloseDeleteModal();
                    NavigateToList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Link silinemedi!");
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
    }
}
