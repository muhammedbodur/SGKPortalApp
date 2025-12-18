using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Pages.Common.Ilce
{
    public partial class Manage
    {
        protected override string PagePermissionKey => "CMN.ILCE.MANAGE";

        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IIlceApiService _ilceService { get; set; } = default!;
        [Inject] private IIlApiService _ilService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter] public int? Id { get; set; }

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private IlceFormModel FormModel { get; set; } = new();
        private List<IlResponseDto> Iller { get; set; } = new();
        private List<string> validationErrors = new();

        private bool IsEditMode => Id.HasValue && Id.Value > 0;
        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private bool NotFound { get; set; } = false;

        private string CurrentIlAdi { get; set; } = string.Empty;
        private bool ShowDeleteModal { get; set; } = false;
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await LoadIller();
            await LoadData();
        }

        private async Task LoadIller()
        {
            try
            {
                var result = await _ilService.GetAllAsync();

                if (result.Success && result.Data != null && result.Data.Any())
                {
                    Iller = result.Data.OrderBy(i => i.IlAdi).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"İl yükleme hatası: {ex.Message}");
                await _toastService.ShowWarningAsync("İller yüklenemedi!");
            }
        }

        private async Task LoadData()
        {
            IsLoading = true;
            NotFound = false;

            try
            {
                if (IsEditMode)
                {
                    var result = await _ilceService.GetByIdAsync(Id!.Value);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "İlçe bulunamadı!");
                    }
                    else
                    {
                        var ilce = result.Data;
                        FormModel = new IlceFormModel
                        {
                            IlId = ilce.IlId,
                            IlceAdi = ilce.IlceAdi
                        };

                        CurrentIlAdi = ilce.IlAdi;
                    }
                }
                else
                {
                    FormModel = new IlceFormModel
                    {
                        IlId = 0,
                        IlceAdi = string.Empty
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

        // ═══════════════════════════════════════════════════════
        // FORM SUBMIT
        // ═══════════════════════════════════════════════════════

        private async Task HandleSubmit()
        {
            // Validation kontrolü
            validationErrors.Clear();

            if (string.IsNullOrWhiteSpace(FormModel.IlceAdi))
            {
                validationErrors.Add("İlçe adı zorunludur!");
            }

            if (FormModel.IlId == 0)
            {
                validationErrors.Add("İl seçimi zorunludur!");
            }

            if (validationErrors.Any())
            {
                await _toastService.ShowWarningAsync("Lütfen tüm zorunlu alanları doldurun!");
                StateHasChanged();
                return;
            }

            IsSaving = true;

            try
            {
                if (IsEditMode)
                {
                    var updateDto = new IlceUpdateRequestDto
                    {
                        IlId = FormModel.IlId,
                        IlceAdi = FormModel.IlceAdi.Trim()
                    };

                    var result = await _ilceService.UpdateAsync(Id!.Value, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("İlçe başarıyla güncellendi!");
                        _navigationManager.NavigateTo("/common/ilce");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "İlçe güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new IlceCreateRequestDto
                    {
                        IlId = FormModel.IlId,
                        IlceAdi = FormModel.IlceAdi.Trim()
                    };

                    var result = await _ilceService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("İlçe başarıyla oluşturuldu!");
                        _navigationManager.NavigateTo("/common/ilce");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "İlçe oluşturulamadı!");
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

        // ═══════════════════════════════════════════════════════
        // DELETE METHODS
        // ═══════════════════════════════════════════════════════

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
                var result = await _ilceService.DeleteAsync(Id!.Value);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("İlçe başarıyla silindi!");
                    _navigationManager.NavigateTo("/common/ilce");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İlçe silinemedi!");
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

        // ═══════════════════════════════════════════════════════
        // NAVIGATION METHODS
        // ═══════════════════════════════════════════════════════

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        private void NavigateToIlceList()
        {
            _navigationManager.NavigateTo("/common/ilce");
        }

        // ═══════════════════════════════════════════════════════
    }
}
