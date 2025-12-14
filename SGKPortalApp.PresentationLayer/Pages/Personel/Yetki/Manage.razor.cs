using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Yetki
{
    public partial class Manage : ComponentBase
    {
        [Inject] private IYetkiApiService _yetkiService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        private bool IsEditMode => Id.HasValue && Id.Value > 0;

        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private bool NotFound { get; set; } = false;

        private bool ShowDeleteModal { get; set; } = false;
        private bool IsDeleting { get; set; } = false;

        private YetkiFormModel FormModel { get; set; } = new();
        private List<YetkiResponseDto> YetkiDropdown { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (IsEditMode)
            {
                await LoadData();
            }
        }

        private async Task LoadData()
        {
            IsLoading = true;
            NotFound = false;

            try
            {
                var dropdownResult = await _yetkiService.GetAllAsync();
                YetkiDropdown = dropdownResult.Success && dropdownResult.Data != null
                    ? dropdownResult.Data
                    : new List<YetkiResponseDto>();

                if (IsEditMode)
                {
                    var result = await _yetkiService.GetByIdAsync(Id!.Value);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "Yetki bulunamadı!");
                    }
                    else
                    {
                        var item = result.Data;
                        FormModel = new YetkiFormModel
                        {
                            YetkiAdi = item.YetkiAdi,
                            Aciklama = item.Aciklama,
                            UstYetkiId = item.UstYetkiId,
                            ControllerAdi = item.ControllerAdi,
                            ActionAdi = item.ActionAdi
                        };

                        YetkiDropdown = YetkiDropdown
                            .Where(x => x.YetkiId != item.YetkiId)
                            .ToList();
                    }
                }
                else
                {
                    FormModel = new YetkiFormModel();
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
                NotFound = IsEditMode;
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
                    var updateDto = new YetkiUpdateRequestDto
                    {
                        YetkiAdi = FormModel.YetkiAdi.Trim(),
                        Aciklama = FormModel.Aciklama.Trim(),
                        UstYetkiId = FormModel.UstYetkiId,
                        ControllerAdi = string.IsNullOrWhiteSpace(FormModel.ControllerAdi) ? null : FormModel.ControllerAdi.Trim(),
                        ActionAdi = string.IsNullOrWhiteSpace(FormModel.ActionAdi) ? null : FormModel.ActionAdi.Trim()
                    };

                    var result = await _yetkiService.UpdateAsync(Id!.Value, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "Yetki başarıyla güncellendi!");
                        NavigateToYetkiList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Yetki güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new YetkiCreateRequestDto
                    {
                        YetkiAdi = FormModel.YetkiAdi.Trim(),
                        Aciklama = FormModel.Aciklama.Trim(),
                        UstYetkiId = FormModel.UstYetkiId,
                        ControllerAdi = string.IsNullOrWhiteSpace(FormModel.ControllerAdi) ? null : FormModel.ControllerAdi.Trim(),
                        ActionAdi = string.IsNullOrWhiteSpace(FormModel.ActionAdi) ? null : FormModel.ActionAdi.Trim()
                    };

                    var result = await _yetkiService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "Yetki başarıyla oluşturuldu!");
                        NavigateToYetkiList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Yetki oluşturulamadı!");
                    }
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"İşlem başarısız: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void ShowDeleteConfirmation() => ShowDeleteModal = true;
        private void CloseDeleteModal() => ShowDeleteModal = false;

        private async Task ConfirmDelete()
        {
            IsDeleting = true;

            try
            {
                var result = await _yetkiService.DeleteAsync(Id!.Value);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "Yetki başarıyla silindi!");
                    NavigateToYetkiList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Yetki silinemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Silme işlemi başarısız: {ex.Message}");
            }
            finally
            {
                IsDeleting = false;
                CloseDeleteModal();
            }
        }

        private void NavigateToYetkiList() => _navigationManager.NavigateTo("/personel/yetki");

        public class YetkiFormModel
        {
            [Required(ErrorMessage = "Yetki adı zorunludur")]
            [StringLength(100, ErrorMessage = "Yetki adı en fazla 100 karakter olabilir")]
            public string YetkiAdi { get; set; } = string.Empty;

            [Required(ErrorMessage = "Açıklama zorunludur")]
            [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
            public string Aciklama { get; set; } = string.Empty;

            public int? UstYetkiId { get; set; }

            [StringLength(100, ErrorMessage = "Controller adı en fazla 100 karakter olabilir")]
            public string? ControllerAdi { get; set; }

            [StringLength(100, ErrorMessage = "Action adı en fazla 100 karakter olabilir")]
            public string? ActionAdi { get; set; }
        }
    }
}
