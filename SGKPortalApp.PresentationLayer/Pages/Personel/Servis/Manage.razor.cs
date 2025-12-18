using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri; // ← YENİ EKLENEN
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Servis
{
    public partial class Manage
    {
        protected override string PagePermissionKey => "PER.SERVIS.MANAGE";

        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IServisApiService _ServisService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter] public int? Id { get; set; }

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private ServisFormModel FormModel { get; set; } = new();
        private bool IsEditMode => Id.HasValue && Id.Value > 0;
        private int CurrentPersonelSayisi { get; set; } = 0;

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private bool NotFound { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL
        // ═══════════════════════════════════════════════════════

        private bool ShowDeleteModal { get; set; } = false;
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════

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
                if (IsEditMode)
                {
                    var result = await _ServisService.GetByIdAsync(Id!.Value);

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

        // ═══════════════════════════════════════════════════════
        // FORM SUBMIT
        // ═══════════════════════════════════════════════════════

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

                    var result = await _ServisService.UpdateAsync(Id!.Value, updateDto);

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

        // ═══════════════════════════════════════════════════════
        // DELETE OPERATIONS
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
                var result = await _ServisService.DeleteAsync(Id!.Value);

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

        // ═══════════════════════════════════════════════════════
        // NAVIGATION
        // ═══════════════════════════════════════════════════════

        private void NavigateToServisList()
        {
            _navigationManager.NavigateTo("/personel/Servis");
        }

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        // ═══════════════════════════════════════════════════════
        // FORM MODEL
        // ═══════════════════════════════════════════════════════

        public class ServisFormModel
        {
            [Required(ErrorMessage = "Servis adı zorunludur")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Servis adı 2-100 karakter arasında olmalıdır")]
            public string ServisAdi { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;
        }
    }
}
