using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Unvan.Components
{
    public partial class UnvanForm : SGKPortalApp.PresentationLayer.Components.Base.FieldPermissionPageBase
    {
        [Inject] private IUnvanApiService _unvanService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        [Parameter] public int Id { get; set; }

        protected override bool IsEditMode => Id > 0;

        private UnvanFormModel FormModel { get; set; } = new();
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
                    var result = await _unvanService.GetByIdAsync(Id);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "Unvan bulunamadı!");
                    }
                    else
                    {
                        var unvan = result.Data;
                        FormModel = new UnvanFormModel
                        {
                            UnvanAdi = unvan.UnvanAdi,
                            IsActive = unvan.Aktiflik == Aktiflik.Aktif
                        };

                        CurrentPersonelSayisi = unvan.PersonelSayisi;
                    }
                }
                else
                {
                    FormModel = new UnvanFormModel
                    {
                        UnvanAdi = string.Empty,
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
                    var updateDto = new UnvanUpdateRequestDto
                    {
                        UnvanAdi = FormModel.UnvanAdi.Trim(),
                        Aktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _unvanService.UpdateAsync(Id, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "Unvan başarıyla güncellendi!");
                        NavigateToUnvanList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Unvan güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new UnvanCreateRequestDto
                    {
                        UnvanAdi = FormModel.UnvanAdi.Trim(),
                        Aktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _unvanService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "Unvan başarıyla oluşturuldu!");
                        NavigateToUnvanList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Unvan oluşturulamadı!");
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
                var result = await _unvanService.DeleteAsync(Id);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "Unvan başarıyla silindi!");
                    NavigateToUnvanList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Unvan silinemedi!");
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

        private void NavigateToUnvanList()
        {
            _navigationManager.NavigateTo("/personel/unvan");
        }

        public class UnvanFormModel
        {
            [Required(ErrorMessage = "Unvan adı zorunludur")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Unvan adı 2-100 karakter arasında olmalıdır")]
            public string UnvanAdi { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;
        }
    }
}
