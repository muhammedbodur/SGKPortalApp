using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
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
        /// Route: /yetki-islem
        /// Convention: {MODUL_KODU}.{URL_TIRESIZ}.{ACTION}
        /// </summary>
        [Inject] private IModulControllerIslemApiService IslemApiService { get; set; } = default!;
        [Inject] private IModulControllerApiService ControllerApiService { get; set; } = default!;
        [Inject] private IModulApiService ModulApiService { get; set; } = default!;
        [Inject] private IDtoDiscoveryApiService DtoDiscoveryApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        private List<ModulControllerIslemResponseDto> Islemler = new();
        private List<ModulControllerIslemResponseDto> FilteredIslemler = new();
        private List<ModulControllerIslemResponseDto> FilteredIslemlerDisplay = new();
        private List<ModulResponseDto> Moduller = new();
        private List<DropdownItemDto> FilteredControllers = new();
        private List<DropdownItemDto> AllControllers = new();
        private bool IsLoading = true;
        private bool IsSaving = false;
        private bool ShowModal = false;
        private int? EditingId = null;

        private string SearchTerm = string.Empty;
        private int FilterModulId = 0;
        private int FilterControllerId = 0;
        private int FilterIslemTipi = -1;
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

        // ActionType states (Buton için)
        private List<DropdownDto> ActionTypes = new();
        private bool IsLoadingActionTypes = false;
        private string? SelectedActionType = null;

        // Computed properties for conditional rendering
        private bool IsFieldType => SelectedIslemTipi == YetkiIslemTipi.Field || SelectedIslemTipi == YetkiIslemTipi.FormField;
        private bool ShowRouteField => SelectedIslemTipi == YetkiIslemTipi.Page;
        private bool IsButonType => SelectedIslemTipi == YetkiIslemTipi.Buton;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadModulDropdown();
            await LoadAllControllers();
            await LoadDtoTypes();
            await LoadActionTypes();
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
            // Üst işlem dropdown'ında sadece Page tipindeki işlemler gösterilir
            FilteredIslemler = Islemler
                .Where(i => i.ModulControllerId == SelectedControllerId
                    && i.ModulControllerIslemId != EditingId
                    && i.IslemTipi == YetkiIslemTipi.Page) // Sadece Page tipi
                .OrderBy(i => i.ModulControllerIslemAdi)
                .ToList();

            // Sayfa tipi için Route ve İşlem Adı otomatik doldur
            AutoPopulateRouteAndIslemAdi();

            GeneratePermissionKey();
        }

        /// <summary>
        /// Controller seçildiğinde Route alanını temizle (kullanıcı manuel dolduracak)
        /// </summary>
        private void AutoPopulateRouteAndIslemAdi()
        {
            // Route alanını boş bırak, kullanıcı manuel dolduracak
            // İşlem Adı da boş kalacak, Route'tan otomatik çıkarılacak
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

        /// <summary>
        /// Route değiştiğinde çağrılır
        /// 1) Controller hiyerarşisi ile Route'u karşılaştırır
        /// 2) Route'tan sayfa adını çıkarıp İşlem Adı'na yazar
        /// </summary>
        private async Task OnRouteChanged()
        {
            if (string.IsNullOrWhiteSpace(Route) || SelectedControllerId <= 0)
            {
                GeneratePermissionKey();
                return;
            }

            var controller = FilteredControllers.FirstOrDefault(c => c.Id == SelectedControllerId);
            var modul = Moduller.FirstOrDefault(m => m.ModulId == SelectedModulId);
            
            if (controller == null || modul == null)
            {
                GeneratePermissionKey();
                return;
            }

            // Route'u temizle - localhost:8080 gibi URL kısımlarını kaldır
            var cleanedRoute = Route.Trim();
            
            // URL formatında girildiyse sadece path kısmını al
            if (cleanedRoute.Contains("://"))
            {
                try
                {
                    var uri = new Uri(cleanedRoute);
                    cleanedRoute = uri.PathAndQuery;
                    Route = cleanedRoute; // Temizlenmiş halini geri yaz
                }
                catch
                {
                    // URI parse edilemezse olduğu gibi devam et
                }
            }

            // Route'u normalize et
            var routeNormalized = cleanedRoute.Trim().TrimStart('/').ToLowerInvariant();
            
            // FullPath'i al (Metadata'dan)
            string fullPathNormalized = "";
            if (!string.IsNullOrWhiteSpace(controller.Metadata))
            {
                fullPathNormalized = controller.Metadata.Trim().TrimStart('/').ToLowerInvariant();
            }

            // Route'tan İşlem Adı'nı otomatik çıkar
            // FullPath ile karşılaştırarak controller kısmını ayır
            // Örnek: FullPath="/Auth/Login", Route="/auth/login" → İşlem Adı: INDEX (route tam eşleşiyor)
            // Örnek: FullPath="/Siramatik/Banko", Route="/siramatik/banko/personel-atama" → İşlem Adı: PERSONEL-ATAMA
            
            if (!string.IsNullOrWhiteSpace(fullPathNormalized) && routeNormalized.StartsWith(fullPathNormalized))
            {
                // Route, FullPath ile başlıyorsa, kalan kısmı sayfa adı olarak al
                var remainingPath = routeNormalized.Substring(fullPathNormalized.Length).TrimStart('/');
                
                if (string.IsNullOrWhiteSpace(remainingPath))
                {
                    // Route tam olarak FullPath'e eşitse, varsayılan INDEX
                    IslemAdi = "INDEX";
                }
                else
                {
                    // Kalan kısım sayfa adı
                    var pageName = remainingPath.Split('/')[0].ToUpperInvariant();
                    IslemAdi = pageName;
                }
            }
            else
            {
                // FullPath yoksa veya eşleşmiyorsa, route'un son kısmını al
                var routeParts = routeNormalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (routeParts.Length > 0)
                {
                    IslemAdi = routeParts[^1].ToUpperInvariant();
                }
                else
                {
                    IslemAdi = "INDEX";
                }
            }

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

            // Page dışı tipler için üst işlem seçimi temizlenir (zorunlu olacak)
            if (SelectedIslemTipi != YetkiIslemTipi.Page)
            {
                // Eğer mevcut üst işlem Page değilse temizle
                if (SelectedUstIslemId > 0)
                {
                    var ustIslem = FilteredIslemler.FirstOrDefault(i => i.ModulControllerIslemId == SelectedUstIslemId);
                    if (ustIslem?.IslemTipi != YetkiIslemTipi.Page)
                    {
                        SelectedUstIslemId = 0;
                    }
                }
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

            // Buton tipi değiştiğinde ActionType temizle
            if (!IsButonType)
            {
                SelectedActionType = null;
            }

            GeneratePermissionKey();
        }

        private void OnActionTypeChanged()
        {
            // ActionType seçildiğinde İşlem Adı'nı otomatik doldur
            if (!string.IsNullOrWhiteSpace(SelectedActionType))
            {
                IslemAdi = SelectedActionType.ToUpperInvariant();
            }
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

            // Controller adı - TÜM hiyerarşiyi dahil et (FullPath'ten al)
            // Metadata'dan FullPath: "/Siramatik/Banko" → "SIRAMATIK.BANKO"
            string controllerKeyPath;
            
            if (!string.IsNullOrWhiteSpace(controller.Metadata))
            {
                // FullPath varsa onu kullan (recursive query'den geliyor)
                var fullPath = controller.Metadata.Trim().TrimStart('/').ToUpperInvariant();
                var controllerParts = fullPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                controllerKeyPath = string.Join(".", controllerParts);
            }
            else
            {
                // FullPath yoksa Ad alanından çıkar (fallback)
                var controllerPath = controller.Ad?.ToUpperInvariant() ?? "UNKNOWN";
                var controllerParts = controllerPath.Split(new[] { " > " }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToArray();
                controllerKeyPath = string.Join(".", controllerParts);
            }

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
                        PermissionKey = $"{controllerKeyPath}.{ustIslemAdi}.{tipPrefix}.{islemAdi}";
                    else
                        PermissionKey = $"{controllerKeyPath}.{ustIslemAdi}.{islemAdi}";
                    return;
                }
            }

            // Üst işlem yoksa direkt oluştur
            if (!string.IsNullOrEmpty(tipPrefix))
                PermissionKey = $"{controllerKeyPath}.{tipPrefix}.{islemAdi}";
            else
                PermissionKey = $"{controllerKeyPath}.{islemAdi}";
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

        private async Task LoadAllControllers()
        {
            var result = await ControllerApiService.GetDropdownAsync();
            if (result.Success && result.Data != null)
                AllControllers = result.Data;
        }

        private async Task LoadData()
        {
            IsLoading = true;
            var result = await IslemApiService.GetAllAsync();
            if (result.Success && result.Data != null)
            {
                Islemler = BuildHierarchicalList(result.Data);
                ApplyFilter();
            }
            IsLoading = false;
        }

        /// <summary>
        /// İşlemleri hiyerarşik sıraya koyar (Page > Alt işlemler)
        /// YetkiAtama sayfasındaki gibi üst işlem altında alt işlemler gösterilir
        /// </summary>
        private List<ModulControllerIslemResponseDto> BuildHierarchicalList(List<ModulControllerIslemResponseDto> items)
        {
            var result = new List<ModulControllerIslemResponseDto>();

            // Modül ve Controller bazında grupla
            var grouped = items
                .GroupBy(i => new { i.ModulId, i.ModulAdi, i.ModulControllerId, i.ModulControllerAdi })
                .OrderBy(g => g.Key.ModulAdi)
                .ThenBy(g => g.Key.ModulControllerAdi);

            foreach (var group in grouped)
            {
                // Önce kök seviyedeki Page'leri ekle (UstIslemId yok)
                var rootPages = group
                    .Where(i => i.IslemTipi == YetkiIslemTipi.Page && !i.UstIslemId.HasValue)
                    .OrderBy(i => i.ModulControllerIslemAdi)
                    .ToList();

                foreach (var rootPage in rootPages)
                {
                    result.Add(rootPage);

                    // Bu Page'in altındaki işlemleri ekle (recursive değil, sadece 1 seviye)
                    var children = group
                        .Where(i => i.UstIslemId == rootPage.ModulControllerIslemId)
                        .OrderBy(i => i.IslemTipi) // Page, Tab, Buton, Field, FormField sırası
                        .ThenBy(i => i.ModulControllerIslemAdi)
                        .ToList();

                    result.AddRange(children);
                }

                // Üst işlemi olan ama üst işlemi bulunamayan işlemler (yanlış veri)
                var orphans = group
                    .Where(i => i.UstIslemId.HasValue && !result.Any(r => r.ModulControllerIslemId == i.UstIslemId))
                    .OrderBy(i => i.ModulControllerIslemAdi)
                    .ToList();

                result.AddRange(orphans);

                // Hiç üst işlem ilişkisi olmayan non-Page işlemler (yanlış veri)
                var standalone = group
                    .Where(i => i.IslemTipi != YetkiIslemTipi.Page && !i.UstIslemId.HasValue && !result.Contains(i))
                    .OrderBy(i => i.ModulControllerIslemAdi)
                    .ToList();

                result.AddRange(standalone);
            }

            return result;
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

        private async Task LoadActionTypes()
        {
            IsLoadingActionTypes = true;
            try
            {
                var result = await IslemApiService.GetActionTypesAsync();
                if (result.Success && result.Data != null)
                {
                    ActionTypes = result.Data;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "ActionType listesi yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"ActionType yükleme hatası: {ex.Message}");
            }
            finally
            {
                IsLoadingActionTypes = false;
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
            StateHasChanged();
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

            // Page dışı tipler için üst işlem zorunlu
            if (SelectedIslemTipi != YetkiIslemTipi.Page && SelectedUstIslemId <= 0)
            {
                await ToastService.ShowWarningAsync("Tab, Buton, Field ve FormField tipleri için üst işlem (Page) seçimi zorunludur");
                return;
            }

            // Üst işlem seçilmişse, Page tipinde olmalı
            if (SelectedUstIslemId > 0)
            {
                var ustIslem = FilteredIslemler.FirstOrDefault(i => i.ModulControllerIslemId == SelectedUstIslemId);
                if (ustIslem == null || ustIslem.IslemTipi != YetkiIslemTipi.Page)
                {
                    await ToastService.ShowWarningAsync("Üst işlem sadece Page tipinde olabilir");
                    return;
                }
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

        private void ApplyFilter()
        {
            var query = Islemler.AsEnumerable();

            // Search
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                query = query.Where(i =>
                    (i.ModulControllerIslemAdi?.ToLower().Contains(searchLower) ?? false));
            }

            // Filter by Modul
            if (FilterModulId > 0)
            {
                query = query.Where(i => i.ModulId == FilterModulId);
            }

            // Filter by Controller
            if (FilterControllerId > 0)
            {
                query = query.Where(i => i.ModulControllerId == FilterControllerId);
            }

            // Filter by IslemTipi
            if (FilterIslemTipi >= 0)
            {
                query = query.Where(i => (int)i.IslemTipi == FilterIslemTipi);
            }

            FilteredIslemlerDisplay = query.ToList();
            StateHasChanged();
        }
    }
}
