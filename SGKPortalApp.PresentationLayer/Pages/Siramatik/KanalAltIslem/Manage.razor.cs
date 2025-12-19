using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalAltIslem
{
    public partial class Manage
    {

        [Inject] private IKanalAltIslemApiService _kanalAltIslemService { get; set; } = default!;
        [Inject] private IKanalIslemApiService _kanalIslemService { get; set; } = default!;
        [Inject] private IKanalAltApiService _kanalAltService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KanalAltIslemId { get; set; }
        
        [Parameter]
        [SupplyParameterFromQuery]
        public int? HizmetBinasiId { get; set; }
        
        [Parameter]
        [SupplyParameterFromQuery]
        public int? KanalIslemId { get; set; }

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool IsEditMode => KanalAltIslemId.HasValue && KanalAltIslemId.Value > 0;

        // Form Model
        private KanalAltIslemFormModel model = new();
        private bool isAktif = true;

        // Lookup Data
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<KanalIslemResponseDto> kanalIslemler = new();
        private List<KanalAltResponseDto> kanalAltlar = new();
        private List<KanalAltResponseDto> filteredKanalAltlar = new();
        private List<KanalAltIslemResponseDto> mevcutKanalAltIslemler = new();
        private int selectedHizmetBinasiId = 0;
        private string hizmetBinasiAdi = string.Empty;
        private string kanalIslemAdi = string.Empty;
        private bool isKanalIslemFromUrl = false;

        // Edit Mode Data
        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDropdownData();

            if (IsEditMode)
            {
                await LoadKanalAltIslem();
            }
            else
            {
                // Yeni kayıt için varsayılan değerler
                model = new KanalAltIslemFormModel
                {
                    HizmetBinasiId = 0,
                    KanalIslemId = 0,
                    KanalAltId = 0,
                    Aktiflik = Aktiflik.Aktif
                };

                // ✅ Güvenlik: URL'den HizmetBinasiId parametresi geldiyse yetki kontrolü
                if (HizmetBinasiId.HasValue && HizmetBinasiId.Value > 0)
                {
                    if (!CanAccessHizmetBinasi(HizmetBinasiId.Value))
                    {
                        await _toastService.ShowWarningAsync("Bu Hizmet Binasına erişim yetkiniz yok!");
                        _logger.LogWarning("Yetkisiz Hizmet Binası erişim denemesi (URL): {BinaId}", HizmetBinasiId.Value);
                        _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
                        return;
                    }

                    selectedHizmetBinasiId = HizmetBinasiId.Value;
                    model.HizmetBinasiId = HizmetBinasiId.Value;
                    await LoadKanalIslemler();
                    _logger.LogInformation($"🔗 URL'den HizmetBinasiId alındı: {HizmetBinasiId.Value}");

                    // URL'den kanal işlem parametresi geldiyse otomatik seç
                    if (KanalIslemId.HasValue && KanalIslemId.Value > 0)
                    {
                        model.KanalIslemId = KanalIslemId.Value;
                        isKanalIslemFromUrl = true;
                        var kanalIslem = kanalIslemler.FirstOrDefault(k => k.KanalIslemId == KanalIslemId.Value);
                        kanalIslemAdi = kanalIslem?.KanalAdi ?? "Bilinmeyen";
                        await LoadKanalAltlar();
                        await LoadMevcutKanalAltIslemler();
                        _logger.LogInformation($"🔗 URL'den KanalIslemId alındı: {KanalIslemId.Value}");
                    }
                }
                else
                {
                    // ✅ URL'den parametre gelmediyse kullanıcının kendi HizmetBinası'nı seç
                    var userHizmetBinasiId = GetCurrentUserHizmetBinasiId();
                    if (userHizmetBinasiId > 0)
                    {
                        selectedHizmetBinasiId = userHizmetBinasiId;
                        model.HizmetBinasiId = userHizmetBinasiId;
                        await LoadKanalIslemler();
                    }
                }

                isAktif = true;
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Hizmet binalarını yükle
                var binaResult = await _hizmetBinasiService.GetActiveAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data;
                }
                else
                {
                    hizmetBinalari = new();
                    await _toastService.ShowWarningAsync("Hizmet binaları yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
        }

        private async Task LoadKanalIslemler()
        {
            if (selectedHizmetBinasiId <= 0)
            {
                kanalIslemler = new();
                return;
            }

            try
            {
                var result = await _kanalIslemService.GetByHizmetBinasiIdAsync(selectedHizmetBinasiId);

                if (result.Success && result.Data != null)
                {
                    kanalIslemler = result.Data.Where(k => k.Aktiflik == Aktiflik.Aktif).ToList();
                }
                else
                {
                    kanalIslemler = new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlemler yüklenirken hata oluştu");
                kanalIslemler = new();
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int binaId))
            {
                // ✅ Güvenlik kontrolü
                if (binaId > 0 && !CanAccessHizmetBinasi(binaId))
                {
                    await _toastService.ShowWarningAsync("Bu Hizmet Binasını seçme yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz Hizmet Binası seçim denemesi: {BinaId}", binaId);
                    return;
                }

                selectedHizmetBinasiId = binaId;
                model.HizmetBinasiId = binaId; // Model'i de güncelle
                model.KanalIslemId = 0; // Kanal işlem seçimini sıfırla
                model.KanalAltId = 0; // Alt işlem seçimini sıfırla
                kanalAltlar = new(); // Alt işlem listesini temizle
                await LoadKanalIslemler();
                StateHasChanged();
            }
        }

        private async Task OnKanalIslemChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int kanalIslemId))
            {
                model.KanalIslemId = kanalIslemId;
                model.KanalAltId = 0; // Alt işlem seçimini sıfırla
                mevcutKanalAltIslemler = new(); // Mevcut kayıtları sıfırla
                await LoadKanalAltlar();
                await LoadMevcutKanalAltIslemler();
                StateHasChanged();
            }
        }

        private async Task LoadKanalAltlar()
        {
            if (model.KanalIslemId <= 0)
            {
                kanalAltlar = new();
                filteredKanalAltlar = new();
                return;
            }

            try
            {
                // Seçilen KanalIslem'in Kanal'ına ait KanalAlt'ları getir
                // Önce KanalIslem'den KanalId'yi bulalım
                var kanalIslem = kanalIslemler.FirstOrDefault(k => k.KanalIslemId == model.KanalIslemId);
                if (kanalIslem == null || kanalIslem.KanalId <= 0)
                {
                    kanalAltlar = new();
                    filteredKanalAltlar = new();
                    return;
                }

                var result = await _kanalAltService.GetByKanalIdAsync(kanalIslem.KanalId);

                if (result.Success && result.Data != null)
                {
                    kanalAltlar = result.Data.Where(k => k.Aktiflik == Aktiflik.Aktif).ToList();
                    _logger.LogInformation($"✅ {kanalAltlar.Count} aktif kanal alt yüklendi (KanalId: {kanalIslem.KanalId})");
                    
                    // Zaten eklenmiş alt kanalları filtrele
                    FilterKanalAltlar();
                }
                else
                {
                    kanalAltlar = new();
                    filteredKanalAltlar = new();
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        _logger.LogWarning($"⚠️ KanalAlt'lar yüklenemedi: {result.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ KanalAlt'lar yüklenirken hata oluştu");
                kanalAltlar = new();
                filteredKanalAltlar = new();
            }
        }
        
        private async Task LoadMevcutKanalAltIslemler()
        {
            if (model.KanalIslemId <= 0)
            {
                mevcutKanalAltIslemler = new();
                return;
            }

            try
            {
                var result = await _kanalAltIslemService.GetByKanalIslemIdAsync(model.KanalIslemId);
                if (result.Success && result.Data != null)
                {
                    mevcutKanalAltIslemler = result.Data;
                    _logger.LogInformation($"📋 {mevcutKanalAltIslemler.Count} mevcut kanal alt işlem yüklendi (KanalIslemId: {model.KanalIslemId})");
                }
                else
                {
                    mevcutKanalAltIslemler = new();
                }
                
                // Zaten eklenmiş alt kanalları filtrele
                FilterKanalAltlar();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mevcut kanal alt işlemler yüklenirken hata");
                mevcutKanalAltIslemler = new();
                filteredKanalAltlar = kanalAltlar.ToList();
            }
        }
        
        private void FilterKanalAltlar()
        {
            // Mevcut kanal alt işlemlerde kullanılan KanalAltId'leri al
            var kullanilmisKanalAltIds = mevcutKanalAltIslemler
                .Select(k => k.KanalAltId)
                .ToHashSet();
            
            // Edit modunda, düzenlenen kaydın alt kanalını listeden çıkarma
            if (IsEditMode && model.KanalAltId > 0)
            {
                kullanilmisKanalAltIds.Remove(model.KanalAltId);
            }
            
            // Kullanılmamış alt kanalları filtrele
            filteredKanalAltlar = kanalAltlar
                .Where(k => !kullanilmisKanalAltIds.Contains(k.KanalAltId))
                .ToList();
            
            _logger.LogInformation($"📋 Filtreleme: {kanalAltlar.Count} toplam alt kanal, {kullanilmisKanalAltIds.Count} kullanılmış, {filteredKanalAltlar.Count} kullanılabilir");
        }

        private async Task LoadKanalAltIslem()
        {
            try
            {
                isLoading = true;
                var result = await _kanalAltIslemService.GetByIdWithDetailsAsync(KanalAltIslemId!.Value);

                if (result.Success && result.Data != null)
                {
                    var altIslem = result.Data;
                    
                    model = new KanalAltIslemFormModel
                    {
                        KanalAltId = altIslem.KanalAltId,
                        KanalIslemId = altIslem.KanalIslemId,
                        HizmetBinasiId = altIslem.HizmetBinasiId,
                        Aktiflik = altIslem.Aktiflik
                    };

                    // Hizmet binasını seç ve hizmet binası adını bul (edit modda readonly input için)
                    selectedHizmetBinasiId = altIslem.HizmetBinasiId;
                    var bina = hizmetBinalari.FirstOrDefault(b => b.HizmetBinasiId == altIslem.HizmetBinasiId);
                    hizmetBinasiAdi = bina?.HizmetBinasiAdi ?? "Bilinmeyen";

                    // Kanal işlemleri yükle
                    await LoadKanalIslemler();
                    
                    // Kanal işlem adını bul (edit modda readonly input için)
                    var kanalIslem = kanalIslemler.FirstOrDefault(k => k.KanalIslemId == altIslem.KanalIslemId);
                    kanalIslemAdi = kanalIslem?.KanalAdi ?? "Bilinmeyen";

                    // Kanal işlem seçili olduğunda KanalAlt'ları da yükle
                    await LoadKanalAltlar();
                    
                    // Mevcut kayıtları yükle (filtreleme için)
                    await LoadMevcutKanalAltIslemler();

                    isAktif = altIslem.Aktiflik == Aktiflik.Aktif;
                    eklenmeTarihi = altIslem.EklenmeTarihi;
                    duzenlenmeTarihi = altIslem.DuzenlenmeTarihi;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Alt işlem bulunamadı");
                    _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alt işlem yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Alt işlem yüklenirken bir hata oluştu");
                _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            try
            {
                isSaving = true;

                // Aktiflik durumunu modele aktar
                model.Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif;

                // HizmetBinasiId'yi modele aktar
                model.HizmetBinasiId = selectedHizmetBinasiId;

                // Validasyon DataAnnotations tarafından yapılıyor
                _logger.LogInformation($"📝 Form submit - KanalAltId: {model.KanalAltId}, KanalIslemId: {model.KanalIslemId}, HizmetBinasiId: {model.HizmetBinasiId}");

                if (IsEditMode)
                {
                    await UpdateKanalAltIslem();
                }
                else
                {
                    await CreateKanalAltIslem();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Form gönderilirken hata oluştu");
                await _toastService.ShowErrorAsync("İşlem sırasında bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task CreateKanalAltIslem()
        {
            // ✅ Güvenlik: Form submit öncesi son kontrol (form manipulation önlemi)
            if (!CanAccessHizmetBinasi(model.HizmetBinasiId))
            {
                await _toastService.ShowErrorAsync("Bu Hizmet Binasında kayıt oluşturma yetkiniz yok!");
                _logger.LogWarning("Yetkisiz kayıt oluşturma denemesi: HizmetBinasiId={BinaId}", model.HizmetBinasiId);
                return;
            }

            var createDto = new KanalAltIslemCreateRequestDto
            {
                KanalAltId = model.KanalAltId,
                KanalIslemId = model.KanalIslemId,
                HizmetBinasiId = model.HizmetBinasiId,
                KioskIslemGrupId = model.KioskIslemGrupId,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalAltIslemService.CreateAsync(createDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync("Kanal alt işlem başarıyla eklendi");
                _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Alt işlem eklenemedi");
            }
        }

        private async Task UpdateKanalAltIslem()
        {
            var updateDto = new KanalAltIslemUpdateRequestDto
            {
                KanalAltId = model.KanalAltId,
                KanalIslemId = model.KanalIslemId,
                HizmetBinasiId = model.HizmetBinasiId,
                KioskIslemGrupId = model.KioskIslemGrupId,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalAltIslemService.UpdateAsync(KanalAltIslemId!.Value, updateDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync("Kanal alt işlem başarıyla güncellendi");
                _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Alt işlem güncellenemedi");
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
        }
    }
}
