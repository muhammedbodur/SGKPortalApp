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
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        
        // Hizmet Binası Bilgisi
        private int selectedHizmetBinasiId = 0;
        private string hizmetBinasiAdi = string.Empty;

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
                // Yeni kayıt - URL'den hizmet binası kontrolü
                if (HizmetBinasiId.HasValue && HizmetBinasiId.Value > 0)
                {
                    selectedHizmetBinasiId = HizmetBinasiId.Value;
                    // Hizmet binası adını bul
                    var bina = hizmetBinalari.FirstOrDefault(b => b.HizmetBinasiId == selectedHizmetBinasiId);
                    hizmetBinasiAdi = bina?.HizmetBinasiAdi ?? "Bilinmeyen";
                }
                else
                {
                    // URL'de parametre yok - hata
                    await _toastService.ShowErrorAsync("Hizmet binası seçilmeden kanal işlem eklenemez");
                    _navigationManager.NavigateTo("/siramatik/kanal-islem");
                    return;
                }

                // Yeni kayıt için varsayılan değerler
                model = new KanalIslemFormModel
                {
                    HizmetBinasiId = selectedHizmetBinasiId,
                    BaslangicNumara = 0,
                    BitisNumara = 9999,
                    Sira = 1,
                    Aktiflik = Aktiflik.Aktif
                };
                isAktif = true;
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
                    
                    // Hizmet binası adını bul
                    var bina = hizmetBinalari.FirstOrDefault(b => b.HizmetBinasiId == selectedHizmetBinasiId);
                    hizmetBinasiAdi = bina?.HizmetBinasiAdi ?? kanalIslem.HizmetBinasiAdi;

                    model = new KanalIslemFormModel
                    {
                        KanalId = kanalIslem.KanalId,
                        KanalAdi = kanalIslem.KanalAdi,
                        HizmetBinasiId = selectedHizmetBinasiId,
                        BaslangicNumara = kanalIslem.BaslangicNumara,
                        BitisNumara = kanalIslem.BitisNumara,
                        Sira = kanalIslem.Sira,
                        Aktiflik = kanalIslem.Aktiflik
                    };

                    isAktif = kanalIslem.Aktiflik == Aktiflik.Aktif;
                    eklenmeTarihi = kanalIslem.EklenmeTarihi;
                    duzenlenmeTarihi = kanalIslem.DuzenlenmeTarihi;
                    
                    _logger.LogInformation($"📝 Model güncellendi - KanalId: {model.KanalId}, HizmetBinasiId: {selectedHizmetBinasiId}");
                }
                else
                {
                    _logger.LogWarning($"⚠️ Kanal işlem bulunamadı: {result.Message}");
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
                _logger.LogInformation($"📝 Form değerleri - KanalId: {model.KanalId}, HizmetBinasiId: {selectedHizmetBinasiId}, Sira: {model.Sira}");

                model.Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif;

                // Validasyon
                if (selectedHizmetBinasiId <= 0)
                {
                    _logger.LogWarning("⚠️ Hizmet binası ID geçersiz");
                    await _toastService.ShowErrorAsync("Hizmet binası bilgisi eksik. Lütfen sayfayı yenileyin.");
                    return;
                }

                if (model.KanalId <= 0)
                {
                    _logger.LogWarning("⚠️ Kanal ID geçersiz");
                    await _toastService.ShowErrorAsync("Lütfen bir kanal seçiniz");
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
                HizmetBinasiId = selectedHizmetBinasiId,
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
                HizmetBinasiId = selectedHizmetBinasiId,
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