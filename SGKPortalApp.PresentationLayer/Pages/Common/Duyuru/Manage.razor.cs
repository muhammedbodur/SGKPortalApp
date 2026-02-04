using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.Duyuru
{
    public partial class Manage
    {
        [Inject] private IDuyuruApiService _duyuruService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        private DuyuruFormModel FormModel { get; set; } = new();

        private bool IsEditMode => Id.HasValue && Id.Value > 0;
        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private bool IsUploading { get; set; } = false;
        private bool NotFound { get; set; } = false;

        private bool ShowDeleteModal { get; set; } = false;
        private bool IsDeleting { get; set; } = false;

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
                    var result = await _duyuruService.GetByIdAsync(Id!.Value);

                    if (!result.Success || result.Data == null)
                    {
                        NotFound = true;
                        await _toastService.ShowErrorAsync(result.Message ?? "Duyuru bulunamadı!");
                    }
                    else
                    {
                        var duyuru = result.Data;
                        FormModel = new DuyuruFormModel
                        {
                            Baslik = duyuru.Baslik,
                            Icerik = duyuru.Icerik,
                            GorselUrl = duyuru.GorselUrl,
                            Sira = duyuru.Sira,
                            YayinTarihi = duyuru.YayinTarihi,
                            BitisTarihi = duyuru.BitisTarihi,
                            Aktiflik = duyuru.Aktiflik
                        };
                    }
                }
                else
                {
                    FormModel = new DuyuruFormModel
                    {
                        Baslik = string.Empty,
                        Icerik = string.Empty,
                        Sira = 1,
                        YayinTarihi = DateTime.Now,
                        Aktiflik = Aktiflik.Aktif
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
            if (string.IsNullOrWhiteSpace(FormModel.Baslik))
            {
                await _toastService.ShowWarningAsync("Başlık zorunludur!");
                return;
            }

            if (string.IsNullOrWhiteSpace(FormModel.Icerik))
            {
                await _toastService.ShowWarningAsync("İçerik zorunludur!");
                return;
            }

            IsSaving = true;

            try
            {
                if (IsEditMode)
                {
                    var updateDto = new DuyuruUpdateRequestDto
                    {
                        Baslik = FormModel.Baslik.Trim(),
                        Icerik = FormModel.Icerik.Trim(),
                        GorselUrl = FormModel.GorselUrl?.Trim(),
                        Sira = FormModel.Sira,
                        YayinTarihi = FormModel.YayinTarihi,
                        BitisTarihi = FormModel.BitisTarihi,
                        Aktiflik = FormModel.Aktiflik
                    };

                    var result = await _duyuruService.UpdateAsync(Id!.Value, updateDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Duyuru başarıyla güncellendi!");
                        _navigationManager.NavigateTo("/common/duyuru");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Duyuru güncellenemedi!");
                    }
                }
                else
                {
                    var createDto = new DuyuruCreateRequestDto
                    {
                        Baslik = FormModel.Baslik.Trim(),
                        Icerik = FormModel.Icerik.Trim(),
                        GorselUrl = FormModel.GorselUrl?.Trim(),
                        Sira = FormModel.Sira,
                        YayinTarihi = FormModel.YayinTarihi,
                        BitisTarihi = FormModel.BitisTarihi,
                        Aktiflik = FormModel.Aktiflik
                    };

                    var result = await _duyuruService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Duyuru başarıyla oluşturuldu!");
                        _navigationManager.NavigateTo("/common/duyuru");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Duyuru oluşturulamadı!");
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

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;

            if (file == null)
                return;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.Name).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                await _toastService.ShowErrorAsync("Geçersiz dosya türü. Sadece JPG, PNG, GIF ve WebP dosyaları yüklenebilir.");
                return;
            }

            if (file.Size > 5 * 1024 * 1024)
            {
                await _toastService.ShowErrorAsync("Dosya boyutu 5MB'dan büyük olamaz.");
                return;
            }

            IsUploading = true;
            StateHasChanged();

            try
            {
                using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
                var result = await _duyuruService.UploadImageAsync(stream, file.Name);

                if (result.Success && !string.IsNullOrEmpty(result.Data))
                {
                    FormModel.GorselUrl = result.Data;
                    await _toastService.ShowSuccessAsync("Görsel başarıyla yüklendi!");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Görsel yüklenemedi!");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Görsel yükleme hatası: {ex.Message}");
            }
            finally
            {
                IsUploading = false;
                StateHasChanged();
            }
        }

        private void RemoveImage()
        {
            FormModel.GorselUrl = null;
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
                var result = await _duyuruService.DeleteAsync(Id!.Value);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Duyuru başarıyla silindi!");
                    _navigationManager.NavigateTo("/common/duyuru");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Duyuru silinemedi!");
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

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        private void NavigateToDuyuruList()
        {
            _navigationManager.NavigateTo("/common/duyuru");
        }
    }
}
