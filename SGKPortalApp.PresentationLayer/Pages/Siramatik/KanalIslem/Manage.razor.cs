using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalIslem
{
    public partial class Manage
    {
        protected override string PagePermissionKey => "SIRA.KANALISLEM.MANAGE";

        [Inject] private IKanalIslemApiService _kanalIslemService { get; set; } = default!;
        [Inject] private IKanalApiService _kanalService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KanalIslemId { get; set; }
        
        [Parameter]
        [SupplyParameterFromQuery]
        public int? HizmetBinasiId { get; set; }

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool IsEditMode => KanalIslemId.HasValue && KanalIslemId.Value > 0;

        // Form Model
        private KanalIslemFormModel model = new();
        private bool isAktif = true;

        // Dropdown Data
        private List<KanalResponseDto> anaKanallar = new();
        private List<KanalResponseDto> filteredAnaKanallar = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        
        // Mevcut Kanal İşlemler (validasyon için)
        private List<KanalIslemResponseDto> mevcutKanalIslemler = new();
        
        // Hizmet Binası Bilgisi
        private int selectedHizmetBinasiId = 0;
        private string hizmetBinasiAdi = string.Empty;
        private bool isHizmetBinasiFromUrl = false;
        
        // Validasyon mesajları
        private string? numaraAralikHatasi = null;
        private string? siraHatasi = null;

        // Edit Mode Data
        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi;

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownData();

            if (IsEditMode)
            {
                await LoadKanalIslem();
            }
            else
            {
                // Yeni kayıt için varsayılan değerler
                model = new KanalIslemFormModel
                {
                    HizmetBinasiId = 0, // Kullanıcı dropdown'dan seçecek
                    BaslangicNumara = 0,
                    BitisNumara = 9999,
                    Sira = 1,
                    Aktiflik = Aktiflik.Aktif
                };

                // URL'den hizmet binası parametresi geldiyse otomatik seç
                if (HizmetBinasiId.HasValue && HizmetBinasiId.Value > 0)
                {
                    model.HizmetBinasiId = HizmetBinasiId.Value;
                    isHizmetBinasiFromUrl = true;
                    var bina = hizmetBinalari.FirstOrDefault(b => b.HizmetBinasiId == HizmetBinasiId.Value);
                    hizmetBinasiAdi = bina?.HizmetBinasiAdi ?? "Bilinmeyen";
                    await LoadMevcutKanalIslemler(HizmetBinasiId.Value);
                    SuggestNextValues();
                }

                isAktif = true;
            }
        }
        
        private async Task LoadMevcutKanalIslemler(int hizmetBinasiId)
        {
            try
            {
                var result = await _kanalIslemService.GetByHizmetBinasiIdAsync(hizmetBinasiId);
                if (result.Success && result.Data != null)
                {
                    mevcutKanalIslemler = result.Data;
                }
                else
                {
                    mevcutKanalIslemler = new();
                }
                
                // Zaten eklenmiş kanalları filtrele
                FilterAnaKanallar();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mevcut kanal işlemler yüklenirken hata");
                mevcutKanalIslemler = new();
                filteredAnaKanallar = anaKanallar.ToList();
            }
        }
        
        private void FilterAnaKanallar()
        {
            // Mevcut kanal işlemlerde kullanılan KanalId'leri al
            var kullanilmisKanalIds = mevcutKanalIslemler
                .Select(k => k.KanalId)
                .ToHashSet();
            
            // Edit modunda, düzenlenen kaydın kanalını listeden çıkarma
            if (IsEditMode && model.KanalId > 0)
            {
                kullanilmisKanalIds.Remove(model.KanalId);
            }
            
            // Kullanılmamış kanalları filtrele
            filteredAnaKanallar = anaKanallar
                .Where(k => !kullanilmisKanalIds.Contains(k.KanalId))
                .ToList();
            
            _logger.LogInformation($"📋 Filtreleme: {anaKanallar.Count} toplam kanal, {kullanilmisKanalIds.Count} kullanılmış, {filteredAnaKanallar.Count} kullanılabilir");
        }
        
        private void SuggestNextValues()
        {
            if (!mevcutKanalIslemler.Any())
            {
                model.Sira = 1;
                model.BaslangicNumara = 1;
                model.BitisNumara = 999;
                return;
            }
            
            // Sonraki sırayı öner
            var maxSira = mevcutKanalIslemler.Max(k => k.Sira);
            model.Sira = maxSira + 1;
            
            // Sonraki numara aralığını öner
            var maxBitisNumara = mevcutKanalIslemler.Max(k => k.BitisNumara);
            model.BaslangicNumara = maxBitisNumara + 1;
            model.BitisNumara = maxBitisNumara + 1000;
        }
        
        private bool ValidateNumaraAraligi()
        {
            numaraAralikHatasi = null;
            
            // Temel kontroller
            if (model.BaslangicNumara < 0)
            {
                numaraAralikHatasi = "Başlangıç numarası 0'dan küçük olamaz.";
                return false;
            }
            
            if (model.BitisNumara <= model.BaslangicNumara)
            {
                numaraAralikHatasi = "Bitiş numarası, başlangıç numarasından büyük olmalıdır.";
                return false;
            }
            
            // Çakışma kontrolü
            var cakisanKayit = mevcutKanalIslemler
                .Where(k => !IsEditMode || k.KanalIslemId != KanalIslemId)
                .FirstOrDefault(k => 
                    (model.BaslangicNumara >= k.BaslangicNumara && model.BaslangicNumara <= k.BitisNumara) ||
                    (model.BitisNumara >= k.BaslangicNumara && model.BitisNumara <= k.BitisNumara) ||
                    (model.BaslangicNumara <= k.BaslangicNumara && model.BitisNumara >= k.BitisNumara));
            
            if (cakisanKayit != null)
            {
                numaraAralikHatasi = $"Bu numara aralığı '{cakisanKayit.KanalAdi}' ({cakisanKayit.BaslangicNumara}-{cakisanKayit.BitisNumara}) ile çakışıyor!";
                return false;
            }
            
            return true;
        }
        
        private bool ValidateSira()
        {
            siraHatasi = null;
            
            if (model.Sira < 1 || model.Sira > 999)
            {
                siraHatasi = "Sıra 1-999 arasında olmalıdır.";
                return false;
            }
            
            // Aynı sıra numarası kontrolü
            var ayniSira = mevcutKanalIslemler
                .Where(k => !IsEditMode || k.KanalIslemId != KanalIslemId)
                .FirstOrDefault(k => k.Sira == model.Sira);
            
            if (ayniSira != null)
            {
                siraHatasi = $"Bu sıra numarası '{ayniSira.KanalAdi}' tarafından kullanılıyor!";
                return false;
            }
            
            return true;
        }
        
        private void OnNumaraChanged()
        {
            ValidateNumaraAraligi();
        }
        
        private void OnSiraChanged()
        {
            ValidateSira();
        }
        
        private async Task OnHizmetBinasiChangedInForm(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int binaId) && binaId > 0)
            {
                model.HizmetBinasiId = binaId;
                await LoadMevcutKanalIslemler(binaId);
                SuggestNextValues();
                StateHasChanged();
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Ana kanalları yükle
                var kanalResult = await _kanalService.GetActiveAsync();
                if (kanalResult.Success && kanalResult.Data != null)
                {
                    anaKanallar = kanalResult.Data;
                    _logger.LogInformation($"✅ {anaKanallar.Count} aktif kanal yüklendi");
                }

                // Hizmet binalarını yükle
                var binaResult = await _hizmetBinasiService.GetActiveAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data;
                    _logger.LogInformation($"✅ {hizmetBinalari.Count} hizmet binası yüklendi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Dropdown verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
        }

        private async Task LoadKanalIslem()
        {
            try
            {
                isLoading = true;
                _logger.LogInformation($"🔄 Kanal işlem yükleniyor... ID: {KanalIslemId}");

                var result = await _kanalIslemService.GetByIdAsync(KanalIslemId!.Value);

                if (result.Success && result.Data != null)
                {
                    var kanalIslem = result.Data;
                    _logger.LogInformation($"✅ Kanal işlem yüklendi: ID={kanalIslem.KanalIslemId}, KanalId={kanalIslem.KanalId}, HizmetBinasiId={kanalIslem.HizmetBinasiId}");

                    // HizmetBinasiId'yi DTO'dan al
                    selectedHizmetBinasiId = kanalIslem.HizmetBinasiId;

                    // Hizmet binası adını bul (edit modda readonly input için)
                    var bina = hizmetBinalari.FirstOrDefault(b => b.HizmetBinasiId == kanalIslem.HizmetBinasiId);
                    hizmetBinasiAdi = bina?.HizmetBinasiAdi ?? kanalIslem.HizmetBinasiAdi;

                    model = new KanalIslemFormModel
                    {
                        KanalId = kanalIslem.KanalId,
                        KanalAdi = kanalIslem.KanalAdi,
                        HizmetBinasiId = kanalIslem.HizmetBinasiId,
                        BaslangicNumara = kanalIslem.BaslangicNumara,
                        BitisNumara = kanalIslem.BitisNumara,
                        Sira = kanalIslem.Sira,
                        Aktiflik = kanalIslem.Aktiflik
                    };

                    isAktif = kanalIslem.Aktiflik == Aktiflik.Aktif;
                    eklenmeTarihi = kanalIslem.EklenmeTarihi;
                    duzenlenmeTarihi = kanalIslem.DuzenlenmeTarihi;
                    
                    // Mevcut kayıtları yükle (validasyon için)
                    await LoadMevcutKanalIslemler(kanalIslem.HizmetBinasiId);
                    
                    // Edit modunda mevcut değerler için validasyon çalıştır
                    ValidateNumaraAraligi();
                    ValidateSira();

                    _logger.LogInformation($" Model güncellendi - KanalId: {model.KanalId}, HizmetBinasiId: {model.HizmetBinasiId}");
                }
                else
                {
                    _logger.LogWarning($" Kanal işlem bulunamadı: {result.Message}");
                    await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem bulunamadı");
                    _navigationManager.NavigateTo("/siramatik/kanal-islem");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Kanal işlem yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Kanal işlem yüklenirken bir hata oluştu");
                _navigationManager.NavigateTo("/siramatik/kanal-islem");
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
                
                _logger.LogInformation($"🔄 Form submit başladı - IsEditMode: {IsEditMode}, KanalIslemId: {KanalIslemId}");
                _logger.LogInformation($"📝 Form değerleri - KanalId: {model.KanalId}, HizmetBinasiId: {model.HizmetBinasiId}, Sira: {model.Sira}");

                model.Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif;

                // Validasyon
                if (model.HizmetBinasiId <= 0)
                {
                    _logger.LogWarning("⚠️ Hizmet binası seçimi zorunlu");
                    await _toastService.ShowErrorAsync("Lütfen bir hizmet binası seçiniz");
                    return;
                }

                if (model.KanalId <= 0)
                {
                    _logger.LogWarning("⚠️ Kanal ID geçersiz");
                    await _toastService.ShowErrorAsync("Lütfen bir kanal seçiniz");
                    return;
                }
                
                // Numara aralığı ve sıra validasyonu
                if (!ValidateNumaraAraligi())
                {
                    await _toastService.ShowErrorAsync(numaraAralikHatasi ?? "Numara aralığı geçersiz");
                    return;
                }
                
                if (!ValidateSira())
                {
                    await _toastService.ShowErrorAsync(siraHatasi ?? "Sıra numarası geçersiz");
                    return;
                }

                if (IsEditMode)
                {
                    _logger.LogInformation($"✏️ Güncelleme modu - ID: {KanalIslemId}");
                    await UpdateKanalIslem();
                }
                else
                {
                    _logger.LogInformation("➕ Yeni kayıt modu");
                    await CreateKanalIslem();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Form gönderilirken hata oluştu");
                await _toastService.ShowErrorAsync($"İşlem sırasında bir hata oluştu: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task CreateKanalIslem()
        {
            _logger.LogInformation("🔄 CreateKanalIslem başladı");
            
            var createDto = new KanalIslemCreateRequestDto
            {
                KanalId = model.KanalId,
                HizmetBinasiId = model.HizmetBinasiId,
                BaslangicNumara = model.BaslangicNumara,
                BitisNumara = model.BitisNumara,
                Sira = model.Sira,
                Aktiflik = model.Aktiflik
            };

            _logger.LogInformation($"📤 API'ye gönderilecek DTO: KanalId={createDto.KanalId}, HizmetBinasiId={createDto.HizmetBinasiId}, Sira={createDto.Sira}");

            var result = await _kanalIslemService.CreateAsync(createDto);

            _logger.LogInformation($"📥 API yanıtı: Success={result.Success}, Message={result.Message}");

            if (result.Success)
            {
                _logger.LogInformation("✅ Kanal işlem başarıyla eklendi");
                await _toastService.ShowSuccessAsync("Kanal işlem başarıyla eklendi");
                _navigationManager.NavigateTo("/siramatik/kanal-islem");
            }
            else
            {
                _logger.LogWarning($"⚠️ Kanal işlem eklenemedi: {result.Message}");
                await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem eklenemedi");
            }
        }

        private async Task UpdateKanalIslem()
        {
            _logger.LogInformation($"🔄 UpdateKanalIslem başladı - KanalIslemId: {KanalIslemId}");
            
            var updateDto = new KanalIslemUpdateRequestDto
            {
                KanalId = model.KanalId,
                HizmetBinasiId = model.HizmetBinasiId,
                BaslangicNumara = model.BaslangicNumara,
                BitisNumara = model.BitisNumara,
                Sira = model.Sira,
                Aktiflik = model.Aktiflik
            };

            _logger.LogInformation($"📤 API'ye gönderilecek DTO: KanalId={updateDto.KanalId}, HizmetBinasiId={updateDto.HizmetBinasiId}, Sira={updateDto.Sira}, Aktiflik={updateDto.Aktiflik}");
            _logger.LogInformation($"🎯 API Endpoint: PUT /api/kanalislem/{KanalIslemId!.Value}");

            var result = await _kanalIslemService.UpdateAsync(KanalIslemId!.Value, updateDto);

            _logger.LogInformation($"📥 API yanıtı: Success={result.Success}, Message={result.Message}");
            
            if (result.Data != null)
            {
                _logger.LogInformation($"📄 Dönen Data: KanalIslemId={result.Data.KanalIslemId}, KanalId={result.Data.KanalId}");
            }

            if (result.Success)
            {
                _logger.LogInformation("✅ Kanal işlem başarıyla güncellendi");
                await _toastService.ShowSuccessAsync("Kanal işlem başarıyla güncellendi");
                _navigationManager.NavigateTo("/siramatik/kanal-islem");
            }
            else
            {
                _logger.LogWarning($"⚠️ Kanal işlem güncellenemedi: {result.Message}");
                await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem güncellenemedi");
            }
        }

        private void NavigateBack()
        {
            _logger.LogInformation("🔙 Geri dönülüyor");
            _navigationManager.NavigateTo("/siramatik/kanal-islem");
        }
    }
}