using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Models.FormModels;
using SGKPortalApp.PresentationLayer.Services.ApiServices;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Departman
{
    public partial class Manage : ComponentBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════
        private readonly IDepartmanApiService _departmanService;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;
        private readonly IMapper _mapper;

        public Manage(
            IDepartmanApiService departmanService,
            NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            IMapper mapper)
        {
            _departmanService = departmanService;
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
            _mapper = mapper;
        }


        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// URL'den gelen Departman ID (düzenleme modu için)
        /// </summary>
        [Parameter]
        public int? Id { get; set; }

        /// <summary>
        /// Form verilerini tutan model
        /// </summary>
        private DepartmanFormModel FormModel { get; set; } = new();

        /// <summary>
        /// Düzenleme modunda mı? (ID varsa true)
        /// </summary>
        private bool IsEditMode => Id.HasValue && Id.Value > 0;

        /// <summary>
        /// Sayfa başlığı (dinamik)
        /// </summary>
        private string PageTitle => IsEditMode ? "Departman Düzenle" : "Yeni Departman Ekle";

        /// <summary>
        /// Buton metni (dinamik)
        /// </summary>
        private string ButtonText => IsEditMode ? "Güncelle" : "Kaydet";

        /// <summary>
        /// Veri yükleniyor mu?
        /// </summary>
        private bool IsLoading { get; set; }

        /// <summary>
        /// Form kaydediliyor mu?
        /// </summary>
        private bool IsSaving { get; set; }


        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            // Düzenleme modundaysa mevcut departmanı yükle
            if (IsEditMode)
            {
                await LoadDepartman();
            }
        }


        // ═══════════════════════════════════════════════════════
        // PRIVATE METHODS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Mevcut departmanı API'den yükle (düzenleme modu için)
        /// </summary>
        private async Task LoadDepartman()
        {
            IsLoading = true;

            try
            {
                var departman = await _departmanService.GetByIdAsync(Id!.Value);

                if (departman != null)
                {
                    // ✅ AUTOMAPPER - Response DTO → Form Model
                    FormModel = _mapper.Map<DepartmanFormModel>(departman);
                }
                else
                {
                    await ShowToast("Departman bulunamadı!", "error");
                    NavigateBack();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Departman yükleme hatası: {ex.Message}");
                await ShowToast("Departman yüklenirken bir hata oluştu!", "error");
                NavigateBack();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Form submit işlemi - Create veya Update
        /// </summary>
        private async Task HandleSubmit()
        {
            IsSaving = true;

            try
            {
                if (IsEditMode)
                {
                    // ═══════════════════════════════════════════
                    // GÜNCELLEME
                    // ═══════════════════════════════════════════

                    // ✅ AUTOMAPPER - Form Model → Update Request DTO
                    var updateDto = _mapper.Map<DepartmanUpdateRequestDto>(FormModel);

                    var result = await _departmanService.UpdateAsync(Id!.Value, updateDto);

                    if (result != null)
                    {
                        await ShowToast("Departman başarıyla güncellendi", "success");
                        NavigateBack();
                    }
                    else
                    {
                        await ShowToast("Departman güncellenemedi!", "error");
                    }
                }
                else
                {
                    // ═══════════════════════════════════════════
                    // YENİ KAYIT
                    // ═══════════════════════════════════════════

                    // ✅ AUTOMAPPER - Form Model → Create Request DTO
                    var createDto = _mapper.Map<DepartmanCreateRequestDto>(FormModel);

                    var result = await _departmanService.CreateAsync(createDto);

                    if (result != null)
                    {
                        await ShowToast("Departman başarıyla eklendi", "success");
                        NavigateBack();
                    }
                    else
                    {
                        await ShowToast("Departman eklenemedi!", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Form submit hatası: {ex.Message}");
                await ShowToast("İşlem sırasında bir hata oluştu!", "error");
            }
            finally
            {
                IsSaving = false;
            }
        }

        /// <summary>
        /// Liste sayfasına geri dön
        /// </summary>
        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/personel/departman");
        }

        /// <summary>
        /// Toast bildirimi göster (geçici - ToastService eklenince değişecek)
        /// </summary>
        private async Task ShowToast(string message, string type)
        {
            await _jsRuntime.InvokeVoidAsync("console.log", $"[{type.ToUpper()}] {message}");
            // TODO: ToastService.Show(message, type) şeklinde değişecek
        }
    }
}