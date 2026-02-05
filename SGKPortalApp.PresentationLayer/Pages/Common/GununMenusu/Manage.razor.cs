using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.GununMenusu
{
    public partial class Manage
    {
        [Inject] private IGununMenusuApiService _gununMenusuService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        private GununMenusuFormModel FormModel { get; set; } = new();

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
                    var result = await _gununMenusuService.GetByIdAsync(Id!.Value);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "Menü bulunamadı!");
                    }
                    else
                    {
                        var menu = result.Data;
                        FormModel = new GununMenusuFormModel
                        {
                            Tarih = menu.Tarih,
                            Icerik = menu.Icerik,
                            Aktiflik = menu.Aktiflik
                        };
                    }
                }
                else
                {
                    FormModel = new GununMenusuFormModel();
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
                if (IsEditMode)
                {
                    var updateDto = new GununMenusuUpdateRequestDto
                    {
                        Tarih = FormModel.Tarih.Date,
                        Icerik = FormModel.Icerik.Trim(),
                        Aktiflik = FormModel.Aktiflik
                    };

                    var result = await _gununMenusuService.UpdateAsync(Id!.Value, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Menü başarıyla güncellendi!");
                        _navigationManager.NavigateTo("/common/gunun-menusu");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Menü güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new GununMenusuCreateRequestDto
                    {
                        Tarih = FormModel.Tarih.Date,
                        Icerik = FormModel.Icerik.Trim(),
                        Aktiflik = FormModel.Aktiflik
                    };

                    var result = await _gununMenusuService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Menü başarıyla oluşturuldu!");
                        _navigationManager.NavigateTo("/common/gunun-menusu");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Menü oluşturulamadı!");
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
            IsDeleting = true;

            try
            {
                var result = await _gununMenusuService.DeleteAsync(Id!.Value);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Menü başarıyla silindi!");
                    _navigationManager.NavigateTo("/common/gunun-menusu");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Menü silinemedi!");
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

        private void NavigateToList()
        {
            _navigationManager.NavigateTo("/common/gunun-menusu");
        }
    }
}
