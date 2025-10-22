using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Pages.Common.HizmetBinasi
{
    public partial class Manage : ComponentBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
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

        private HizmetBinasiFormModel FormModel { get; set; } = new();
        private List<DepartmanResponseDto> Departmanlar { get; set; } = new();
        private List<string> validationErrors = new();

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
            await LoadDepartmanlar();
            await LoadData();
        }

        // Departmanları yükleme kısmı
        private async Task LoadDepartmanlar()
        {
            try
            {
                var result = await _departmanService.GetAllAsync();

                // ServiceResult içindeki Data'ya erişin
                if (result.Success && result.Data != null && result.Data.Any())
                {
                    // Sadece aktif departmanları al
                    Departmanlar = result.Data
                        .Where(d => d.DepartmanAktiflik == Aktiflik.Aktif)
                        .OrderBy(d => d.DepartmanAdi)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Departman yükleme hatası: {ex.Message}");
                await _toastService.ShowWarningAsync("Departmanlar yüklenemedi!");
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
                        await _toastService.ShowErrorAsync(result.Message
                            ?? "Hizmet Binası bulunamadı!");
                    }
                    else
                    {
                        var hizmetBinasi = result.Data;
                        FormModel = new HizmetBinasiFormModel
                        {
                            HizmetBinasiAdi = hizmetBinasi.HizmetBinasiAdi,
                            DepartmanId = hizmetBinasi.DepartmanId,
                            Adres = hizmetBinasi.Adres,
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
                        DepartmanId = 0,
                        Adres = string.Empty,
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
            // Validation kontrolü
            validationErrors.Clear();

            if (string.IsNullOrWhiteSpace(FormModel.HizmetBinasiAdi))
            {
                validationErrors.Add("Hizmet Binası adı zorunludur!");
            }

            if (FormModel.DepartmanId == 0)
            {
                validationErrors.Add("Departman seçimi zorunludur!");
            }

            if (validationErrors.Any())
            {
                await _toastService.ShowWarningAsync("Lütfen tüm zorunlu alanları doldurun!");
                return;
            }

            IsSaving = true;

            try
            {
                if (IsEditMode)
                {
                    var updateDto = new HizmetBinasiUpdateRequestDto
                    {
                        HizmetBinasiAdi = FormModel.HizmetBinasiAdi.Trim(),
                        DepartmanId = FormModel.DepartmanId,
                        Adres = FormModel.Adres?.Trim(),
                        HizmetBinasiAktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _hizmetBinasiService.UpdateAsync(Id!.Value, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message
                            ?? "Hizmet Binası başarıyla güncellendi!");
                        NavigateToHizmetBinasiList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message
                            ?? "Hizmet Binası güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new HizmetBinasiCreateRequestDto
                    {
                        HizmetBinasiAdi = FormModel.HizmetBinasiAdi.Trim(),
                        DepartmanId = FormModel.DepartmanId,
                        Adres = FormModel.Adres?.Trim(),
                        HizmetBinasiAktiflik = FormModel.IsActive ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _hizmetBinasiService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync(result.Message
                            ?? "Hizmet Binası başarıyla oluşturuldu!");
                        NavigateToHizmetBinasiList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message
                            ?? "Hizmet Binası oluşturulamadı!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Kayıt işlemi sırasında bir hata oluştu!");
            }
            finally
            {
                IsSaving = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // DELETE MODAL
        // ═══════════════════════════════════════════════════════

        private void ShowDeleteConfirmation()
        {
            if (CurrentPersonelSayisi > 0)
            {
                _toastService.ShowWarningAsync(
                    $"Bu hizmet binasında {CurrentPersonelSayisi} personel bulunmaktadır. " +
                    "Silmeden önce personelleri başka bir hizmet binasına taşıyın.");
                return;
            }
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
                    await _toastService.ShowSuccessAsync(result.Message
                        ?? "Hizmet Binası başarıyla silindi!");
                    NavigateToHizmetBinasiList();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message
                        ?? "Hizmet Binası silinemedi!");
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

        private void NavigateToHizmetBinasiList()
            => _navigationManager.NavigateTo("/common/hizmetbinasi");

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }
    }
}