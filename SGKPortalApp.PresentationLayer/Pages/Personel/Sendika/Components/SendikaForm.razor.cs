using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Sendika.Components
{
    public partial class SendikaForm : SGKPortalApp.PresentationLayer.Components.Base.FieldPermissionPageBase
    {
        [Inject] private ISendikaApiService _sendikaService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        [Parameter] public int Id { get; set; }

        protected override bool IsEditMode => Id > 0;

        private SendikaFormModel FormModel { get; set; } = new();
        private bool IsLoading { get; set; }
        private bool IsSaving { get; set; }
        private bool NotFound { get; set; }
        private bool ShowDeleteModal { get; set; }
        private bool IsDeleting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (IsEditMode)
            {
                await LoadSendika();
            }
        }

        private async Task LoadSendika()
        {
            IsLoading = true;
            try
            {
                var result = await _sendikaService.GetByIdAsync(Id);

                if (result.Success && result.Data != null)
                {
                    FormModel.SendikaAdi = result.Data.SendikaAdi;
                    FormModel.IsActive = result.Data.Aktiflik == Aktiflik.Aktif;
                }
                else
                {
                    NotFound = true;
                    await _toastService.ShowErrorAsync("Sendika bulunamadı!");
                }
            }
            catch (Exception ex)
            {
                NotFound = true;
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
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
                    var updateDto = new SendikaUpdateRequestDto
                    {
                        SendikaAdi = FormModel.SendikaAdi,
                        Aktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _sendikaService.UpdateAsync(Id, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Sendika başarıyla güncellendi!");
                        NavigateToSendikaList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result?.Message ?? "Güncelleme başarısız!");
                    }
                }
                else
                {
                    var createDto = new SendikaCreateRequestDto
                    {
                        SendikaAdi = FormModel.SendikaAdi
                    };

                    var result = await _sendikaService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Sendika başarıyla oluşturuldu!");
                        NavigateToSendikaList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result?.Message ?? "Oluşturma başarısız!");
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
                var result = await _sendikaService.DeleteAsync(Id);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Sendika başarıyla silindi!");
                    NavigateToSendikaList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result?.Message ?? "Silme işlemi başarısız!");
                    CloseDeleteModal();
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
                CloseDeleteModal();
            }
            finally
            {
                IsDeleting = false;
            }
        }

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        private void NavigateToSendikaList()
        {
            _navigationManager.NavigateTo("/personel/sendika");
        }
    }
}
