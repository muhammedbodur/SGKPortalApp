using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Servis.Components
{
    public partial class ServisForm : SGKPortalApp.PresentationLayer.Components.Base.FieldPermissionPageBase
    {
        [Inject] private IServisApiService _ServisService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        [Parameter] public int Id { get; set; }

        protected override bool IsEditMode => Id > 0;

        private ServisFormModel FormModel { get; set; } = new();
        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; }
        private bool NotFound { get; set; }
        private int CurrentPersonelSayisi { get; set; }
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
                    var result = await _ServisService.GetByIdAsync(Id);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "Servis bulunamadı!");
                    }
                    else
                    {
                        var Servis = result.Data;
                        FormModel = new ServisFormModel
                        {
                            ServisAdi = Servis.ServisAdi,
                            IsActive = Servis.Aktiflik == Aktiflik.Aktif
                        };

                        CurrentPersonelSayisi = Servis.PersonelSayisi;
                    }
                }
                else
                {
                    FormModel = new ServisFormModel
                    {
                        ServisAdi = string.Empty,
                        IsActive = true
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
            IsSaving = true;

            try
            {
                if (IsEditMode)
                {
                    var updateDto = new ServisUpdateRequestDto
                    {
                        ServisAdi = FormModel.ServisAdi.Trim(),
                        Aktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _ServisService.UpdateAsync(Id, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "Servis başarıyla güncellendi!");
                        NavigateToServisList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Servis güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new ServisCreateRequestDto
                    {
                        ServisAdi = FormModel.ServisAdi.Trim(),
                        Aktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _ServisService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "Servis başarıyla oluşturuldu!");
                        NavigateToServisList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Servis oluşturulamadı!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync($"İşlem başarısız: {ex.Message}");
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
                var result = await _ServisService.DeleteAsync(Id);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "Servis başarıyla silindi!");
                    NavigateToServisList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Servis silinemedi!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Silme işlemi başarısız!");
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

        private void NavigateToServisList()
        {
            _navigationManager.NavigateTo("/personel/servis");
        }

        public class ServisFormModel
        {
            [Required(ErrorMessage = "Servis adı zorunludur")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Servis adı 2-100 karakter arasında olmalıdır")]
            public string ServisAdi { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;
        }
    }
}
