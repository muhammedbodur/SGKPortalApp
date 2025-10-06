using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri; // ← YENİ EKLENEN
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices;
using SGKPortalApp.PresentationLayer.Services.UIServices;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Departman
{
    public partial class Manage : ComponentBase
    {
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
                            IsActive = departman.DepartmanAktiflik == Aktiflik.Aktif
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
            IsSaving = true;

            try
            {
                if (IsEditMode)
                {
                    var updateDto = new DepartmanUpdateRequestDto
                    {
                        DepartmanAdi = FormModel.DepartmanAdi.Trim(),
                        DepartmanAktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _departmanService.UpdateAsync(Id!.Value, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "Departman başarıyla güncellendi!");
                        NavigateToDepartmanList();
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
                        DepartmanAktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _departmanService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "Departman başarıyla oluşturuldu!");
                        NavigateToDepartmanList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Departman oluşturulamadı!");
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
                var result = await _departmanService.DeleteAsync(Id!.Value);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "Departman başarıyla silindi!");
                    NavigateToDepartmanList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Departman silinemedi!");
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

        private void NavigateToDepartmanList()
        {
            _navigationManager.NavigateTo("/personel/departman");
        }

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        // ═══════════════════════════════════════════════════════
        // FORM MODEL
        // ═══════════════════════════════════════════════════════

        public class DepartmanFormModel
        {
            [Required(ErrorMessage = "Departman adı zorunludur")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Departman adı 2-100 karakter arasında olmalıdır")]
            public string DepartmanAdi { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;
        }
    }
}
