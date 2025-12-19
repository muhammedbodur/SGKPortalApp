using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Departman
{
    public partial class Manage
    {
        // ═══════════════════════════════════════════════════════
        // PERMISSION CONFIGURATION
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Permission Key: PER.PERSONELDEPARTMAN.MANAGE
        /// Route: /personel-departman/manage/{id}
        /// Convention: {MODUL_KODU}.{URL_TIRESIZ}.{ACTION}
        /// </summary>

        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IDepartmanApiService _departmanService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter] public int? Id { get; set; }

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private DepartmanFormModel FormModel { get; set; } = new();

        private bool IsEditMode => Id.HasValue && Id.Value > 0;
        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private bool NotFound { get; set; } = false;

        private int CurrentPersonelSayisi { get; set; } = 0;
        private bool ShowDeleteModal { get; set; } = false;
        private bool IsDeleting { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

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
                    var result = await _departmanService.GetByIdAsync(Id!.Value);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "Departman bulunamadı!");
                    }
                    else
                    {
                        var departman = result.Data;
                        FormModel = new DepartmanFormModel
                        {
                            DepartmanAdi = departman.DepartmanAdi,
                            IsActive = departman.Aktiflik == Aktiflik.Aktif
                        };

                        CurrentPersonelSayisi = departman.PersonelSayisi;
                    }
                }
                else
                {
                    FormModel = new DepartmanFormModel
                    {
                        DepartmanAdi = string.Empty,
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
            if (string.IsNullOrWhiteSpace(FormModel.DepartmanAdi))
            {
                await _toastService.ShowWarningAsync("Departman adı zorunludur!");
                return;
            }

            IsSaving = true;

            try
            {
                if (IsEditMode)
                {
                    var updateDto = new DepartmanUpdateRequestDto
                    {
                        DepartmanAdi = FormModel.DepartmanAdi.Trim(),
                        Aktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _departmanService.UpdateAsync(Id!.Value, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Departman başarıyla güncellendi!");
                        _navigationManager.NavigateTo("/personel/departman");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Departman güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new DepartmanCreateRequestDto
                    {
                        DepartmanAdi = FormModel.DepartmanAdi.Trim(),
                        Aktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _departmanService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Departman başarıyla oluşturuldu!");
                        _navigationManager.NavigateTo("/personel/departman");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Departman oluşturulamadı!");
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
                var result = await _departmanService.DeleteAsync(Id!.Value);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Departman başarıyla silindi!");
                    _navigationManager.NavigateTo("/personel/departman");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Departman silinemedi!");
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

        private void NavigateToDepartmanList()
        {
            _navigationManager.NavigateTo("/personel/departman");
        }

        // ═══════════════════════════════════════════════════════
        // FORM MODEL
        // ═══════════════════════════════════════════════════════

        public class DepartmanFormModel
        {
            [Required(ErrorMessage = "Departman adı zorunludur")]
            [StringLength(150, ErrorMessage = "Departman adı en fazla 150 karakter olabilir")]
            public string DepartmanAdi { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;
        }
    }
}
