using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Yetki.Controller
{
    public partial class Index
    {
        // ═══════════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════════

        [Inject] private IModulControllerApiService ControllerApiService { get; set; } = default!;
        [Inject] private IModulApiService ModulApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════════
        // FIELD PERMISSION CONFIGURATION (FieldPermissionPageBase)
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Permission Key: YET.YETKICONTROLLER.INDEX
        /// Route: /yetki/controller
        /// Convention: {MODUL_KODU}.{URL_TIRESIZ}.{ACTION}
        /// </summary>

        // ═══════════════════════════════════════════════════════════
        // STATE
        // ═══════════════════════════════════════════════════════════

        private List<ModulControllerResponseDto> Controllers = new();
        private List<ModulControllerResponseDto> FilteredControllers = new();
        private List<ModulControllerResponseDto> HierarchicalControllers = new(); // Hiyerarşik sıralı liste
        private List<DropdownItemDto> Moduller = new();
        private List<DropdownItemDto> ParentControllers = new(); // Üst controller seçimi için
        private bool IsLoading = true;
        private bool IsSaving = false;
        private bool ShowModal = false;
        private int? EditingId = null;
        private int SelectedModulId = 0;
        private int? SelectedUstModulControllerId = null; // Hiyerarşik yapı için
        private string ControllerAdi = string.Empty;

        private string SearchTerm = string.Empty;
        private int FilterModulId = 0;
        private string SortBy = "ControllerAdi";
        private string SortDirection = "asc";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadModulDropdown();
            await LoadData();
        }

        private async Task LoadModulDropdown()
        {
            var result = await ModulApiService.GetDropdownAsync();
            if (result.Success && result.Data != null)
                Moduller = result.Data;
        }

        private async Task LoadData()
        {
            IsLoading = true;
            var result = await ControllerApiService.GetAllAsync();
            if (result.Success && result.Data != null)
            {
                Controllers = result.Data;
                ApplyFilter();
            }
            IsLoading = false;
        }

        private void ShowAddModal()
        {
            EditingId = null;
            SelectedModulId = 0;
            SelectedUstModulControllerId = null;
            ControllerAdi = string.Empty;
            ParentControllers = new();
            ShowModal = true;
        }

        private async void StartEdit(ModulControllerResponseDto c)
        {
            EditingId = c.ModulControllerId;
            SelectedModulId = c.ModulId;
            SelectedUstModulControllerId = c.UstModulControllerId;
            ControllerAdi = c.ModulControllerAdi;
            await LoadParentControllers();
            ShowModal = true;
            StateHasChanged();
        }

        private void CloseModal()
        {
            ShowModal = false;
            EditingId = null;
        }

        private async Task OnModulChangedInModal()
        {
            SelectedUstModulControllerId = null;
            await LoadParentControllers();
        }

        private async Task LoadParentControllers()
        {
            if (SelectedModulId <= 0)
            {
                ParentControllers = new();
                return;
            }

            var result = await ControllerApiService.GetDropdownByModulIdAsync(SelectedModulId);
            if (result.Success && result.Data != null)
            {
                // Editing yapıyorsak, kendisini listeden çıkar (self-parent olamaz)
                ParentControllers = result.Data
                    .Where(c => !EditingId.HasValue || c.Id != EditingId.Value)
                    .ToList();
            }
        }

        private async Task SaveController()
        {
            if (SelectedModulId <= 0 || string.IsNullOrWhiteSpace(ControllerAdi))
            {
                await ToastService.ShowWarningAsync("Modül ve Controller adı zorunludur");
                return;
            }

            IsSaving = true;
            StateHasChanged();

            if (EditingId.HasValue)
            {
                var request = new ModulControllerUpdateRequestDto
                {
                    ModulId = SelectedModulId,
                    ModulControllerAdi = ControllerAdi.Trim(),
                    UstModulControllerId = SelectedUstModulControllerId > 0 ? SelectedUstModulControllerId : null
                };
                var result = await ControllerApiService.UpdateAsync(EditingId.Value, request);
                if (result.Success)
                    await ToastService.ShowSuccessAsync(result.Message ?? "Controller güncellendi");
                else
                    await ToastService.ShowErrorAsync(result.Message ?? "Controller güncellenemedi");
            }
            else
            {
                var request = new ModulControllerCreateRequestDto
                {
                    ModulId = SelectedModulId,
                    ModulControllerAdi = ControllerAdi.Trim(),
                    UstModulControllerId = SelectedUstModulControllerId > 0 ? SelectedUstModulControllerId : null
                };
                var result = await ControllerApiService.CreateAsync(request);
                if (result.Success)
                    await ToastService.ShowSuccessAsync(result.Message ?? "Controller eklendi");
                else
                    await ToastService.ShowErrorAsync(result.Message ?? "Controller eklenemedi");
            }

            IsSaving = false;
            CloseModal();
            await LoadData();
        }

        private async Task DeleteController(ModulControllerResponseDto c)
        {
            if (c.IslemCount > 0)
            {
                await ToastService.ShowWarningAsync($"Bu controller'a bağlı {c.IslemCount} işlem var. Önce işlemleri silin.");
                return;
            }

            var confirmed = await JS.InvokeAsync<bool>("confirm", $"'{c.ModulControllerAdi}' silinsin mi?");
            if (!confirmed) return;

            var result = await ControllerApiService.DeleteAsync(c.ModulControllerId);
            if (result.Success)
                await ToastService.ShowSuccessAsync(result.Message ?? "Controller silindi");
            else
                await ToastService.ShowErrorAsync(result.Message ?? "Controller silinemedi");
            await LoadData();
        }

        private void ApplyFilter()
        {
            var query = Controllers.AsEnumerable();

            // Search
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                query = query.Where(c =>
                    (c.ModulControllerAdi?.ToLower().Contains(searchLower) ?? false));
            }

            // Filter by Modul
            if (FilterModulId > 0)
            {
                query = query.Where(c => c.ModulId == FilterModulId);
            }

            // Sort
            query = SortBy switch
            {
                "ModulAdi" => SortDirection == "asc" 
                    ? query.OrderBy(c => c.ModulAdi) 
                    : query.OrderByDescending(c => c.ModulAdi),
                "EklenmeTarihi" => SortDirection == "asc" 
                    ? query.OrderBy(c => c.EklenmeTarihi) 
                    : query.OrderByDescending(c => c.EklenmeTarihi),
                _ => SortDirection == "asc" 
                    ? query.OrderBy(c => c.ModulControllerAdi) 
                    : query.OrderByDescending(c => c.ModulControllerAdi)
            };

            FilteredControllers = query.ToList();
            
            // Hiyerarşik yapıyı oluştur
            BuildHierarchy();
            
            StateHasChanged();
        }

        /// <summary>
        /// Parent-child hiyerarşisini oluşturur
        /// Root controller'lar önce, altlarındaki child'lar hemen ardından gösterilir
        /// </summary>
        private void BuildHierarchy()
        {
            HierarchicalControllers = new();
            
            // 1. Root controller'ları bul (UstModulControllerId == null)
            var rootControllers = FilteredControllers
                .Where(c => !c.UstModulControllerId.HasValue)
                .ToList();
            
            // 2. Her root controller için, önce kendisini ekle, sonra child'larını ekle
            foreach (var root in rootControllers)
            {
                HierarchicalControllers.Add(root);
                
                // Bu root'un child'larını bul ve ekle
                AddChildControllers(root.ModulControllerId, 1);
            }
        }

        /// <summary>
        /// Belirtilen parent'ın child controller'larını recursive olarak ekler
        /// </summary>
        private void AddChildControllers(int parentId, int level)
        {
            var children = FilteredControllers
                .Where(c => c.UstModulControllerId == parentId)
                .ToList();
            
            foreach (var child in children)
            {
                HierarchicalControllers.Add(child);
                
                // Bu child'ın da child'ları varsa onları da ekle (recursive)
                AddChildControllers(child.ModulControllerId, level + 1);
            }
        }

        /// <summary>
        /// Controller'ın hiyerarşi seviyesini döndürür (0=root, 1=child, 2=grandchild...)
        /// </summary>
        private int GetHierarchyLevel(ModulControllerResponseDto controller)
        {
            int level = 0;
            var current = controller;
            
            while (current.UstModulControllerId.HasValue)
            {
                level++;
                current = Controllers.FirstOrDefault(c => c.ModulControllerId == current.UstModulControllerId.Value);
                if (current == null) break;
            }
            
            return level;
        }
    }
}
