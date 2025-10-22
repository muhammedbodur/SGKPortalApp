using Microsoft.AspNetCore.Components;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.PersonelAtama
{
    public partial class Index
    {
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IPersonelApiService _personelService { get; set; } = default!;
        [Inject] private IKanalAltIslemApiService _kanalAltIslemService { get; set; } = default!;
        [Inject] private IKanalPersonelApiService _kanalPersonelService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        // State
        private bool isLoading = false;
        private HizmetBinasiResponseDto? selectedHizmetBinasi;
        
        // Computed Properties
        private int toplamAtamaSayisi => personelAtamalari.Count(x => x.Uzmanlik != PersonelUzmanlik.BilgisiYok);
        
        // Data Lists
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<PersonelResponseDto> personeller = new();
        private List<KanalAltIslemResponseDto> kanalAltIslemler = new();
        private List<KanalPersonelResponseDto> personelAtamalari = new();
        
        // Grouped Data
        private List<KanalIslemGroup> kanalIslemler = new();

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadHizmetBinalari();
        }

        private async Task LoadHizmetBinalari()
        {
            try
            {
                isLoading = true;
                var result = await _hizmetBinasiService.GetAllAsync();
                
                if (result.Success && result.Data != null)
                {
                    hizmetBinalari = result.Data;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Hizmet binaları yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binaları yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Hizmet binaları yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int hizmetBinasiId) && hizmetBinasiId > 0)
            {
                selectedHizmetBinasi = hizmetBinalari.FirstOrDefault(x => x.HizmetBinasiId == hizmetBinasiId);
                await LoadData(hizmetBinasiId);
            }
            else
            {
                selectedHizmetBinasi = null;
                ClearData();
            }
        }

        private async Task LoadData(int hizmetBinasiId)
        {
            try
            {
                isLoading = true;

                // Paralel olarak tüm verileri yükle
                var personelTask = LoadPersoneller(hizmetBinasiId);
                var kanalAltIslemTask = LoadKanalAltIslemler(hizmetBinasiId);
                var personelAtamaTask = LoadPersonelAtamalari(hizmetBinasiId);

                await Task.WhenAll(personelTask, kanalAltIslemTask, personelAtamaTask);

                // Kanal İşlem gruplarını oluştur
                GroupKanalIslemler();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veriler yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadPersoneller(int hizmetBinasiId)
        {
            var personelResult = await _personelService.GetAllAsync();
            
            if (personelResult.Success && personelResult.Data != null)
            {
                // Hizmet binasına göre filtrele
                personeller = personelResult.Data
                    .Where(p => p.HizmetBinasiId == hizmetBinasiId)
                    .ToList();
            }
            else
            {
                personeller = new();
                await _toastService.ShowWarningAsync("Personel listesi yüklenemedi");
            }
        }

        private async Task LoadKanalAltIslemler(int hizmetBinasiId)
        {
            var kanalAltIslemResult = await _kanalAltIslemService.GetByHizmetBinasiIdAsync(hizmetBinasiId);
            
            if (kanalAltIslemResult.Success && kanalAltIslemResult.Data != null)
            {
                kanalAltIslemler = kanalAltIslemResult.Data;
            }
            else
            {
                kanalAltIslemler = new();
                await _toastService.ShowWarningAsync("Kanal alt işlemler yüklenemedi");
            }
        }

        private async Task LoadPersonelAtamalari(int hizmetBinasiId)
        {
            var atamalarResult = await _kanalPersonelService.GetByHizmetBinasiIdAsync(hizmetBinasiId);
            
            if (atamalarResult.Success && atamalarResult.Data != null)
            {
                personelAtamalari = atamalarResult.Data;
            }
            else
            {
                personelAtamalari = new();
                // Hata mesajı gösterme, boş liste normal bir durum olabilir
            }
        }

        private void GroupKanalIslemler()
        {
            kanalIslemler = kanalAltIslemler
                .GroupBy(x => new { x.KanalIslemId, x.KanalAdi })
                .Select(g => new KanalIslemGroup
                {
                    KanalIslemId = g.Key.KanalIslemId,
                    KanalIslemAdi = g.Key.KanalAdi,
                    AltIslemler = g.OrderBy(x => x.Sira).ToList()
                })
                .OrderBy(x => x.KanalIslemAdi)
                .ToList();
        }

        private void ClearData()
        {
            personeller = new();
            kanalAltIslemler = new();
            personelAtamalari = new();
            kanalIslemler = new();
        }

        private KanalPersonelResponseDto? GetPersonelAtama(string tcKimlikNo, int kanalAltIslemId)
        {
            return personelAtamalari.FirstOrDefault(x => 
                x.TcKimlikNo == tcKimlikNo && 
                x.KanalAltIslemId == kanalAltIslemId);
        }

        private async Task ToggleUzmanlik(string tcKimlikNo, int kanalAltIslemId)
        {
            try
            {
                var atama = GetPersonelAtama(tcKimlikNo, kanalAltIslemId);
                PersonelUzmanlik yeniUzmanlik;

                if (atama == null)
                {
                    // Yeni atama oluştur - Uzman olarak başlat
                    yeniUzmanlik = PersonelUzmanlik.Uzman;
                    await CreateAtama(tcKimlikNo, kanalAltIslemId, yeniUzmanlik);
                }
                else
                {
                    // Mevcut atamayı döngüsel olarak değiştir
                    // BilgisiYok -> Uzman -> YrdUzman -> BilgisiYok
                    yeniUzmanlik = atama.Uzmanlik switch
                    {
                        PersonelUzmanlik.BilgisiYok => PersonelUzmanlik.Uzman,
                        PersonelUzmanlik.Uzman => PersonelUzmanlik.YrdUzman,
                        PersonelUzmanlik.YrdUzman => PersonelUzmanlik.BilgisiYok,
                        _ => PersonelUzmanlik.Uzman
                    };

                    if (yeniUzmanlik == PersonelUzmanlik.BilgisiYok)
                    {
                        // Atamayı sil
                        await DeleteAtama(atama.KanalPersonelId);
                    }
                    else
                    {
                        // Atamayı güncelle
                        await UpdateAtama(atama.KanalPersonelId, yeniUzmanlik);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uzmanlık seviyesi değiştirilirken hata oluştu");
                await _toastService.ShowErrorAsync("İşlem sırasında bir hata oluştu");
            }
        }

        private async Task CreateAtama(string tcKimlikNo, int kanalAltIslemId, PersonelUzmanlik uzmanlik)
        {
            var createDto = new KanalPersonelCreateRequestDto
            {
                TcKimlikNo = tcKimlikNo,
                KanalAltIslemId = kanalAltIslemId,
                Uzmanlik = uzmanlik,
                Aktiflik = BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
            };

            var result = await _kanalPersonelService.CreateAsync(createDto);

            if (result.Success && result.Data != null)
            {
                personelAtamalari.Add(result.Data);
                
                var personel = personeller.FirstOrDefault(p => p.TcKimlikNo == tcKimlikNo);
                var altIslem = kanalAltIslemler.FirstOrDefault(k => k.KanalAltIslemId == kanalAltIslemId);
                
                if (personel != null && altIslem != null)
                {
                    await _toastService.ShowSuccessAsync($"{personel.AdSoyad} - {altIslem.KanalAltAdi}: {GetUzmanlikText(uzmanlik)}");
                }
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Atama oluşturulamadı");
            }
        }

        private async Task UpdateAtama(int kanalPersonelId, PersonelUzmanlik yeniUzmanlik)
        {
            var updateDto = new KanalPersonelUpdateRequestDto
            {
                Uzmanlik = yeniUzmanlik,
                Aktiflik = BusinessObjectLayer.Enums.Common.Aktiflik.Aktif
            };

            var result = await _kanalPersonelService.UpdateAsync(kanalPersonelId, updateDto);

            if (result.Success)
            {
                var atama = personelAtamalari.FirstOrDefault(x => x.KanalPersonelId == kanalPersonelId);
                
                if (atama != null)
                {
                    atama.Uzmanlik = yeniUzmanlik;
                    atama.DuzenlenmeTarihi = DateTime.Now;
                    
                    await _toastService.ShowSuccessAsync($"{atama.PersonelAdSoyad} - {atama.KanalAltIslemAdi}: {GetUzmanlikText(yeniUzmanlik)}");
                }
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Atama güncellenemedi");
            }
        }

        private async Task DeleteAtama(int kanalPersonelId)
        {
            var result = await _kanalPersonelService.DeleteAsync(kanalPersonelId);

            if (result.Success)
            {
                var atama = personelAtamalari.FirstOrDefault(x => x.KanalPersonelId == kanalPersonelId);
                
                if (atama != null)
                {
                    personelAtamalari.Remove(atama);
                    await _toastService.ShowInfoAsync($"{atama.PersonelAdSoyad} - {atama.KanalAltIslemAdi}: Atama kaldırıldı");
                }
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Atama silinemedi");
            }
        }

        private string GetUzmanlikButtonClass(PersonelUzmanlik uzmanlik)
        {
            return uzmanlik switch
            {
                PersonelUzmanlik.Uzman => "btn-success",
                PersonelUzmanlik.YrdUzman => "btn-info",
                PersonelUzmanlik.BilgisiYok => "btn-outline-secondary",
                _ => "btn-outline-secondary"
            };
        }

        private string GetUzmanlikBadgeClass(PersonelUzmanlik uzmanlik)
        {
            return uzmanlik switch
            {
                PersonelUzmanlik.Uzman => "bg-label-success",
                PersonelUzmanlik.YrdUzman => "bg-label-info",
                PersonelUzmanlik.BilgisiYok => "bg-label-secondary",
                _ => "bg-label-secondary"
            };
        }

        private string GetUzmanlikIcon(PersonelUzmanlik uzmanlik)
        {
            return uzmanlik switch
            {
                PersonelUzmanlik.Uzman => "bx-star",
                PersonelUzmanlik.YrdUzman => "bx-user",
                PersonelUzmanlik.BilgisiYok => "bx-x",
                _ => "bx-question-mark"
            };
        }

        private string GetUzmanlikText(PersonelUzmanlik uzmanlik)
        {
            return uzmanlik switch
            {
                PersonelUzmanlik.Uzman => "Uzman",
                PersonelUzmanlik.YrdUzman => "Yrd. Uzman",
                PersonelUzmanlik.BilgisiYok => "Yok",
                _ => "?"
            };
        }

        private async Task ExportToExcel()
        {
            await _toastService.ShowInfoAsync("Personel atama export özelliği geliştirme aşamasındadır");
        }

        private async Task ExportToPdf()
        {
            await _toastService.ShowInfoAsync("Personel atama export özelliği geliştirme aşamasındadır");
        }

        // Helper class for grouping
        private class KanalIslemGroup
        {
            public int KanalIslemId { get; set; }
            public string KanalIslemAdi { get; set; } = string.Empty;
            public List<KanalAltIslemResponseDto> AltIslemler { get; set; } = new();
        }
    }
}
