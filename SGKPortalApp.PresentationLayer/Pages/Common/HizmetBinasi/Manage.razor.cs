using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Pages.Common.HizmetBinasi
{
    public partial class Manage : ComponentBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter] public int? Id { get; set; }

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private HizmetBinasiFormModel FormModel { get; set; } = new();
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
                    var result = await _hizmetBinasiService.GetByIdAsync(Id!.Value);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "HizmetBinasi bulunamadı!");
                    }
                    else
                    {
                        var hizmetBinasi = result.Data;
                        FormModel = new HizmetBinasiFormModel
                        {
                            HizmetBinasiAdi = hizmetBinasi.HizmetBinasiAdi,
                            IsActive = hizmetBinasi.HizmetBinasiAktiflik == Aktiflik.Aktif
                        };

                        CurrentPersonelSayisi = hizmetBinasi.PersonelSayisi;
                    }
                }
                else
                {
                    FormModel = new HizmetBinasiFormModel
                    {
                        HizmetBinasiAdi = string.Empty,
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
                    var updateDto = new HizmetBinasiUpdateRequestDto
                    {
                        HizmetBinasiAdi = FormModel.HizmetBinasiAdi.Trim(),
                        HizmetBinasiAktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _hizmetBinasiService.UpdateAsync(Id!.Value, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "HizmetBinasi başarıyla güncellendi!");
                        NavigateToHizmetBinasiList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "HizmetBinasi güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new HizmetBinasiCreateRequestDto
                    {
                        HizmetBinasiAdi = FormModel.HizmetBinasiAdi.Trim(),
                        HizmetBinasiAktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _hizmetBinasiService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message ?? "HizmetBinasi başarıyla oluşturuldu!");
                        NavigateToHizmetBinasiList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "HizmetBinasi oluşturulamadı!");
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
                var result = await _hizmetBinasiService.DeleteAsync(Id!.Value);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "HizmetBinasi başarıyla silindi!");
                    NavigateToHizmetBinasiList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "HizmetBinasi silinemedi!");
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

        private void NavigateToHizmetBinasiList() => _navigationManager.NavigateTo("/common/hizmetbinasi");

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        // ═══════════════════════════════════════════════════════
        // FORM MODEL
        // ═══════════════════════════════════════════════════════

        public class HizmetBinasiFormModel
        {
            [Required(ErrorMessage = "HizmetBinasi adı zorunludur")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "HizmetBinasi adı 2-100 karakter arasında olmalıdır")]
            public string HizmetBinasiAdi { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;
        }
    }
}
