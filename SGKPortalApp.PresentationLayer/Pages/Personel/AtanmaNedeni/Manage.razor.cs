using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.AtanmaNedeni
{
    public partial class Manage : ComponentBase
    {
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IAtanmaNedeniApiService _atanmaNedeniApiService { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        private bool IsEditMode => Id.HasValue && Id.Value > 0;
        private bool IsLoading { get; set; } = false;
        private bool IsSaving { get; set; } = false;
        private bool IsDeleting { get; set; } = false;
        private bool ShowDeleteModal { get; set; } = false;

        private AtanmaNedeniCreateRequestDto FormModel { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            if (IsEditMode)
            {
                await LoadData();
            }
        }

        private async Task LoadData()
        {
            try
            {
                IsLoading = true;
                var response = await _atanmaNedeniApiService.GetByIdAsync(Id!.Value);

                if (response?.Success != true || response.Data == null)
                {
                    await _toastService.ShowErrorAsync(response?.Message ?? "Atanma nedeni bulunamadı!");
                    NavigateToList();
                    return;
                }

                FormModel.AtanmaNedeni = response.Data.AtanmaNedeni;
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veri yüklenirken hata oluştu: {ex.Message}");
                NavigateToList();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            try
            {
                IsSaving = true;

                if (IsEditMode)
                {
                    var updateDto = new AtanmaNedeniUpdateRequestDto
                    {
                        AtanmaNedeni = FormModel.AtanmaNedeni
                    };

                    var response = await _atanmaNedeniApiService.UpdateAsync(Id!.Value, updateDto);

                    if (response?.Success == true)
                    {
                        await _toastService.ShowSuccessAsync($"{FormModel.AtanmaNedeni} başarıyla güncellendi!");
                        NavigateToList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(response?.Message ?? "Güncelleme işlemi başarısız oldu!");
                    }
                }
                else
                {
                    var response = await _atanmaNedeniApiService.CreateAsync(FormModel);

                    if (response?.Success == true)
                    {
                        await _toastService.ShowSuccessAsync($"{FormModel.AtanmaNedeni} başarıyla eklendi!");
                        NavigateToList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(response?.Message ?? "Ekleme işlemi başarısız oldu!");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                await _toastService.ShowErrorAsync(ex.Message);
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"İşlem sırasında hata oluştu: {ex.Message}");
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
            _navigationManager.NavigateTo("/personel/atanma-nedeni");
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
            if (!IsEditMode || Id == null) return;

            try
            {
                IsDeleting = true;
                var response = await _atanmaNedeniApiService.DeleteAsync(Id.Value);

                if (response?.Success == true)
                {
                    await _toastService.ShowSuccessAsync($"{FormModel.AtanmaNedeni} başarıyla silindi!");
                    NavigateToList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(response?.Message ?? "Silme işlemi başarısız oldu!");
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
    }
}
