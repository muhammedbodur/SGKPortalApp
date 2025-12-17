using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Yetki.Islem
{
    public partial class Index
    {
        /// <summary>
        /// Permission Key: YET.YETKIISLEM.INDEX
        /// Route: /yetki-islem
        /// Convention: {MODUL_KODU}.{URL_TIRESIZ}.{ACTION}
        /// </summary>
        protected override string PagePermissionKey => "YET.YETKIISLEM.INDEX";
        [Inject] private IModulControllerIslemApiService IslemApiService { get; set; } = default!;
        [Inject] private IModulControllerApiService ControllerApiService { get; set; } = default!;
        [Inject] private IModulApiService ModulApiService { get; set; } = default!;
        [Inject] private IDtoDiscoveryApiService DtoDiscoveryApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        private List<ModulControllerIslemResponseDto> Islemler = new();
        private List<ModulControllerIslemResponseDto> FilteredIslemler = new();
        private List<ModulResponseDto> Moduller = new(); // ← ModulKodu için ModulResponseDto kullan
        private List<DropdownItemDto> FilteredControllers = new();
        private bool IsLoading = true;
        private bool IsSaving = false;
        private bool ShowModal = false;
        private int? EditingId = null;
        private int SelectedModulId = 0;
        private int SelectedControllerId = 0;
        private string IslemAdi = string.Empty;
        private string? Route = null;
        private string? PermissionKey = null;
        private SayfaTipi SelectedSayfaTipi = SayfaTipi.Protected;
        private YetkiSeviyesi SelectedMinYetki = YetkiSeviyesi.View;
        private YetkiIslemTipi SelectedIslemTipi = YetkiIslemTipi.Page;
        private int SelectedUstIslemId = 0;
        private string? DtoTypeName = null;
        private string? DtoFieldName = null;

        // DTO Discovery states
        private List<DtoTypeInfo> DtoTypes = new();
        private List<DtoPropertyInfo> DtoFields = new();
        private bool IsLoadingDtoTypes = false;
        private bool IsLoadingDtoFields = false;

        // Computed properties for conditional rendering
        private bool IsFieldType => SelectedIslemTipi == YetkiIslemTipi.Field || SelectedIslemTipi == YetkiIslemTipi.FormField;
        private bool ShowRouteField => SelectedIslemTipi == YetkiIslemTipi.Page;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadModulDropdown();
            await LoadDtoTypes();
            await LoadData();
        }

        private async Task LoadModulDropdown()
        {
            var result = await ModulApiService.GetAllAsync(); // ← ModulKodu'ya erişim için GetAllAsync kullan
            if (result.Success && result.Data != null)
                Moduller = result.Data;
        }

        private async Task OnModulChanged()
        {
            SelectedControllerId = 0;
            SelectedUstIslemId = 0;
            FilteredControllers = new();
            FilteredIslemler = new();

            if (SelectedModulId > 0)
            {
                var result = await ControllerApiService.GetDropdownByModulIdAsync(SelectedModulId);
                if (result.Success && result.Data != null)
                    FilteredControllers = result.Data;
            }
        }

        private void OnControllerChanged()
        {
            SelectedUstIslemId = 0;
            FilteredIslemler = Islemler
                .Where(i => i.ModulControllerId == SelectedControllerId && i.ModulControllerIslemId != EditingId)
                .OrderBy(i => i.ModulControllerIslemAdi)
                .ToList();

            GeneratePermissionKey();
        }

        private async Task OnDtoTypeChanged()
        {
            // DTO Type değiştiğinde field'ları temizle ve yeni DTO'nun field'larını yükle
            DtoFieldName = null;
            DtoFields = new();

            if (!string.IsNullOrEmpty(DtoTypeName))
            {
                await LoadDtoFields(DtoTypeName);
            }

            GeneratePermissionKey();
        }

        private void OnDtoFieldNameChanged()
        {
            // DTO Field Name seçildiğinde İşlem Adı'nı otomatik doldur (uppercase)
            if (!string.IsNullOrWhiteSpace(DtoFieldName) && string.IsNullOrWhiteSpace(IslemAdi))
            {
                // Nested field ise (Personel.DepartmanId) sadece son kısmı al (DepartmanId)
                var fieldName = DtoFieldName.Contains(".")
                    ? DtoFieldName.Split('.').Last()
                    : DtoFieldName;

                IslemAdi = fieldName.ToUpperInvariant();
            }
            GeneratePermissionKey();
        }

        private void OnIslemAdiChanged()
        {
            GeneratePermissionKey();
        }

        private void OnUstIslemChanged()
        {
            GeneratePermissionKey();
        }

        private void OnIslemTipiChanged()
        {
            // İşlem tipi değiştiğinde ilgili alanları temizle
            if (!IsFieldType)
            {
                DtoTypeName = null;
                DtoFieldName = null;
            }

            if (!ShowRouteField)
            {
                Route = null;
            }

            // İşlem tipine göre MinYetki varsayılan değerini ayarla
            SelectedMinYetki = SelectedIslemTipi switch
            {
                YetkiIslemTipi.Page => YetkiSeviyesi.View,      // Sayfa: Görünsün ama readonly
                YetkiIslemTipi.Tab => YetkiSeviyesi.Edit,       // Tab: Aktif olsun
                YetkiIslemTipi.Buton => YetkiSeviyesi.Edit,     // Buton: Tıklanabilsin
                YetkiIslemTipi.Field => YetkiSeviyesi.View,     // Field: Görünsün (Detail sayfası için)
                YetkiIslemTipi.FormField => YetkiSeviyesi.View, // FormField: Görünsün ama disabled
                _ => YetkiSeviyesi.View
            };

            GeneratePermissionKey();
        }

        private void GeneratePermissionKey()
        {
            // Permission Key'i otomatik oluştur
            if (SelectedModulId == 0 || SelectedControllerId == 0 || string.IsNullOrWhiteSpace(IslemAdi))
            {
                PermissionKey = null;
                return;
            }

            var modul = Moduller.FirstOrDefault(m => m.ModulId == SelectedModulId);
            var controller = FilteredControllers.FirstOrDefault(c => c.Id == SelectedControllerId);

            if (modul == null || controller == null)
            {
                PermissionKey = null;
                return;
            }

            // ModulKodu (örn: PER) - Artık doğrudan ModulKodu'dan alınıyor
            var modulKodu = modul.ModulKodu?.ToUpperInvariant() ?? "UNKNOWN";

            // Controller adı (örn: PERSONEL)
            var controllerAdi = controller.Ad?.ToUpperInvariant() ?? "UNKNOWN";

            // İşlem adı (zaten uppercase)
            var islemAdi = IslemAdi.Trim().ToUpperInvariant();

            // İşlem tipine göre prefix belirle
            string tipPrefix = SelectedIslemTipi switch
            {
                YetkiIslemTipi.Field => "FIELD",           // Detail sayfası salt okunur field
                YetkiIslemTipi.FormField => "FORMFIELD",   // Manage sayfası form alanı
                YetkiIslemTipi.Buton => "ACTION",
                YetkiIslemTipi.Tab => "TAB",
                YetkiIslemTipi.Page => "", // Page için prefix yok
                _ => ""
            };

            // Üst işlem varsa onun adını al
            if (SelectedUstIslemId > 0)
            {
                var ustIslem = FilteredIslemler.FirstOrDefault(i => i.ModulControllerIslemId == SelectedUstIslemId);
                if (ustIslem != null)
                {
                    // Üst işlemin adını al
                    var ustIslemAdi = ustIslem.ModulControllerIslemAdi?.ToUpperInvariant() ?? "";

                    // Tip prefix varsa ekle
                    if (!string.IsNullOrEmpty(tipPrefix))
                        PermissionKey = $"{modulKodu}.{controllerAdi}.{ustIslemAdi}.{tipPrefix}.{islemAdi}";
                    else
                        PermissionKey = $"{modulKodu}.{controllerAdi}.{ustIslemAdi}.{islemAdi}";
                    return;
                }
            }

            // Üst işlem yoksa direkt oluştur
            if (!string.IsNullOrEmpty(tipPrefix))
                PermissionKey = $"{modulKodu}.{controllerAdi}.{tipPrefix}.{islemAdi}";
            else
                PermissionKey = $"{modulKodu}.{controllerAdi}.{islemAdi}";
        }

        private string GetIslemPrefix(ModulControllerIslemResponseDto islem)
        {
            return islem.IslemTipi switch
            {
                YetkiIslemTipi.Page => "📄",
                YetkiIslemTipi.Tab => "📑",
                YetkiIslemTipi.Buton => "🔘",
                YetkiIslemTipi.Field => "📝",
                YetkiIslemTipi.FormField => "✏️",
                _ => "•"
            };
        }

        private async Task LoadData()
        {
            IsLoading = true;
            var result = await IslemApiService.GetAllAsync();
            if (result.Success && result.Data != null)
                Islemler = result.Data;
            IsLoading = false;
        }

        private async Task LoadDtoTypes()
        {
            IsLoadingDtoTypes = true;
            try
            {
                var result = await DtoDiscoveryApiService.GetAllDtoTypesAsync();
                if (result.Success && result.Data != null)
                {
                    DtoTypes = result.Data;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "DTO'lar yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"DTO yükleme hatası: {ex.Message}");
            }
            finally
            {
                IsLoadingDtoTypes = false;
            }
        }

        private async Task LoadDtoFields(string dtoTypeName)
        {
            IsLoadingDtoFields = true;
            try
            {
                var result = await DtoDiscoveryApiService.GetDtoPropertiesAsync(dtoTypeName);
                if (result.Success && result.Data != null)
                {
                    DtoFields = result.Data;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Field'lar yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Field yükleme hatası: {ex.Message}");
            }
            finally
            {
                IsLoadingDtoFields = false;
            }
        }

        private void ShowAddModal()
        {
            EditingId = null;
            SelectedModulId = 0;
            SelectedControllerId = 0;
            SelectedUstIslemId = 0;
            FilteredControllers = new();
            FilteredIslemler = new();
            IslemAdi = string.Empty;
            Route = null;
            PermissionKey = null;
            SelectedSayfaTipi = SayfaTipi.Protected;
            SelectedMinYetki = YetkiSeviyesi.View;
            SelectedIslemTipi = YetkiIslemTipi.Page;
            DtoTypeName = null;
            DtoFieldName = null;
            ShowModal = true;
        }

        private async Task StartEdit(ModulControllerIslemResponseDto i)
        {
            EditingId = i.ModulControllerIslemId;
            SelectedModulId = i.ModulId;
            await OnModulChanged();
            SelectedControllerId = i.ModulControllerId;
            OnControllerChanged();
            IslemAdi = i.ModulControllerIslemAdi;
            Route = i.Route;
            PermissionKey = i.PermissionKey;
            SelectedSayfaTipi = i.SayfaTipi;
            SelectedMinYetki = i.MinYetkiSeviyesi;
            SelectedIslemTipi = i.IslemTipi;
            SelectedUstIslemId = i.UstIslemId ?? 0;
            DtoTypeName = i.DtoTypeName;
            DtoFieldName = i.DtoFieldName;

            // DTO Type varsa field'ları yükle
            if (!string.IsNullOrEmpty(DtoTypeName))
            {
                await LoadDtoFields(DtoTypeName);
            }

            ShowModal = true;
        }

        private void CloseModal()
        {
            ShowModal = false;
            EditingId = null;
        }

        private async Task SaveIslem()
        {
            if (SelectedControllerId <= 0 || string.IsNullOrWhiteSpace(IslemAdi))
            {
                await ToastService.ShowWarningAsync("Controller ve İşlem adı zorunludur");
                return;
            }

            if (string.IsNullOrWhiteSpace(PermissionKey))
            {
                await ToastService.ShowWarningAsync("Permission Key otomatik oluşturulamadı. Lütfen gerekli alanları doldurun.");
                return;
            }

            // Field tipi için DTO Field Name zorunlu
            if (IsFieldType && string.IsNullOrWhiteSpace(DtoFieldName))
            {
                await ToastService.ShowWarningAsync("Field tipi için DTO Field Name zorunludur");
                return;
            }

            IsSaving = true;
            StateHasChanged();

            if (EditingId.HasValue)
            {
                var request = new ModulControllerIslemUpdateRequestDto
                {
                    ModulControllerId = SelectedControllerId,
                    ModulControllerIslemAdi = IslemAdi.Trim(),
                    IslemTipi = SelectedIslemTipi,
                    UstIslemId = SelectedUstIslemId > 0 ? SelectedUstIslemId : null,
                    Route = string.IsNullOrWhiteSpace(Route) ? null : Route.Trim(),
                    PermissionKey = string.IsNullOrWhiteSpace(PermissionKey) ? null : PermissionKey.Trim(),
                    SayfaTipi = SelectedSayfaTipi,
                    MinYetkiSeviyesi = SelectedMinYetki,
                    DtoTypeName = string.IsNullOrWhiteSpace(DtoTypeName) ? null : DtoTypeName.Trim(),
                    DtoFieldName = string.IsNullOrWhiteSpace(DtoFieldName) ? null : DtoFieldName.Trim()
                };
                var result = await IslemApiService.UpdateAsync(EditingId.Value, request);
                if (result.Success)
                    await ToastService.ShowSuccessAsync(result.Message ?? "İşlem güncellendi");
                else
                    await ToastService.ShowErrorAsync(result.Message ?? "İşlem güncellenemedi");
            }
            else
            {
                var request = new ModulControllerIslemCreateRequestDto
                {
                    ModulControllerId = SelectedControllerId,
                    ModulControllerIslemAdi = IslemAdi.Trim(),
                    IslemTipi = SelectedIslemTipi,
                    UstIslemId = SelectedUstIslemId > 0 ? SelectedUstIslemId : null,
                    Route = string.IsNullOrWhiteSpace(Route) ? null : Route.Trim(),
                    PermissionKey = string.IsNullOrWhiteSpace(PermissionKey) ? null : PermissionKey.Trim(),
                    SayfaTipi = SelectedSayfaTipi,
                    MinYetkiSeviyesi = SelectedMinYetki,
                    DtoTypeName = string.IsNullOrWhiteSpace(DtoTypeName) ? null : DtoTypeName.Trim(),
                    DtoFieldName = string.IsNullOrWhiteSpace(DtoFieldName) ? null : DtoFieldName.Trim()
                };
                var result = await IslemApiService.CreateAsync(request);
                if (result.Success)
                    await ToastService.ShowSuccessAsync(result.Message ?? "İşlem eklendi");
                else
                    await ToastService.ShowErrorAsync(result.Message ?? "İşlem eklenemedi");
            }

            IsSaving = false;
            CloseModal();
            await LoadData();
        }

        private async Task DeleteIslem(ModulControllerIslemResponseDto i)
        {
            var confirmed = await JS.InvokeAsync<bool>("confirm", $"'{i.ModulControllerIslemAdi}' silinsin mi?");
            if (!confirmed) return;

            var result = await IslemApiService.DeleteAsync(i.ModulControllerIslemId);
            if (result.Success)
                await ToastService.ShowSuccessAsync(result.Message ?? "İşlem silindi");
            else
                await ToastService.ShowErrorAsync(result.Message ?? "İşlem silinemedi");
            await LoadData();
        }
    }
}
