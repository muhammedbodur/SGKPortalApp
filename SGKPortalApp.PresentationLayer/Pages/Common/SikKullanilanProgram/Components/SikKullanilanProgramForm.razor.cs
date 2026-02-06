using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.SikKullanilanProgram.Components
{
    public partial class SikKullanilanProgramForm : SGKPortalApp.PresentationLayer.Components.Base.FieldPermissionPageBase
    {
        [Parameter] public int Id { get; set; }

        [Inject] private ISikKullanilanProgramApiService _programService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private SikKullanilanProgramFormModel FormModel { get; set; } = new();

        protected override bool IsEditMode => Id > 0;

        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; }
        private bool NotFound { get; set; }

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
                    var result = await _programService.GetByIdAsync(Id);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "Kayıt bulunamadı!");
                    }
                    else
                    {
                        var item = result.Data;
                        FormModel = new SikKullanilanProgramFormModel
                        {
                            ProgramAdi = item.ProgramAdi,
                            Url = item.Url,
                            IkonClass = item.IkonClass,
                            RenkKodu = item.RenkKodu,
                            Sira = item.Sira,
                            Aktiflik = item.Aktiflik
                        };
                    }
                }
                else
                {
                    FormModel = new SikKullanilanProgramFormModel();
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
                    var request = new SikKullanilanProgramCreateRequestDto
                    {
                        ProgramAdi = FormModel.ProgramAdi,
                        Url = FormModel.Url,
                        IkonClass = FormModel.IkonClass,
                        RenkKodu = FormModel.RenkKodu,
                        Sira = FormModel.Sira,
                        Aktiflik = FormModel.Aktiflik
                    };

                    var result = await _programService.CreateAsync(request);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Program başarıyla oluşturuldu!");
                        NavigateToList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Program oluşturulamadı!");
                    }
                }
                else
                {
                    var request = new SikKullanilanProgramUpdateRequestDto
                    {
                        ProgramAdi = FormModel.ProgramAdi,
                        Url = FormModel.Url,
                        IkonClass = FormModel.IkonClass,
                        RenkKodu = FormModel.RenkKodu,
                        Sira = FormModel.Sira,
                        Aktiflik = FormModel.Aktiflik
                    };

                    var result = await _programService.UpdateAsync(Id, request);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Program başarıyla güncellendi!");
                        NavigateToList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Program güncellenemedi!");
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
            _navigationManager.NavigateTo("/common/sik-kullanilan-program");
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
                var result = await _programService.DeleteAsync(Id);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Program başarıyla silindi!");
                    CloseDeleteModal();
                    NavigateToList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Program silinemedi!");
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
