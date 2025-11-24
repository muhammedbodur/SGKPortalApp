using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Tv
{
    public partial class BankoEslestirme
    {
        [Inject]
        private ITvApiService _tvService { get; set; } = default!;

        [Inject]
        private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;

        [Inject]
        private IBankoApiService _bankoService { get; set; } = default!;

        [Inject]
        private IToastService _toastService { get; set; } = default!;

        [Inject]
        private NavigationManager _navigationManager { get; set; } = default!;

        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<TvResponseDto> tvler = new();
        private List<BankoResponseDto> allBankolar = new();
        private List<BankoResponseDto> assignedBankolar = new();
        private List<BankoResponseDto> unassignedBankolar = new();

        private int selectedHizmetBinasiId = 0;
        private int selectedTvId = 0;
        private string selectedTvName = string.Empty;

        private bool isLoading = true;
        private bool isLoadingBankolar = false;
        private bool isSaving = false;

        private int draggedBankoId = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;

            try
            {
                // Hizmet binalarını yükle
                var binaResult = await _hizmetBinasiService.GetAllAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data.OrderBy(b => b.HizmetBinasiAdi).ToList();
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veri yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            selectedHizmetBinasiId = int.Parse(e.Value?.ToString() ?? "0");
            selectedTvId = 0;
            selectedTvName = string.Empty;
            tvler.Clear();
            assignedBankolar.Clear();
            unassignedBankolar.Clear();

            if (selectedHizmetBinasiId > 0)
            {
                try
                {
                    var result = await _tvService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                    if (result.Success && result.Data != null)
                    {
                        tvler = result.Data.OrderBy(t => t.TvAdi).ToList();
                    }
                }
                catch (Exception ex)
                {
                    await _toastService.ShowErrorAsync($"TV'ler yüklenirken hata oluştu: {ex.Message}");
                }
            }
        }

        private async Task SelectTv(int tvId)
        {
            selectedTvId = tvId;
            var tv = tvler.FirstOrDefault(t => t.TvId == tvId);
            selectedTvName = tv?.TvAdi ?? string.Empty;

            await LoadBankolarAsync();
        }

        private async Task LoadBankolarAsync()
        {
            if (selectedHizmetBinasiId == 0) return;

            isLoadingBankolar = true;

            try
            {
                // Hizmet binasındaki tüm bankoları yükle
                var bankoResult = await _bankoService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                if (bankoResult.Success && bankoResult.Data != null)
                {
                    allBankolar = bankoResult.Data.OrderBy(b => b.BankoNo).ToList();

                    // TV detaylarını yükle (eşleştirilmiş bankoları almak için)
                    var tvResult = await _tvService.GetWithDetailsAsync(selectedTvId);
                    if (tvResult.Success && tvResult.Data != null)
                    {
                        var assignedBankoIds = tvResult.Data.EslesmiBankoIdler ?? new List<int>();

                        assignedBankolar = allBankolar
                            .Where(b => assignedBankoIds.Contains(b.BankoId))
                            .ToList();

                        unassignedBankolar = allBankolar
                            .Where(b => !assignedBankoIds.Contains(b.BankoId))
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Bankolar yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                isLoadingBankolar = false;
            }
        }

        // Drag & Drop Events
        private void OnDragStart(int bankoId)
        {
            draggedBankoId = bankoId;
        }

        private async Task OnDropToAssigned(int targetBankoId)
        {
            if (draggedBankoId == 0) return;

            await AddBanko(draggedBankoId);
            draggedBankoId = 0;
        }

        private async Task OnDropToUnassigned(int targetBankoId)
        {
            if (draggedBankoId == 0) return;

            await RemoveBanko(draggedBankoId);
            draggedBankoId = 0;
        }

        // Add/Remove Operations
        private async Task AddBanko(int bankoId)
        {
            isSaving = true;

            try
            {
                var result = await _tvService.AddBankoToTvAsync(selectedTvId, bankoId);
                
                if (result.Success)
                {
                    var banko = unassignedBankolar.FirstOrDefault(b => b.BankoId == bankoId);
                    if (banko != null)
                    {
                        unassignedBankolar.Remove(banko);
                        assignedBankolar.Add(banko);
                        assignedBankolar = assignedBankolar.OrderBy(b => b.BankoNo).ToList();

                        // TV listesini yenile (banko sayısını güncellemek için)
                        await ReloadTvListAsync();

                        await _toastService.ShowSuccessAsync($"Banko {banko.BankoNo} eklendi");
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Banko eklenemedi");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task RemoveBanko(int bankoId)
        {
            isSaving = true;

            try
            {
                var result = await _tvService.RemoveBankoFromTvAsync(selectedTvId, bankoId);
                
                if (result.Success)
                {
                    var banko = assignedBankolar.FirstOrDefault(b => b.BankoId == bankoId);
                    if (banko != null)
                    {
                        assignedBankolar.Remove(banko);
                        unassignedBankolar.Add(banko);
                        unassignedBankolar = unassignedBankolar.OrderBy(b => b.BankoNo).ToList();

                        // TV listesini yenile (banko sayısını güncellemek için)
                        await ReloadTvListAsync();

                        await _toastService.ShowSuccessAsync($"Banko {banko.BankoNo} kaldırıldı");
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Banko kaldırılamadı");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task ReloadTvListAsync()
        {
            if (selectedHizmetBinasiId > 0)
            {
                try
                {
                    var result = await _tvService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                    if (result.Success && result.Data != null)
                    {
                        tvler = result.Data.OrderBy(t => t.TvAdi).ToList();
                    }
                }
                catch (Exception ex)
                {
                    // Sessizce hata yut, kullanıcıya gösterme
                    Console.WriteLine($"TV listesi yenilenirken hata: {ex.Message}");
                }
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/tv");
        }
    }
}
