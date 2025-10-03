using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Models.FormModels;
using SGKPortalApp.PresentationLayer.Services.ApiServices;
using SGKPortalApp.PresentationLayer.Services.UIServices; // ✅ Toast için

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
        [Inject] private IMapper _mapper { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        private DepartmanFormModel FormModel { get; set; } = new();
        private bool IsEditMode => Id.HasValue && Id.Value > 0;
        private string PageTitle => IsEditMode ? "Departman Düzenle" : "Yeni Departman Ekle";
        private string ButtonText => IsEditMode ? "Güncelle" : "Kaydet";
        private bool IsLoading { get; set; }
        private bool IsSaving { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (IsEditMode)
            {
                await LoadDepartman();
            }
        }

        private async Task LoadDepartman()
        {
            IsLoading = true;
            try
            {
                var departman = await _departmanService.GetByIdAsync(Id!.Value);

                if (departman != null)
                {
                    FormModel = _mapper.Map<DepartmanFormModel>(departman);
                }
                else
                {
                    await _toastService.ShowErrorAsync("Departman bulunamadı!"); // ✅
                    NavigateBack();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("Departman yüklenirken bir hata oluştu!"); // ✅
                NavigateBack();
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
                    var updateDto = _mapper.Map<DepartmanUpdateRequestDto>(FormModel);
                    var result = await _departmanService.UpdateAsync(Id!.Value, updateDto);

                    if (result != null)
                    {
                        await _toastService.ShowSuccessAsync("Departman başarıyla güncellendi"); // ✅
                        NavigateBack();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync("Departman güncellenemedi!"); // ✅
                    }
                }
                else
                {
                    var createDto = _mapper.Map<DepartmanCreateRequestDto>(FormModel);
                    var result = await _departmanService.CreateAsync(createDto);

                    if (result != null)
                    {
                        await _toastService.ShowSuccessAsync("Departman başarıyla eklendi"); // ✅
                        NavigateBack();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync("Departman eklenemedi!"); // ✅
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                await _toastService.ShowErrorAsync("İşlem sırasında bir hata oluştu!"); // ✅
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/personel/departman");
        }
    }
}