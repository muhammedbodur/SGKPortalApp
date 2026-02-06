using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.Il.Components
{
    public partial class IlForm
    {
        [Parameter] public int? Id { get; set; }

        [Inject] private IIlApiService _ilService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private IlFormModel FormModel { get; set; } = new();

        private bool IsEditMode => Id.HasValue && Id.Value > 0;
        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; }
        private bool NotFound { get; set; }

        private int CurrentIlceSayisi { get; set; }
        private bool ShowDeleteModal { get; set; }
        private bool IsDeleting { get; set; }

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
                    var result = await _ilService.GetByIdAsync(Id!.Value);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "İl bulunamadı!");
                    }
                    else
                    {
                        var il = result.Data;
                        FormModel = new IlFormModel
                        {
                            IlAdi = il.IlAdi
                        };

                        CurrentIlceSayisi = il.IlceSayisi;
                    }
                }
                else
                {
                    FormModel = new IlFormModel
                    {
                        IlAdi = string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Veri yüklenirken bir hata oluştu!");
                NotFound = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            if (string.IsNullOrWhiteSpace(FormModel.IlAdi))
            {
                await _toastService.ShowWarningAsync("İl adı zorunludur!");
                return;
            }

            IsSaving = true;

            try
            {
                if (IsEditMode)
                {
                    var updateDto = new IlUpdateRequestDto
                    {
                        IlAdi = FormModel.IlAdi.Trim()
                    };

                    var result = await _ilService.UpdateAsync(Id!.Value, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("İl başarıyla güncellendi!");
                        _navigationManager.NavigateTo("/common/il");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "İl güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new IlCreateRequestDto
                    {
                        IlAdi = FormModel.IlAdi.Trim()
                    };

                    var result = await _ilService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("İl başarıyla oluşturuldu!");
                        _navigationManager.NavigateTo("/common/il");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "İl oluşturulamadı!");
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

        private void ShowDeleteConfirmation()
        {
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
        }

        private async Task ConfirmDelete()
        {
            if (!IsEditMode)
                return;

            IsDeleting = true;

            try
            {
                var result = await _ilService.DeleteAsync(Id!.Value);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("İl başarıyla silindi!");
                    _navigationManager.NavigateTo("/common/il");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İl silinemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsDeleting = false;
                CloseDeleteModal();
            }
        }

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        private void NavigateToIlList()
        {
            _navigationManager.NavigateTo("/common/il");
        }
    }
}
