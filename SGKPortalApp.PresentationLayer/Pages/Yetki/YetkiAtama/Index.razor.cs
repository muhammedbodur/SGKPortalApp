using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Models.ViewModels;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Yetki.YetkiAtama
{
    public partial class Index : ComponentBase
    {
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        // DEPENDENCY INJECTION
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        [Inject] private IPersonelApiService _personelService { get; set; } = default!;
        [Inject] private IPersonelYetkiApiService _personelYetkiService { get; set; } = default!;
        [Inject] private IModulControllerIslemApiService _modulControllerIslemService { get; set; } = default!;
        [Inject] private IDepartmanApiService _departmanService { get; set; } = default!;
        [Inject] private IServisApiService _servisService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        // STATE PROPERTIES
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        private bool IsLoading { get; set; } = false;
        private bool IsSaving { get; set; } = false;
        private bool IsSearching { get; set; } = false;

        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        // PERSONEL SEÇİMİ
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        private string SearchTerm { get; set; } = string.Empty;
        private int FilterDepartmanId { get; set; } = 0;
        private int FilterServisId { get; set; } = 0;
        private List<DropdownItemDto> DepartmanDropdown { get; set; } = new();
        private List<DropdownItemDto> ServisDropdown { get; set; } = new();
        private List<PersonelResponseDto> PersonelList { get; set; } = new();
        private PersonelResponseDto? SelectedPersonel { get; set; }

        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        // YETKİ AĞACI
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        private List<PermissionTreeNode> PermissionTree { get; set; } = new();
        private Dictionary<int, PersonelYetkiResponseDto> ExistingPermissions { get; set; } = new();

        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        // LIFECYCLE
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        protected override async Task OnInitializedAsync()
        {
            await LoadDropdowns();
        }

        private async Task LoadDropdowns()
        {
            try
            {
                var departmanTask = _departmanService.GetActiveAsync();
                var servisTask = _servisService.GetActiveAsync();

                await Task.WhenAll(departmanTask, servisTask);

                var departmanResult = await departmanTask;
                var servisResult = await servisTask;

                DepartmanDropdown = departmanResult.Success && departmanResult.Data != null
                    ? departmanResult.Data.Select(d => new DropdownItemDto { Id = d.DepartmanId, Ad = d.DepartmanAdi }).ToList()
                    : new List<DropdownItemDto>();

                ServisDropdown = servisResult.Success && servisResult.Data != null
                    ? servisResult.Data.Select(s => new DropdownItemDto { Id = s.ServisId, Ad = s.ServisAdi }).ToList()
                    : new List<DropdownItemDto>();
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Dropdown yüklenemedi: {ex.Message}");
            }
        }

        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        // PERSONEL ARAMA
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        private async Task HandleSearchKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await SearchPersonel();
            }
        }

        private async Task SearchPersonel()
        {
            // En az bir filtre girilmeli
            if (string.IsNullOrWhiteSpace(SearchTerm) && FilterDepartmanId <= 0 && FilterServisId <= 0)
            {
                PersonelList = new();
                return;
            }

            IsSearching = true;
            try
            {
                var filter = new PersonelFilterRequestDto
                {
                    SearchTerm = string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim(),
                    DepartmanId = FilterDepartmanId > 0 ? FilterDepartmanId : null,
                    ServisId = FilterServisId > 0 ? FilterServisId : null,
                    AktiflikDurum = PersonelAktiflikDurum.Aktif,
                    PageNumber = 1,
                    PageSize = 50
                };

                var result = await _personelService.GetPagedAsync(filter);
                if (result.Success && result.Data != null)
                {
                    // PersonelListResponseDto -> PersonelResponseDto dönüşümü
                    PersonelList = result.Data.Items.Select(p => new PersonelResponseDto
                    {
                        TcKimlikNo = p.TcKimlikNo,
                        SicilNo = p.SicilNo,
                        AdSoyad = p.AdSoyad,
                        Email = p.Email,
                        DepartmanId = 0, // Liste DTO'da yok
                        DepartmanAdi = p.DepartmanAdi,
                        ServisId = 0, // Liste DTO'da yok
                        ServisAdi = p.ServisAdi
                    }).ToList();
                }
                else
                {
                    PersonelList = new();
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Arama hatası: {ex.Message}");
            }
            finally
            {
                IsSearching = false;
            }
        }

        private async Task SelectPersonel(PersonelResponseDto personel)
        {
            SelectedPersonel = personel;
            await LoadPermissionTree();
        }

        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        // YETKİ AĞACI YÜKLEME
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        private async Task LoadPermissionTree()
        {
            if (SelectedPersonel == null)
                return;

            IsLoading = true;
            try
            {
                // Tüm işlemleri getir
                var islemResult = await _modulControllerIslemService.GetAllAsync();
                
                // Personelin mevcut yetkilerini getir
                var yetkiResult = await _personelYetkiService.GetByTcKimlikNoAsync(SelectedPersonel.TcKimlikNo);

                if (islemResult.Success && islemResult.Data != null)
                {
                    // Mevcut yetkileri dictionary'e al
                    ExistingPermissions = yetkiResult.Success && yetkiResult.Data != null
                        ? yetkiResult.Data.ToDictionary(x => x.ModulControllerIslemId)
                        : new Dictionary<int, PersonelYetkiResponseDto>();

                    // Tree yapısını oluştur
                    PermissionTree = BuildPermissionTree(islemResult.Data);
                }
                else
                {
                    PermissionTree = new();
                    await _toastService.ShowErrorAsync(islemResult.Message ?? "İşlemler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Yetkiler yüklenemedi: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private List<PermissionTreeNode> BuildPermissionTree(List<ModulControllerIslemResponseDto> islemler)
        {
            var nodes = new List<PermissionTreeNode>();

            // Modül bazlı grupla
            var modulGroups = islemler
                .GroupBy(x => new { x.ModulId, x.ModulAdi })
                .OrderBy(g => g.Key.ModulAdi);

            foreach (var modulGroup in modulGroups)
            {
                var modulNode = new PermissionTreeNode
                {
                    ModulControllerIslemId = -modulGroup.Key.ModulId, // Negatif ID (grup için)
                    IslemAdi = modulGroup.Key.ModulAdi,
                    IslemTipi = YetkiIslemTipi.Grup,
                    IsGroupNode = true
                };

                // Controller bazlı grupla
                var controllerGroups = modulGroup
                    .GroupBy(x => new { x.ModulControllerId, x.ModulControllerAdi })
                    .OrderBy(g => g.Key.ModulControllerAdi);

                foreach (var controllerGroup in controllerGroups)
                {
                    var controllerNode = new PermissionTreeNode
                    {
                        ModulControllerIslemId = -controllerGroup.Key.ModulControllerId - 10000, // Negatif ID (grup için)
                        IslemAdi = controllerGroup.Key.ModulControllerAdi,
                        IslemTipi = YetkiIslemTipi.Grup,
                        IsGroupNode = true
                    };

                    // İşlemleri ekle (hiyerarşik)
                    var rootIslemler = controllerGroup.Where(x => x.UstIslemId == null).OrderBy(x => x.ModulControllerIslemAdi);
                    foreach (var islem in rootIslemler)
                    {
                        var islemNode = CreateIslemNode(islem, controllerGroup.ToList());
                        controllerNode.Children.Add(islemNode);
                    }

                    modulNode.Children.Add(controllerNode);
                }

                nodes.Add(modulNode);
            }

            return nodes;
        }

        private PermissionTreeNode CreateIslemNode(ModulControllerIslemResponseDto islem, List<ModulControllerIslemResponseDto> allIslemler)
        {
            var node = new PermissionTreeNode
            {
                ModulControllerIslemId = islem.ModulControllerIslemId,
                IslemAdi = islem.ModulControllerIslemAdi,
                Aciklama = islem.Aciklama,
                PermissionKey = islem.PermissionKey,
                IslemTipi = islem.IslemTipi,
                IsGroupNode = false
            };

            // Mevcut yetkiyi kontrol et
            if (ExistingPermissions.TryGetValue(islem.ModulControllerIslemId, out var existingPerm))
            {
                node.PersonelYetkiId = existingPerm.PersonelYetkiId;
                node.SelectedLevel = existingPerm.YetkiSeviyesi;
                node.OriginalLevel = existingPerm.YetkiSeviyesi;
            }

            // Alt işlemleri ekle
            var childIslemler = allIslemler.Where(x => x.UstIslemId == islem.ModulControllerIslemId).OrderBy(x => x.ModulControllerIslemAdi);
            foreach (var child in childIslemler)
            {
                node.Children.Add(CreateIslemNode(child, allIslemler));
            }

            return node;
        }

        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        // YETKİ DEĞİŞTİRME
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        private void SetPermissionLevel(PermissionTreeNode node, YetkiSeviyesi level)
        {
            if (node.IsGroupNode)
                return;

            node.SelectedLevel = level;
            node.HasChanges = node.SelectedLevel != node.OriginalLevel;
            StateHasChanged();
        }

        private void HandleLevelChanged((PermissionTreeNode Node, YetkiSeviyesi Level) args)
        {
            SetPermissionLevel(args.Node, args.Level);
        }

        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        // TOPLU KAYDETME
        // amamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamamam
        private async Task SaveAllPermissions()
        {
            if (SelectedPersonel == null)
                return;

            IsSaving = true;
            try
            {
                var changedNodes = GetAllChangedNodes(PermissionTree);
                
                if (!changedNodes.Any())
                {
                    await _toastService.ShowInfoAsync("Değişiklik yapılmadı");
                    return;
                }

                int successCount = 0;
                int errorCount = 0;

                foreach (var node in changedNodes)
                {
                    try
                    {
                        if (node.PersonelYetkiId > 0)
                        {
                            // Güncelle
                            var updateDto = new PersonelYetkiUpdateRequestDto
                            {
                                ModulControllerIslemId = node.ModulControllerIslemId,
                                YetkiSeviyesi = node.SelectedLevel
                            };
                            var result = await _personelYetkiService.UpdateAsync(node.PersonelYetkiId, updateDto);
                            if (result.Success) successCount++; else errorCount++;
                        }
                        else
                        {
                            // Yeni oluştur
                            var createDto = new PersonelYetkiCreateRequestDto
                            {
                                TcKimlikNo = SelectedPersonel.TcKimlikNo,
                                ModulControllerIslemId = node.ModulControllerIslemId,
                                YetkiSeviyesi = node.SelectedLevel
                            };
                            var result = await _personelYetkiService.CreateAsync(createDto);
                            if (result.Success) successCount++; else errorCount++;
                        }
                    }
                    catch
                    {
                        errorCount++;
                    }
                }

                if (errorCount == 0)
                {
                    await _toastService.ShowSuccessAsync($"{successCount} yetki başarıyla kaydedildi");
                }
                else
                {
                    await _toastService.ShowWarningAsync($"{successCount} başarılı, {errorCount} hatalı");
                }

                // Yeniden yükle
                await LoadPermissionTree();
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Kaydetme hatası: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private List<PermissionTreeNode> GetAllChangedNodes(List<PermissionTreeNode> nodes)
        {
            var result = new List<PermissionTreeNode>();
            foreach (var node in nodes)
            {
                if (!node.IsGroupNode && node.HasChanges)
                {
                    result.Add(node);
                }
                result.AddRange(GetAllChangedNodes(node.Children));
            }
            return result;
        }
    }
}
