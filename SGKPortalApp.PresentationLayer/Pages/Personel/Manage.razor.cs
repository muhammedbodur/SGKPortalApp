using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices;
using SGKPortalApp.PresentationLayer.Components.Base;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.AspNetCore.WebUtilities;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels;
using AutoMapper;

namespace SGKPortalApp.PresentationLayer.Pages.Personel
{
    public partial class Manage : BasePageComponent
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IWebHostEnvironment _webHostEnvironment { get; set; } = default!;
        [Inject] private IPersonelApiService _personelApiService { get; set; } = default!;
        [Inject] private IDepartmanApiService _departmanApiService { get; set; } = default!;
        [Inject] private IServisApiService _servisApiService { get; set; } = default!;
        [Inject] private IUnvanApiService _unvanApiService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiApiService { get; set; } = default!;
        [Inject] private IIlApiService _ilApiService { get; set; } = default!;
        [Inject] private IIlceApiService _ilceApiService { get; set; } = default!;
        [Inject] private IAtanmaNedeniApiService _atanmaNedeniApiService { get; set; } = default!;
        [Inject] private IMapper _mapper { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════

        [Parameter] public string? TcKimlikNo { get; set; }

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private bool IsEditMode => !string.IsNullOrEmpty(TcKimlikNo);

        // Lookup Lists
        private List<DepartmanResponseDto> Departmanlar { get; set; } = new();
        private List<ServisResponseDto> Servisler { get; set; } = new();
        private List<UnvanResponseDto> Unvanlar { get; set; } = new();
        private List<HizmetBinasiResponseDto> HizmetBinalari { get; set; } = new();
        private List<IlResponseDto> Iller { get; set; } = new();
        private List<IlceResponseDto> TumIlceler { get; set; } = new();
        private List<IlceResponseDto> Ilceler { get; set; } = new();
        private List<AtanmaNedeniResponseDto> AtanmaNedenleri { get; set; } = new();
        private bool IsLoading { get; set; } = false;
        private bool IsSaving { get; set; } = false;
        private int CurrentStep { get; set; } = 1; // 1: TC+Ad Soyad, 2: Tüm bilgiler
        private string ActiveTab { get; set; } = "personel";
        private bool IsUploadingImage { get; set; } = false;
        private string UploadErrorMessage { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════
        // MODELS
        // ═══════════════════════════════════════════════════════

        private Step1Model Step1Model { get; set; } = new();
        private PersonelFormModel FormModel { get; set; } = new();

        // Dinamik Listeler
        private List<CocukModel> Cocuklar { get; set; } = new();
        private List<HizmetModel> Hizmetler { get; set; } = new();
        private List<EgitimModel> Egitimler { get; set; } = new();
        private List<YetkiModel> Yetkiler { get; set; } = new();
        private List<CezaModel> Cezalar { get; set; } = new();
        private List<EngelModel> Engeller { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            // Lookup listelerini yükle
            await LoadLookupData();

            if (IsEditMode)
            {
                // Düzenleme modu: Direkt adım 2'ye geç
                CurrentStep = 2;
                await LoadPersonelData();
            }
            else
            {
                // Query string'den TC ve Ad Soyad kontrolü (Index'ten geldiyse)
                var uri = new Uri(_navigationManager.Uri);
                var queryParams = QueryHelpers.ParseQuery(uri.Query);
                
                if (queryParams.TryGetValue("tc", out var tc) && 
                    queryParams.TryGetValue("adsoyad", out var adSoyad) &&
                    !string.IsNullOrWhiteSpace(tc) && 
                    !string.IsNullOrWhiteSpace(adSoyad))
                {
                    // Index'ten geldi, direkt Adım 2'ye geç
                    FormModel.TcKimlikNo = tc.ToString();
                    FormModel.AdSoyad = adSoyad.ToString();
                    FormModel.NickName = GenerateNickName(adSoyad.ToString());
                    CurrentStep = 2;
                }
                else
                {
                    // Yeni ekleme modu: Adım 1'den başla
                    CurrentStep = 1;
                }
            }
        }

        private async Task LoadLookupData()
        {
            try
            {
                var departmanTask = _departmanApiService.GetAllAsync();
                var servisTask = _servisApiService.GetAllAsync();
                var unvanTask = _unvanApiService.GetAllAsync();
                var hizmetBinasiTask = _hizmetBinasiApiService.GetAllAsync();
                var ilTask = _ilApiService.GetAllAsync();
                var ilceTask = _ilceApiService.GetAllAsync();
                var atanmaNedeniTask = _atanmaNedeniApiService.GetAllAsync();

                await Task.WhenAll(departmanTask, servisTask, unvanTask, hizmetBinasiTask, ilTask, ilceTask, atanmaNedeniTask);

                Departmanlar = (await departmanTask)?.Data ?? new List<DepartmanResponseDto>();
                Servisler = (await servisTask)?.Data ?? new List<ServisResponseDto>();
                Unvanlar = (await unvanTask)?.Data ?? new List<UnvanResponseDto>();
                HizmetBinalari = (await hizmetBinasiTask)?.Data ?? new List<HizmetBinasiResponseDto>();
                Iller = (await ilTask)?.Data ?? new List<IlResponseDto>();
                TumIlceler = (await ilceTask)?.Data ?? new List<IlceResponseDto>();
                AtanmaNedenleri = (await atanmaNedeniTask)?.Data ?? new List<AtanmaNedeniResponseDto>();
                
                // İl seçiliyse ilçeleri filtrele
                FilterIlceler();
                
                // Select2'leri initialize et
                await RefreshSelect2();
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Lookup verileri yüklenirken hata: {ex.Message}");
            }
        }

        private async Task LoadPersonelData()
        {
            IsLoading = true;
            try
            {
                // API'den personel verisini getir
                var personel = await _personelApiService.GetByTcKimlikNoAsync(TcKimlikNo!);
                
                if (personel == null)
                {
                    await _toastService.ShowErrorAsync("Personel bulunamadı!");
                    _navigationManager.NavigateTo("/personel");
                    return;
                }

                // DTO'dan FormModel'e dönüştür
                FormModel = MapToFormModel(personel);
                
                // İlçeleri filtrele
                FilterIlceler();
                
                // Select2'leri initialize et
                await RefreshSelect2();
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Personel yüklenirken hata oluştu: {ex.Message}");
                _navigationManager.NavigateTo("/personel");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private PersonelFormModel MapToFormModel(PersonelResponseDto dto)
        {
            return _mapper.Map<PersonelFormModel>(dto);
        }


        // ═══════════════════════════════════════════════════════
        // STEP 1 METHODS (TC + Ad Soyad)
        // ═══════════════════════════════════════════════════════

        private async Task HandleStep1Submit()
        {
            IsSaving = true;
            try
            {
                // Simulate API call - TC kontrolü
                await Task.Delay(500);

                // Adım 1'den Adım 2'ye geç
                FormModel.TcKimlikNo = Step1Model.TcKimlikNo;
                FormModel.AdSoyad = Step1Model.AdSoyad;
                FormModel.NickName = GenerateNickName(Step1Model.AdSoyad);
                
                CurrentStep = 2;
                await _toastService.ShowSuccessAsync("Temel bilgiler kaydedildi. Diğer bilgileri girebilirsiniz.");
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

        // ═══════════════════════════════════════════════════════
        // FINAL SUBMIT (Tüm bilgileri kaydet)
        // ═══════════════════════════════════════════════════════

        private async Task HandleFinalSubmit()
        {
            IsSaving = true;
            try
            {
                // Toplu kayıt DTO'sunu hazırla
                var completeRequest = new PersonelCompleteRequestDto
                {
                    Personel = MapToCreateDto(FormModel),
                    Cocuklar = Cocuklar.Where(c => c.DogumTarihi.HasValue && !string.IsNullOrEmpty(c.Isim)).Select(c => new PersonelCocukCreateRequestDto
                    {
                        PersonelTcKimlikNo = FormModel.TcKimlikNo,
                        CocukAdi = c.Isim,
                        CocukDogumTarihi = DateOnly.FromDateTime(c.DogumTarihi!.Value),
                        OgrenimDurumu = OgrenimDurumu.ilkokul
                    }).ToList(),
                    Hizmetler = Hizmetler.Where(h => h.BaslamaTarihi.HasValue && h.DepartmanId > 0 && h.ServisId > 0).Select(h => new PersonelHizmetCreateRequestDto
                    {
                        TcKimlikNo = FormModel.TcKimlikNo,
                        DepartmanId = h.DepartmanId,
                        ServisId = h.ServisId,
                        GorevBaslamaTarihi = h.BaslamaTarihi!.Value,
                        GorevAyrilmaTarihi = h.AyrilmaTarihi,
                        Sebep = h.Sebep
                    }).ToList(),
                    Egitimler = Egitimler.Where(e => e.BaslangicTarihi.HasValue && !string.IsNullOrEmpty(e.EgitimAdi)).Select(e => new PersonelEgitimCreateRequestDto
                    {
                        TcKimlikNo = FormModel.TcKimlikNo,
                        EgitimAdi = e.EgitimAdi!,
                        EgitimBaslangicTarihi = e.BaslangicTarihi!.Value,
                        EgitimBitisTarihi = e.BitisTarihi,
                        Aciklama = null
                    }).ToList(),
                    ImzaYetkileri = Yetkiler.Where(y => y.ImzaYetkisiBaslamaTarihi.HasValue && y.DepartmanId > 0 && y.ServisId > 0).Select(y => new PersonelImzaYetkisiCreateRequestDto
                    {
                        TcKimlikNo = FormModel.TcKimlikNo,
                        DepartmanId = y.DepartmanId,
                        ServisId = y.ServisId,
                        GorevDegisimSebebi = y.GorevDegisimSebebi,
                        ImzaYetkisiBaslamaTarihi = y.ImzaYetkisiBaslamaTarihi!.Value,
                        ImzaYetkisiBitisTarihi = y.ImzaYetkisiBitisTarihi,
                        Aciklama = null
                    }).ToList(),
                    Cezalar = Cezalar.Where(c => c.CezaTarihi.HasValue && !string.IsNullOrEmpty(c.CezaSebebi)).Select(c => new PersonelCezaCreateRequestDto
                    {
                        TcKimlikNo = FormModel.TcKimlikNo,
                        CezaSebebi = c.CezaSebebi!,
                        AltBendi = c.AltBendi,
                        CezaTarihi = c.CezaTarihi!.Value,
                        Aciklama = null
                    }).ToList(),
                    Engeller = Engeller.Where(e => !string.IsNullOrEmpty(e.EngelNedeni1)).Select(e => new PersonelEngelCreateRequestDto
                    {
                        TcKimlikNo = FormModel.TcKimlikNo,
                        EngelDerecesi = e.EngelDerecesi,
                        EngelNedeni1 = e.EngelNedeni1,
                        EngelNedeni2 = e.EngelNedeni2,
                        EngelNedeni3 = e.EngelNedeni3,
                        Aciklama = null
                    }).ToList()
                };

                if (IsEditMode)
                {
                    // Toplu güncelleme (Transaction)
                    var response = await _personelApiService.UpdateCompleteAsync(FormModel.TcKimlikNo, completeRequest);
                    
                    if (response?.Success == true)
                    {
                        await _toastService.ShowSuccessAsync($"{FormModel.AdSoyad} ve tüm bilgileri başarıyla güncellendi!");
                        _navigationManager.NavigateTo($"/personel/detail/{FormModel.TcKimlikNo}");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(response?.Message ?? "Güncelleme işlemi başarısız oldu!");
                    }
                }
                else
                {
                    // Toplu ekleme (Transaction)
                    var response = await _personelApiService.CreateCompleteAsync(completeRequest);
                    
                    if (response?.Success == true)
                    {
                        await _toastService.ShowSuccessAsync($"{FormModel.AdSoyad} ve tüm bilgileri başarıyla eklendi!");
                        _navigationManager.NavigateTo($"/personel/detail/{FormModel.TcKimlikNo}");
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(response?.Message ?? "Ekleme işlemi başarısız oldu!");
                    }
                }
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("Validation Error"))
            {
                var errorMessage = "Lütfen aşağıdaki alanları kontrol edin:\n";
                if (ex.Message.Contains("Email"))
                    errorMessage += "- Email adresi gerekli\n";
                if (ex.Message.Contains("SicilNo"))
                    errorMessage += "- Sicil No gerekli\n";
                
                await _toastService.ShowErrorAsync(errorMessage);
            }
            catch (HttpRequestException ex)
            {
                await _toastService.ShowErrorAsync(ex.Message);
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"İşlem sırasında hata oluştu: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private PersonelCreateRequestDto MapToCreateDto(PersonelFormModel model)
        {
            return _mapper.Map<PersonelCreateRequestDto>(model);
        }

        private PersonelUpdateRequestDto MapToUpdateDto(PersonelFormModel model)
        {
            return _mapper.Map<PersonelUpdateRequestDto>(model);
        }

        // ═══════════════════════════════════════════════════════
        // TAB METHODS
        private void SetActiveTab(string tabName)
        {
            ActiveTab = tabName;
            StateHasChanged();
        }

        private void OnIlChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int ilId))
            {
                FormModel.IlId = ilId;
                FormModel.IlceId = 0;
                FilterIlceler();
            }
        }

        private void FilterIlceler()
        {
            if (FormModel.IlId > 0)
            {
                Ilceler = TumIlceler.Where(i => i.IlId == FormModel.IlId).ToList();
            }
            else
            {
                Ilceler = new List<IlceResponseDto>();
            }
        }

        private void NavigateToPersonelList()
        {
            _navigationManager.NavigateTo("/personel");
        }

        // ═══════════════════════════════════════════════════════
        // DYNAMIC LIST METHODS
        // ═══════════════════════════════════════════════════════

        // Çocuk
        private void AddCocuk() => Cocuklar.Add(new CocukModel());
        private void RemoveCocuk(int index) => Cocuklar.RemoveAt(index);

        // Hizmet
        private void AddHizmet() => Hizmetler.Add(new HizmetModel());
        private void RemoveHizmet(int index) => Hizmetler.RemoveAt(index);

        // Eğitim
        private void AddEgitim() => Egitimler.Add(new EgitimModel());
        private void RemoveEgitim(int index) => Egitimler.RemoveAt(index);

        // Yetki
        private void AddYetki() => Yetkiler.Add(new YetkiModel());
        private void RemoveYetki(int index) => Yetkiler.RemoveAt(index);

        // Ceza
        private void AddCeza() => Cezalar.Add(new CezaModel());
        private void RemoveCeza(int index) => Cezalar.RemoveAt(index);

        // Engel
        private void AddEngel() => Engeller.Add(new EngelModel());
        private void RemoveEngel(int index) => Engeller.RemoveAt(index);

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

        private string GetDepartmanAdi(int id)
        {
            return Departmanlar.FirstOrDefault(d => d.DepartmanId == id)?.DepartmanAdi ?? "-";
        }

        private string GetServisAdi(int id)
        {
            return Servisler.FirstOrDefault(s => s.ServisId == id)?.ServisAdi ?? "-";
        }

        private string GetUnvanAdi(int id)
        {
            return Unvanlar.FirstOrDefault(u => u.UnvanId == id)?.UnvanAdi ?? "-";
        }

        private string GenerateNickName(string adSoyad)
        {
            if (string.IsNullOrWhiteSpace(adSoyad))
                return string.Empty;

            var parts = adSoyad.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return string.Empty;

            if (parts.Length == 1)
                return parts[0].ToUpper();

            // Son kelime soyad, diğerleri ad
            var soyad = parts[^1].ToUpper();
            var adIlkHarfler = string.Join(".", parts.Take(parts.Length - 1).Select(p => p[0].ToString().ToUpper()));

            return $"{adIlkHarfler}.{soyad}";
        }

        private string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        // ═══════════════════════════════════════════════════════
        // IMAGE UPLOAD METHODS
        // ═══════════════════════════════════════════════════════

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            UploadErrorMessage = string.Empty;
            IsUploadingImage = true;

            try
            {
                var file = e.File;

                // TC Kimlik No kontrolü
                if (string.IsNullOrWhiteSpace(FormModel.TcKimlikNo))
                {
                    UploadErrorMessage = "Fotoğraf yüklemek için önce TC Kimlik No girilmelidir.";
                    await _toastService.ShowErrorAsync(UploadErrorMessage);
                    return;
                }

                // Dosya boyutu kontrolü (5MB)
                const long maxFileSize = 5 * 1024 * 1024;
                if (file.Size > maxFileSize)
                {
                    UploadErrorMessage = "Dosya boyutu 5MB'dan büyük olamaz.";
                    return;
                }

                // Dosya tipi kontrolü
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    UploadErrorMessage = "Sadece JPG, PNG ve GIF formatları desteklenmektedir.";
                    return;
                }

                // Dosya uzantısını al
                var extension = Path.GetExtension(file.Name).ToLower();
                if (string.IsNullOrEmpty(extension))
                {
                    extension = file.ContentType switch
                    {
                        "image/jpeg" => ".jpg",
                        "image/jpg" => ".jpg",
                        "image/png" => ".png",
                        "image/gif" => ".gif",
                        _ => ".jpg"
                    };
                }

                // Dosya adı: TcKimlikNo.extension (örn: 12345678901.jpg)
                var fileName = $"{FormModel.TcKimlikNo}{extension}";

                // wwwroot/images/avatars klasörünü oluştur
                var avatarFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                if (!Directory.Exists(avatarFolder))
                {
                    Directory.CreateDirectory(avatarFolder);
                }

                // Tam dosya yolu
                var filePath = Path.Combine(avatarFolder, fileName);

                // Eski dosyayı sil (eğer varsa)
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Dosyayı kaydet
                using var stream = file.OpenReadStream(maxFileSize);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await stream.CopyToAsync(fileStream);

                // Veritabanı için relatif yol
                FormModel.Resim = $"/images/avatars/{fileName}";

                await _toastService.ShowSuccessAsync($"Fotoğraf başarıyla yüklendi: {fileName}");
            }
            catch (Exception ex)
            {
                UploadErrorMessage = $"Fotoğraf yüklenirken hata oluştu: {ex.Message}";
                await _toastService.ShowErrorAsync(UploadErrorMessage);
            }
            finally
            {
                IsUploadingImage = false;
                StateHasChanged();
            }
        }

        private async Task RemovePhoto()
        {
            try
            {
                // Eğer dosya sunucuda varsa sil
                if (!string.IsNullOrEmpty(FormModel.Resim) && FormModel.Resim.StartsWith("/images/avatars/"))
                {
                    var fileName = Path.GetFileName(FormModel.Resim);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars", fileName);
                    
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                FormModel.Resim = string.Empty;
                await _toastService.ShowSuccessAsync("Fotoğraf kaldırıldı.");
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Fotoğraf kaldırılırken hata oluştu: {ex.Message}");
            }
            finally
            {
                StateHasChanged();
            }
        }
    }
}
