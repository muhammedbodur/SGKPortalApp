using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
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
using SGKPortalApp.PresentationLayer.Helpers;
using AutoMapper;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;
using SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.StateServices;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.PresentationLayer.Pages.Personel
{
    public partial class Manage : FieldPermissionPageBase
    {
        // ═══════════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════════

        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IWebHostEnvironment _webHostEnvironment { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;
        [Inject] private ImageHelper _imageHelper { get; set; } = default!;
        [Inject] private IPersonelApiService _personelApiService { get; set; } = default!;
        [Inject] private IDepartmanApiService _departmanApiService { get; set; } = default!;
        [Inject] private IServisApiService _servisApiService { get; set; } = default!;
        [Inject] private IUnvanApiService _unvanApiService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiApiService { get; set; } = default!;
        [Inject] private IDepartmanHizmetBinasiApiService _departmanHizmetBinasiApiService { get; set; } = default!;
        [Inject] private ISendikaApiService _sendikaApiService { get; set; } = default!;
        [Inject] private IIlApiService _ilApiService { get; set; } = default!;
        [Inject] private IIlceApiService _ilceApiService { get; set; } = default!;
        [Inject] private IAtanmaNedeniApiService _atanmaNedeniApiService { get; set; } = default!;
        [Inject] private IMapper _mapper { get; set; } = default!;
        [Inject] private IUserInfoService _userInfoService { get; set; } = default!;
        [Inject] private PersonelImagePathHelper _imagePathHelper { get; set; } = default!;

        // ═══════════════════════════════════════════════════════════
        // PARAMETERS
        // ═══════════════════════════════════════════════════════════

        [Parameter] public string? TcKimlikNo { get; set; }

        // ═══════════════════════════════════════════════════════════
        // FIELD PERMISSION CONFIGURATION (FieldPermissionPageBase)
        // ═══════════════════════════════════════════════════════════

        // ⚡ PagePermissionKey artık otomatik çözümleniyor!
        // Route: /personel/manage → PermissionKey: PER.PERSONEL.MANAGE
        // Route: /personel/manage/{tcKimlikNo} → PermissionKey: PER.PERSONEL.MANAGE
        // Manuel override gerekmez, FieldPermissionPageBase otomatik halleder

        /// <summary>
        /// Edit modu: TcKimlikNo parametresi varsa true
        /// </summary>
        protected override bool IsEditMode => !string.IsNullOrEmpty(TcKimlikNo);

        /// <summary>
        /// Field permission prefix'i: Her zaman PER.PERSONEL.MANAGE (field permission sadece Edit'te aktif)
        /// Not: Create modunda field-level permission uygulanmaz, sadece Edit modunda aktif
        /// </summary>

        // Lookup Lists
        private List<DepartmanResponseDto> Departmanlar { get; set; } = new();
        private List<ServisResponseDto> Servisler { get; set; } = new();
        private List<UnvanResponseDto> Unvanlar { get; set; } = new();
        private List<HizmetBinasiResponseDto> HizmetBinalari { get; set; } = new();
        private List<DepartmanHizmetBinasiResponseDto> DepartmanHizmetBinalari { get; set; } = new();
        private List<SendikaResponseDto> Sendikalar { get; set; } = new();
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
        private List<ImzaYetkisiModel> Yetkiler { get; set; } = new();
        private List<CezaModel> Cezalar { get; set; } = new();
        private List<EngelModel> Engeller { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            // Base class permission yükleme ve event subscription'ı yapar
            await base.OnInitializedAsync();

            if (IsEditMode)
            {
                // Düzenleme modu: Lookup ve Personel verisini PARALEL yükle (performans iyileştirmesi)
                CurrentStep = 2;
                IsLoading = true;

                var lookupTask = LoadLookupDataAsync();
                var personelTask = _personelApiService.GetByTcKimlikNoAsync(TcKimlikNo!);

                await Task.WhenAll(lookupTask, personelTask);

                // Personel verisini işle
                var result = await personelTask;
                if (!result.Success || result.Data == null)
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Personel bulunamadı!");
                    _navigationManager.NavigateTo("/personel");
                    return;
                }

                await ProcessPersonelDataAsync(result.Data);
                IsLoading = false;
            }
            else
            {
                // Yeni ekleme modu: Sadece lookup yükle
                await LoadLookupDataAsync();

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
                    FormModel.NickName = StringHelper.GenerateNickName(adSoyad.ToString(), 8);
                    CurrentStep = 2;
                }
                else
                {
                    // Yeni ekleme modu: Adım 1'den başla
                    CurrentStep = 1;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private async Task LoadLookupDataAsync()
        {
            try
            {
                var departmanTask = _departmanApiService.GetAllAsync();
                var servisTask = _servisApiService.GetAllAsync();
                var unvanTask = _unvanApiService.GetAllAsync();
                var hizmetBinasiTask = _hizmetBinasiApiService.GetAllAsync();
                var sendikaTask = _sendikaApiService.GetAllAsync();
                var ilTask = _ilApiService.GetAllAsync();
                var ilceTask = _ilceApiService.GetAllAsync();
                var atanmaNedeniTask = _atanmaNedeniApiService.GetAllAsync();

                await Task.WhenAll(departmanTask, servisTask, unvanTask, hizmetBinasiTask, sendikaTask, ilTask, ilceTask, atanmaNedeniTask);

                Departmanlar = (await departmanTask)?.Data ?? new List<DepartmanResponseDto>();
                Servisler = (await servisTask)?.Data ?? new List<ServisResponseDto>();
                Unvanlar = (await unvanTask)?.Data ?? new List<UnvanResponseDto>();
                HizmetBinalari = (await hizmetBinasiTask)?.Data ?? new List<HizmetBinasiResponseDto>();
                Sendikalar = (await sendikaTask)?.Data ?? new List<SendikaResponseDto>();
                Iller = (await ilTask)?.Data ?? new List<IlResponseDto>();
                TumIlceler = (await ilceTask)?.Data ?? new List<IlceResponseDto>();
                AtanmaNedenleri = (await atanmaNedeniTask)?.Data ?? new List<AtanmaNedeniResponseDto>();

                // İl seçiliyse ilçeleri filtrele
                FilterIlceler();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lookup verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync($"Lookup verileri yüklenirken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// API'den gelen personel verisini FormModel ve collection'lara aktar
        /// </summary>
        private async Task ProcessPersonelDataAsync(PersonelResponseDto data)
        {
            // DTO'dan FormModel'e dönüştürme
            FormModel = MapToFormModel(data);

            // Collection'ları yükle
            Cocuklar = data.Cocuklar?.Select(c => new CocukModel
            {
                Isim = c.CocukAdi,
                DogumTarihi = c.CocukDogumTarihi.ToDateTime(TimeOnly.MinValue),
                OgrenimDurumu = c.OgrenimDurumu
            }).ToList() ?? new List<CocukModel>();

            Hizmetler = data.Hizmetler?.Select(h => new HizmetModel
            {
                DepartmanId = h.DepartmanId,
                Departman = h.DepartmanAdi,
                ServisId = h.ServisId,
                Servis = h.ServisAdi,
                BaslamaTarihi = h.GorevBaslamaTarihi,
                AyrilmaTarihi = h.GorevAyrilmaTarihi,
                Sebep = h.Sebep
            }).ToList() ?? new List<HizmetModel>();

            Egitimler = data.Egitimler?.Select(e => new EgitimModel
            {
                EgitimAdi = e.EgitimAdi,
                BaslangicTarihi = e.EgitimBaslangicTarihi,
                BitisTarihi = e.EgitimBitisTarihi
            }).ToList() ?? new List<EgitimModel>();

            Yetkiler = data.ImzaYetkileriDetay?.Select(y => new ImzaYetkisiModel
            {
                DepartmanId = y.DepartmanId,
                ServisId = y.ServisId,
                GorevDegisimSebebi = y.GorevDegisimSebebi,
                ImzaYetkisiBaslamaTarihi = y.ImzaYetkisiBaslamaTarihi,
                ImzaYetkisiBitisTarihi = y.ImzaYetkisiBitisTarihi
            }).ToList() ?? new List<ImzaYetkisiModel>();

            Cezalar = data.Cezalar?.Select(c => new CezaModel
            {
                CezaSebebi = c.CezaSebebi,
                AltBendi = c.AltBendi,
                CezaTarihi = c.CezaTarihi
            }).ToList() ?? new List<CezaModel>();

            Engeller = data.Engeller?.Select(e => new EngelModel
            {
                EngelDerecesi = e.EngelDerecesi,
                EngelNedeni1 = e.EngelNedeni1,
                EngelNedeni2 = e.EngelNedeni2,
                EngelNedeni3 = e.EngelNedeni3
            }).ToList() ?? new List<EngelModel>();

            // İlçeleri filtrele
            FilterIlceler();

            // Departmana bağlı hizmet binalarını yükle
            if (FormModel.DepartmanId > 0)
            {
                await LoadDepartmanHizmetBinalariAsync(FormModel.DepartmanId);
            }

            // Select2'leri initialize et
            await RefreshSelect2();
        }

        private PersonelFormModel MapToFormModel(PersonelResponseDto dto)
        {
            return _mapper.Map<PersonelFormModel>(dto);
        }


        // ═══════════════════════════════════════════════════════
        //  DROPDOWN CHANGE HANDLERS - GÜVENLİ PARSE
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// DepartmanHizmetBinasi değişiklik handler - GÜVENLİ INT PARSE
        /// </summary>
        private void OnDepartmanHizmetBinasiChanged(ChangeEventArgs e)
        {
            try
            {
                var value = e.Value?.ToString();
                Console.WriteLine($" [DEPARTMAN HIZMET BINASI] Raw Value: '{value}'");

                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine(" [DEPARTMAN HIZMET BINASI] Boş değer");
                    FormModel.DepartmanHizmetBinasiId = 0;
                    StateHasChanged();
                    return;
                }

                if (int.TryParse(value, out int parsedValue))
                {
                    FormModel.DepartmanHizmetBinasiId = parsedValue;
                    Console.WriteLine($" [DEPARTMAN HIZMET BINASI] Parse başarılı: {parsedValue}");

                    // Seçilen binayı logla
                    var selectedBina = DepartmanHizmetBinalari.FirstOrDefault(b => b.DepartmanHizmetBinasiId == parsedValue);
                    if (selectedBina != null)
                    {
                        Console.WriteLine($"    Seçilen Bina: {selectedBina.HizmetBinasiAdi}");
                    }
                }
                else
                {
                    Console.WriteLine($" [DEPARTMAN HIZMET BINASI] Parse BAŞARISIZ: '{value}'");
                    FormModel.DepartmanHizmetBinasiId = 0;
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [DEPARTMAN HIZMET BINASI] EXCEPTION: {ex.Message}");
                FormModel.DepartmanHizmetBinasiId = 0;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Departman değişiklik handler - GÜVENLİ INT PARSE + DepartmanHizmetBinalari yükleme
        /// </summary>
        private async Task OnDepartmanChangedAsync(ChangeEventArgs e)
        {
            try
            {
                var value = e.Value?.ToString();
                if (int.TryParse(value, out int parsedValue))
                {
                    FormModel.DepartmanId = parsedValue;
                    FormModel.DepartmanHizmetBinasiId = 0; // Bina seçimini sıfırla
                    Console.WriteLine($" [DEPARTMAN] ID: {parsedValue}");

                    // Seçilen departmana bağlı hizmet binalarını yükle
                    await LoadDepartmanHizmetBinalariAsync(parsedValue);
                }
                else
                {
                    FormModel.DepartmanId = 0;
                    FormModel.DepartmanHizmetBinasiId = 0;
                    DepartmanHizmetBinalari = new List<DepartmanHizmetBinasiResponseDto>();
                    Console.WriteLine($" [DEPARTMAN] Parse başarısız: '{value}'");
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [DEPARTMAN] EXCEPTION: {ex.Message}");
                FormModel.DepartmanId = 0;
                FormModel.DepartmanHizmetBinasiId = 0;
                DepartmanHizmetBinalari = new List<DepartmanHizmetBinasiResponseDto>();
            }
        }

        /// <summary>
        /// Seçilen departmana bağlı DepartmanHizmetBinalari listesini yükler
        /// </summary>
        private async Task LoadDepartmanHizmetBinalariAsync(int departmanId)
        {
            try
            {
                if (departmanId <= 0)
                {
                    DepartmanHizmetBinalari = new List<DepartmanHizmetBinasiResponseDto>();
                    return;
                }

                var result = await _departmanHizmetBinasiApiService.GetByDepartmanAsync(departmanId);
                if (result?.Success == true && result.Data != null)
                {
                    DepartmanHizmetBinalari = result.Data;
                    Console.WriteLine($"   📍 {DepartmanHizmetBinalari.Count} hizmet binası yüklendi (Departman: {departmanId})");
                }
                else
                {
                    DepartmanHizmetBinalari = new List<DepartmanHizmetBinasiResponseDto>();
                    Console.WriteLine($"   ⚠️ Departman için hizmet binası bulunamadı (Departman: {departmanId})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [DepartmanHizmetBinalari] EXCEPTION: {ex.Message}");
                DepartmanHizmetBinalari = new List<DepartmanHizmetBinasiResponseDto>();
            }
        }

        /// <summary>
        /// Servis değişiklik handler - GÜVENLİ INT PARSE
        /// </summary>
        private void OnServisChanged(ChangeEventArgs e)
        {
            try
            {
                var value = e.Value?.ToString();
                if (int.TryParse(value, out int parsedValue))
                {
                    FormModel.ServisId = parsedValue;
                    Console.WriteLine($" [SERVIS] ID: {parsedValue}");
                }
                else
                {
                    FormModel.ServisId = 0;
                    Console.WriteLine($" [SERVIS] Parse başarısız: '{value}'");
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [SERVIS] EXCEPTION: {ex.Message}");
                FormModel.ServisId = 0;
            }
        }

        /// <summary>
        /// Ünvan değişiklik handler - GÜVENLİ INT PARSE
        /// </summary>
        private void OnUnvanChanged(ChangeEventArgs e)
        {
            try
            {
                var value = e.Value?.ToString();
                if (int.TryParse(value, out int parsedValue))
                {
                    FormModel.UnvanId = parsedValue;
                    Console.WriteLine($" [ÜNVAN] ID: {parsedValue}");
                }
                else
                {
                    FormModel.UnvanId = 0;
                    Console.WriteLine($" [ÜNVAN] Parse başarısız: '{value}'");
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [ÜNVAN] EXCEPTION: {ex.Message}");
                FormModel.UnvanId = 0;
            }
        }

        /// <summary>
        /// İl değişiklik handler - GÜVENLİ INT PARSE + İLÇE FİLTRELEME
        /// </summary>
        private void OnIlChanged(ChangeEventArgs e)
        {
            try
            {
                var value = e.Value?.ToString();
                Console.WriteLine($" [İL] Raw Value: '{value}'");

                if (string.IsNullOrWhiteSpace(value))
                {
                    FormModel.IlId = 0;
                    FormModel.IlceId = 0;
                    Ilceler = new List<IlceResponseDto>();
                    StateHasChanged();
                    return;
                }

                if (int.TryParse(value, out int ilId))
                {
                    FormModel.IlId = ilId;
                    FormModel.IlceId = 0; // İlçe sıfırlansın
                    Console.WriteLine($" [İL] Parse başarılı: {ilId}");

                    // İlçeleri filtrele
                    FilterIlceler();
                    Console.WriteLine($"   📍 {Ilceler.Count} ilçe yüklendi");
                }
                else
                {
                    Console.WriteLine($" [İL] Parse başarısız: '{value}'");
                    FormModel.IlId = 0;
                    FormModel.IlceId = 0;
                    Ilceler = new List<IlceResponseDto>();
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [İL] EXCEPTION: {ex.Message}");
                FormModel.IlId = 0;
                FormModel.IlceId = 0;
                Ilceler = new List<IlceResponseDto>();
            }
        }

        /// <summary>
        /// İlçe değişiklik handler - GÜVENLİ INT PARSE
        /// </summary>
        private void OnIlceChanged(ChangeEventArgs e)
        {
            try
            {
                var value = e.Value?.ToString();
                if (int.TryParse(value, out int parsedValue))
                {
                    FormModel.IlceId = parsedValue;
                    Console.WriteLine($" [İLÇE] ID: {parsedValue}");
                }
                else
                {
                    FormModel.IlceId = 0;
                    Console.WriteLine($" [İLÇE] Parse başarısız: '{value}'");
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [İLÇE] EXCEPTION: {ex.Message}");
                FormModel.IlceId = 0;
            }
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
                FormModel.NickName = StringHelper.GenerateNickName(Step1Model.AdSoyad, 8);

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
            if (!CanEditPage)
            {
                await _toastService.ShowErrorAsync("Bu işlem için yetkiniz bulunmuyor.");
                return;
            }

            //  DETAYLI VALİDASYON + DEBUG LOG
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("🚀 FORM SUBMIT BAŞLADI");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine($"📋 TC Kimlik No: {FormModel.TcKimlikNo}");
            Console.WriteLine($"📋 Ad Soyad: {FormModel.AdSoyad}");
            Console.WriteLine($"📋 Email: {FormModel.Email}");
            Console.WriteLine($"📋 Sicil No: {FormModel.SicilNo}");
            Console.WriteLine($"───────────────────────────────────────────────────────");
            Console.WriteLine($" HizmetBinasiId: {FormModel.DepartmanHizmetBinasiId}");
            Console.WriteLine($" DepartmanId: {FormModel.DepartmanId}");
            Console.WriteLine($" ServisId: {FormModel.ServisId}");
            Console.WriteLine($" UnvanId: {FormModel.UnvanId}");
            Console.WriteLine($" AtanmaNedeniId: {FormModel.AtanmaNedeniId}");
            Console.WriteLine($"───────────────────────────────────────────────────────");
            Console.WriteLine($"📍 IlId: {FormModel.IlId}");
            Console.WriteLine($"📍 IlceId: {FormModel.IlceId}");
            Console.WriteLine("═══════════════════════════════════════════════════════");

            // ✅ DETAYLI VALİDASYON
            var validationErrors = new List<string>();

            // Temel bilgiler
            if (string.IsNullOrWhiteSpace(FormModel.TcKimlikNo) || FormModel.TcKimlikNo.Length != 11)
                validationErrors.Add("❌ TC Kimlik No 11 haneli olmalıdır");

            if (string.IsNullOrWhiteSpace(FormModel.AdSoyad))
                validationErrors.Add("❌ Ad Soyad zorunludur");

            // Email opsiyonel - boşlukları temizle
            if (!string.IsNullOrWhiteSpace(FormModel.Email))
            {
                FormModel.Email = FormModel.Email.Trim();
                if (!FormModel.Email.Contains("@"))
                    validationErrors.Add("❌ Geçerli bir email adresi giriniz");
            }
            else
            {
                FormModel.Email = null;
            }

            // SicilNo opsiyonel - 0 ise null yap
            if (FormModel.SicilNo == 0)
                FormModel.SicilNo = null;

            // Kurum bilgileri
            if (FormModel.DepartmanHizmetBinasiId == 0)
                validationErrors.Add("❌ Hizmet Binası seçimi zorunludur");

            if (FormModel.DepartmanId == 0)
                validationErrors.Add("❌ Departman seçimi zorunludur");

            if (FormModel.ServisId == 0)
                validationErrors.Add("❌ Servis seçimi zorunludur");

            if (FormModel.UnvanId == 0)
                validationErrors.Add("❌ Ünvan seçimi zorunludur");

            // İletişim bilgileri
            if (FormModel.IlId == 0)
                validationErrors.Add("❌ İl seçimi zorunludur");

            if (FormModel.IlceId == 0)
                validationErrors.Add("❌ İlçe seçimi zorunludur");

            if (validationErrors.Any())
            {
                Console.WriteLine("❌ VALİDASYON HATALARI:");
                foreach (var error in validationErrors)
                {
                    Console.WriteLine($"   {error}");
                }

                var errorMessage = "⚠️ Lütfen aşağıdaki alanları kontrol ediniz:\n\n" + string.Join("\n", validationErrors);
                await _toastService.ShowErrorAsync(errorMessage);
                return;
            }

            Console.WriteLine(" Tüm validasyonlar geçti!");

            IsSaving = true;
            try
            {
                // Toplu kayıt DTO'sunu hazırla
                var completeRequest = MapToCompleteDto(FormModel);

                Console.WriteLine($"🚀 API'ye gönderiliyor... DepartmanHizmetBinasiId: {completeRequest.Personel.DepartmanHizmetBinasiId}");

                if (IsEditMode)
                {
                    // Toplu güncelleme (Transaction)
                    var response = await _personelApiService.UpdateCompleteAsync(FormModel.TcKimlikNo, completeRequest);

                    if (response?.Success == true)
                    {
                        Console.WriteLine(" Güncelleme başarılı!");
                        await _toastService.ShowSuccessAsync($"{FormModel.AdSoyad} ve tüm bilgileri başarıyla güncellendi!");
                        _navigationManager.NavigateTo($"/personel/detail/{FormModel.TcKimlikNo}");
                    }
                    else
                    {
                        Console.WriteLine($" API Hatası: {response?.Message}");
                        
                        // Hata mesajlarını kullanıcı dostu hale getir
                        var userFriendlyMessage = ParseApiErrorMessage(response?.Message);
                        
                        if (response?.Message != null && response.Message.Any())
                        {
                            Console.WriteLine(" Detaylı Hatalar:");
                            foreach (var error in response.Message)
                            {
                                Console.WriteLine($"   • {error}");
                            }
                        }
                        
                        await _toastService.ShowErrorAsync(userFriendlyMessage);
                    }
                }
                else
                {
                    // Toplu ekleme (Transaction)
                    var response = await _personelApiService.CreateCompleteAsync(completeRequest);

                    if (response?.Success == true)
                    {
                        Console.WriteLine(" Ekleme başarılı!");
                        await _toastService.ShowSuccessAsync($"{FormModel.AdSoyad} ve tüm bilgileri başarıyla eklendi!");
                        _navigationManager.NavigateTo($"/personel/detail/{FormModel.TcKimlikNo}");
                    }
                    else
                    {
                        Console.WriteLine($" API Hatası: {response?.Message}");
                        
                        // Hata mesajlarını kullanıcı dostu hale getir
                        var userFriendlyMessage = ParseApiErrorMessage(response?.Message);
                        
                        if (response?.Message != null && response.Message.Any())
                        {
                            Console.WriteLine(" Detaylı Hatalar:");
                            foreach (var error in response.Message)
                            {
                                Console.WriteLine($"   • {error}");
                            }
                        }
                        
                        await _toastService.ShowErrorAsync(userFriendlyMessage);
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
                Console.WriteLine($" HTTP Exception: {ex.Message}");
                var userFriendlyMessage = ParseHttpErrorMessage(ex.Message);
                await _toastService.ShowErrorAsync(userFriendlyMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Genel Exception: {ex.Message}");
                Console.WriteLine($"   StackTrace: {ex.StackTrace}");
                
                var userFriendlyMessage = ParseGeneralErrorMessage(ex.Message);
                await _toastService.ShowErrorAsync(userFriendlyMessage);
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

        private PersonelCompleteRequestDto MapToCompleteDto(PersonelFormModel model)
        {
            var completeDto = new PersonelCompleteRequestDto
            {
                Personel = MapToCreateDto(model),
                Cocuklar = Cocuklar.Where(c => c.DogumTarihi.HasValue && !string.IsNullOrEmpty(c.Isim)).Select(c => new PersonelCocukCreateRequestDto
                {
                    PersonelTcKimlikNo = model.TcKimlikNo,
                    CocukAdi = c.Isim,
                    CocukDogumTarihi = DateOnly.FromDateTime(c.DogumTarihi!.Value),
                    OgrenimDurumu = c.OgrenimDurumu
                }).ToList(),
                Hizmetler = Hizmetler.Where(h => h.BaslamaTarihi.HasValue && h.DepartmanId > 0 && h.ServisId > 0).Select(h => new PersonelHizmetCreateRequestDto
                {
                    TcKimlikNo = model.TcKimlikNo,
                    DepartmanId = h.DepartmanId,
                    ServisId = h.ServisId,
                    GorevBaslamaTarihi = h.BaslamaTarihi!.Value,
                    GorevAyrilmaTarihi = h.AyrilmaTarihi,
                    Sebep = h.Sebep
                }).ToList(),
                Egitimler = Egitimler.Where(e => e.BaslangicTarihi.HasValue && !string.IsNullOrEmpty(e.EgitimAdi)).Select(e => new PersonelEgitimCreateRequestDto
                {
                    TcKimlikNo = model.TcKimlikNo,
                    EgitimAdi = e.EgitimAdi!,
                    EgitimBaslangicTarihi = e.BaslangicTarihi!.Value,
                    EgitimBitisTarihi = e.BitisTarihi,
                    Aciklama = null
                }).ToList(),
                ImzaYetkileri = Yetkiler.Where(y => y.ImzaYetkisiBaslamaTarihi.HasValue && y.DepartmanId > 0 && y.ServisId > 0).Select(y => new PersonelImzaYetkisiCreateRequestDto
                {
                    TcKimlikNo = model.TcKimlikNo,
                    DepartmanId = y.DepartmanId!.Value,
                    ServisId = y.ServisId!.Value,
                    GorevDegisimSebebi = y.GorevDegisimSebebi,
                    ImzaYetkisiBaslamaTarihi = y.ImzaYetkisiBaslamaTarihi!.Value,
                    ImzaYetkisiBitisTarihi = y.ImzaYetkisiBitisTarihi,
                    Aciklama = null
                }).ToList(),
                Cezalar = Cezalar.Where(c => c.CezaTarihi.HasValue && !string.IsNullOrEmpty(c.CezaSebebi)).Select(c => new PersonelCezaCreateRequestDto
                {
                    TcKimlikNo = model.TcKimlikNo,
                    CezaSebebi = c.CezaSebebi!,
                    AltBendi = c.AltBendi,
                    CezaTarihi = c.CezaTarihi!.Value,
                    Aciklama = null
                }).ToList(),
                Engeller = Engeller.Where(e => !string.IsNullOrEmpty(e.EngelNedeni1)).Select(e => new PersonelEngelCreateRequestDto
                {
                    TcKimlikNo = model.TcKimlikNo,
                    EngelDerecesi = e.EngelDerecesi,
                    EngelNedeni1 = e.EngelNedeni1,
                    EngelNedeni2 = e.EngelNedeni2,
                    EngelNedeni3 = e.EngelNedeni3,
                    Aciklama = null
                }).ToList()
            };
            return completeDto;
        }

        // ═══════════════════════════════════════════════════════
        // TAB METHODS
        // ═══════════════════════════════════════════════════════

        private void SetActiveTab(string tabName)
        {
            ActiveTab = tabName;
            StateHasChanged();
        }

        // ═══════════════════════════════════════════════════════
        // TAB HELPER METHODS (Blazor Server Compatible)
        // ═══════════════════════════════════════════════════════

        private void SetPersonelTab() => SetActiveTab("personel");
        private void SetIletisimTab() => SetActiveTab("iletisim");
        private void SetKisiselTab() => SetActiveTab("kisisel");
        private void SetOzlukTab() => SetActiveTab("ozluk");
        private void SetEsCocukTab() => SetActiveTab("es-cocuk");
        private void SetHizmetTab() => SetActiveTab("hizmet");
        private void SetEgitimTab() => SetActiveTab("egitim");
        private void SetYetkiTab() => SetActiveTab("yetki");
        private void SetCezaTab() => SetActiveTab("ceza");
        private void SetEngelTab() => SetActiveTab("engel");
        private void SetFotografTab() => SetActiveTab("fotograf");

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
        private void AddCocuk()
        {
            Cocuklar.Add(new CocukModel());
            StateHasChanged();
        }
        
        private void RemoveCocuk(int index)
        {
            Cocuklar.RemoveAt(index);
            StateHasChanged();
        }

        // Hizmet
        private void AddHizmet()
        {
            Hizmetler.Add(new HizmetModel());
            StateHasChanged();
        }
        
        private void RemoveHizmet(int index)
        {
            Hizmetler.RemoveAt(index);
            StateHasChanged();
        }

        // Eğitim
        private void AddEgitim()
        {
            Egitimler.Add(new EgitimModel());
            StateHasChanged();
        }
        
        private void RemoveEgitim(int index)
        {
            Egitimler.RemoveAt(index);
            StateHasChanged();
        }

        // Yetki
        private void AddYetki()
        {
            Yetkiler.Add(new ImzaYetkisiModel());
            StateHasChanged();
        }
        
        private void RemoveYetki(int index)
        {
            Yetkiler.RemoveAt(index);
            StateHasChanged();
        }

        // Ceza
        private void AddCeza()
        {
            Cezalar.Add(new CezaModel());
            StateHasChanged();
        }
        
        private void RemoveCeza(int index)
        {
            Cezalar.RemoveAt(index);
            StateHasChanged();
        }

        // Engel
        private void AddEngel()
        {
            Engeller.Add(new EngelModel());
            StateHasChanged();
        }
        
        private void RemoveEngel(int index)
        {
            Engeller.RemoveAt(index);
            StateHasChanged();
        }

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

        // ═══════════════════════════════════════════════════════
        // IMAGE UPLOAD METHODS
        // ═══════════════════════════════════════════════════════

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            UploadErrorMessage = string.Empty;
            IsUploadingImage = true;
            StateHasChanged();

            try
            {
                var file = e.File;

                if (string.IsNullOrWhiteSpace(FormModel.TcKimlikNo))
                {
                    UploadErrorMessage = " Fotoğraf yüklemek için önce TC Kimlik No girilmelidir.";
                    await _toastService.ShowErrorAsync(UploadErrorMessage);
                    return;
                }

                const long maxFileSize = 2 * 1024 * 1024; // 2MB
                if (file.Size > maxFileSize)
                {
                    UploadErrorMessage = $" Dosya boyutu çok büyük! (Maks: 2MB, Mevcut: {file.Size / 1024 / 1024:F2}MB)";
                    await _toastService.ShowErrorAsync(UploadErrorMessage);
                    return;
                }

                _logger.LogInformation("Personel resmi yükleniyor: {TcKimlikNo}, Dosya boyutu: {FileSize} bytes",
                    FormModel.TcKimlikNo, file.Size);

                // ============================ ÇÖZÜM BURADA ============================
                // Blazor'dan gelen geçici stream'i kalıcı bir MemoryStream'e kopyalıyoruz.
                await using var memoryStream = new MemoryStream();
                using var imageStreamFromBlazor = file.OpenReadStream(maxFileSize);
                await imageStreamFromBlazor.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Stream'in başına dönmeyi unutmuyoruz!
                                           // =======================================================================

                // Artık ImageHelper'a güvenli, kalıcı kopyayı gönderiyoruz.
                var optimizedImage = await _imageHelper.LoadResizeAndOptimizeAsync(
                    memoryStream, // GEÇİCİ STREAM YERİNE HAFIZADAKİ KOPYAYI VER
                    maxWidth: 400,
                    maxHeight: 400,
                    quality: 85
                );

                if (optimizedImage == null)
                {
                    UploadErrorMessage = " Geçersiz resim dosyası! Desteklenen formatlar: JPG, PNG, GIF, WEBP vb. veya dosya bozuk.";
                    await _toastService.ShowErrorAsync(UploadErrorMessage);
                    return;
                }

                _logger.LogInformation("Resim işlendi: Orijinal {OriginalSize}KB → Optimize {OptimizedSize}KB",
                    file.Size / 1024, optimizedImage.Length / 1024);

                var fileName = _imageHelper.GenerateSafeFileName(FormModel.TcKimlikNo, ".jpg");

                if (!string.IsNullOrEmpty(FormModel.Resim))
                {
                    _imageHelper.DeleteImage(FormModel.Resim);
                }

                var imagePath = await _imageHelper.SaveImageAsync(
                    optimizedImage,
                    fileName,
                    subfolder: "avatars"
                );

                // Dosya sisteminin güncellenmesi için kısa bir bekleme
                await Task.Delay(100);

                // ⭐ DB'ye sadece filename kaydet (yeni standart)
                var filenameOnly = _imagePathHelper.ExtractFilename(imagePath);

                // ⭐ KRITIK: Resmi önce null yap, sonra set et
                // Bu sayede CachedImage component'i tamamen yeniden render olur
                FormModel.Resim = null;
                StateHasChanged();
                await Task.Delay(50);
                
                FormModel.Resim = filenameOnly;
                StateHasChanged();

                if (IsEditMode)
                {
                    var updateDto = _mapper.Map<PersonelUpdateRequestDto>(FormModel);
                    var response = await _personelApiService.UpdateAsync(FormModel.TcKimlikNo, updateDto);
                    if (response?.Success == true)
                    {
                        await _toastService.ShowSuccessAsync($" Fotoğraf başarıyla yüklendi ve kaydedildi! ({optimizedImage.Length / 1024}KB)");
                        
                        // Eğer kullanıcı kendi resmini güncelliyorsa, TopBar'ı güncellemek için sayfayı yenile
                        var currentUserTcKimlikNo = _userInfoService.GetTcKimlikNo();
                        if (FormModel.TcKimlikNo == currentUserTcKimlikNo)
                        {
                            await Task.Delay(500); // Toast mesajının görünmesi için kısa bekleme
                            _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
                        }
                    }
                    else
                    {
                        await _toastService.ShowWarningAsync(" Fotoğraf yüklendi ancak veritabanı güncellenemedi. Lütfen formu kaydedin.");
                    }
                }
                else
                {
                    await _toastService.ShowSuccessAsync($" Fotoğraf başarıyla yüklendi! ({optimizedImage.Length / 1024}KB) - Formu kaydetmeyi unutmayın.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel resmi yüklenirken genel bir hata oluştu: {TcKimlikNo}", FormModel.TcKimlikNo);
                UploadErrorMessage = $" Fotoğraf yüklenirken bir hata oluştu: {ex.Message}";
                await _toastService.ShowErrorAsync(UploadErrorMessage);
            }
            finally
            {
                IsUploadingImage = false;
                StateHasChanged();
            }
        }

        // ═══════════════════════════════════════════════════════
        // ERROR MESSAGE PARSING
        // ═══════════════════════════════════════════════════════

        private string ParseApiErrorMessage(dynamic? errorMessages)
        {
            if (errorMessages == null)
                return "İşlem başarısız oldu. Lütfen tekrar deneyiniz.";

            var errorList = errorMessages as IEnumerable<string>;
            if (errorList == null || !errorList.Any())
                return "İşlem başarısız oldu. Lütfen tekrar deneyiniz.";

            var firstError = errorList.First();

            // Duplicate TC Kimlik No
            if (firstError.Contains("TC Kimlik") || firstError.Contains("TcKimlikNo") || 
                firstError.Contains("duplicate key") && firstError.Contains("28228204"))
            {
                return "❌ Bu TC Kimlik Numarası zaten kayıtlı!\n\nBu personel sistemde mevcut. Düzenlemek için personel listesinden seçiniz.";
            }

            // Duplicate Email
            if (firstError.Contains("Email") || firstError.Contains("email") || 
                firstError.Contains("IX_PER_Personeller_Email"))
            {
                return "❌ Bu email adresi başka bir personel tarafından kullanılıyor!\n\nLütfen farklı bir email adresi giriniz.";
            }

            // Foreign Key Violation
            if (firstError.Contains("FOREIGN KEY") || firstError.Contains("FK_"))
            {
                if (firstError.Contains("AtanmaNedenleri"))
                    return "❌ Geçersiz Atanma Nedeni!\n\nLütfen listeden geçerli bir atanma nedeni seçiniz veya boş bırakınız.";
                
                if (firstError.Contains("Departman"))
                    return "❌ Geçersiz Departman!\n\nLütfen listeden geçerli bir departman seçiniz.";
                
                if (firstError.Contains("Servis"))
                    return "❌ Geçersiz Servis!\n\nLütfen listeden geçerli bir servis seçiniz.";
                
                if (firstError.Contains("Unvan"))
                    return "❌ Geçersiz Ünvan!\n\nLütfen listeden geçerli bir ünvan seçiniz.";

                return "❌ Geçersiz veri girişi!\n\nLütfen tüm zorunlu alanları doğru şekilde doldurunuz.";
            }

            // Unique Constraint Violation
            if (firstError.Contains("unique") || firstError.Contains("UNIQUE"))
            {
                return "❌ Bu kayıt zaten mevcut!\n\nGirdiğiniz bilgiler başka bir kayıtta kullanılıyor.";
            }

            // Genel hata
            return $"❌ İşlem başarısız!\n\n{firstError}";
        }

        private string ParseHttpErrorMessage(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
                return "Sunucu ile bağlantı kurulamadı. Lütfen tekrar deneyiniz.";

            if (errorMessage.Contains("duplicate key") || errorMessage.Contains("PRIMARY KEY"))
            {
                if (errorMessage.Contains("TcKimlikNo") || errorMessage.Contains("28228204"))
                    return "❌ Bu TC Kimlik Numarası zaten kayıtlı!";
                
                if (errorMessage.Contains("Email"))
                    return "❌ Bu email adresi başka bir personel tarafından kullanılıyor!";
            }

            if (errorMessage.Contains("FOREIGN KEY"))
                return "❌ Geçersiz veri girişi! Lütfen tüm zorunlu alanları doğru şekilde doldurunuz.";

            if (errorMessage.Contains("timeout") || errorMessage.Contains("Timeout"))
                return "⏱️ İşlem zaman aşımına uğradı. Lütfen tekrar deneyiniz.";

            if (errorMessage.Contains("404") || errorMessage.Contains("Not Found"))
                return "❌ İstenen kaynak bulunamadı.";

            if (errorMessage.Contains("500") || errorMessage.Contains("Internal Server"))
                return "❌ Sunucu hatası oluştu. Lütfen sistem yöneticisine başvurunuz.";

            return $"❌ Hata: {errorMessage}";
        }

        private string ParseGeneralErrorMessage(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
                return "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyiniz.";

            if (errorMessage.Contains("duplicate") || errorMessage.Contains("Duplicate"))
                return "❌ Bu kayıt zaten mevcut! Lütfen farklı bilgiler giriniz.";

            if (errorMessage.Contains("constraint") || errorMessage.Contains("Constraint"))
                return "❌ Veri bütünlüğü hatası! Lütfen girdiğiniz bilgileri kontrol ediniz.";

            if (errorMessage.Contains("null") || errorMessage.Contains("Null"))
                return "❌ Zorunlu alanlar eksik! Lütfen tüm gerekli alanları doldurunuz.";

            return $"❌ Hata: {errorMessage}";
        }

        private async Task RemovePhoto()
        {
            try
            {
                if (!CanEditPage)
                {
                    await _toastService.ShowErrorAsync("Bu işlem için yetkiniz bulunmuyor.");
                    return;
                }

                if (string.IsNullOrEmpty(FormModel.Resim))
                {
                    await _toastService.ShowWarningAsync(" Silinecek fotoğraf bulunmuyor.");
                    return;
                }

                // Dosyayı sil
                var deleted = _imageHelper.DeleteImage(FormModel.Resim);

                // FormModel'den temizle
                FormModel.Resim = string.Empty;

                //  HEMEN VERİTABANINDAN SİL
                if (IsEditMode)
                {
                    var updateDto = _mapper.Map<PersonelUpdateRequestDto>(FormModel);
                    var response = await _personelApiService.UpdateAsync(FormModel.TcKimlikNo, updateDto);

                    if (response?.Success == true)
                    {
                        _logger.LogInformation(" Resim veritabanından silindi: {TcKimlikNo}", FormModel.TcKimlikNo);
                        await _toastService.ShowSuccessAsync(" Fotoğraf başarıyla kaldırıldı ve veritabanından silindi.");
                    }
                    else
                    {
                        _logger.LogWarning(" Resim dosyadan silindi ama DB güncellenemedi: {Message}", response?.Message);
                        await _toastService.ShowWarningAsync(" Fotoğraf kaldırıldı ancak veritabanı güncellenemedi. Lütfen formu kaydedin.");
                    }
                }
                else
                {
                    if (deleted)
                    {
                        await _toastService.ShowSuccessAsync(" Fotoğraf kaldırıldı. Formu kaydetmeyi unutmayın.");
                    }
                    else
                    {
                        await _toastService.ShowWarningAsync(" Fotoğraf dosyası bulunamadı ama temizlendi.");
                    }
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel resmi silinirken hata oluştu: {TcKimlikNo}", FormModel.TcKimlikNo);
                await _toastService.ShowErrorAsync($" Fotoğraf kaldırılırken hata oluştu: {ex.Message}");
            }
        }
    }
}
