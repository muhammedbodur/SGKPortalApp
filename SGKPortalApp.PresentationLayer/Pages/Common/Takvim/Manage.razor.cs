using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Common.Takvim
{
    public partial class Manage
    {
        [SupplyParameterFromQuery(Name = "id")] public int? Id { get; set; }

        [Inject] private IResmiTatilApiService _resmiTatilService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private ResmiTatilFormModel FormModel { get; set; } = new();
        private bool IsEditMode => Id.HasValue && Id.Value > 0;
        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private bool NotFound { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            if (IsEditMode)
            {
                await LoadTatil();
            }
            else
            {
                FormModel = new ResmiTatilFormModel
                {
                    Tarih = DateTime.Today,
                    TatilTipi = TatilTipi.SabitTatil,
                    YariGun = false
                };
                IsLoading = false;
            }
        }

        private async Task LoadTatil()
        {
            IsLoading = true;
            try
            {
                var result = await _resmiTatilService.GetByIdAsync(Id!.Value);

                if (result.Success && result.Data != null)
                {
                    FormModel = new ResmiTatilFormModel
                    {
                        TatilId = result.Data.TatilId,
                        TatilAdi = result.Data.TatilAdi,
                        Tarih = result.Data.Tarih,
                        TatilTipi = result.Data.TatilTipi,
                        YariGun = result.Data.YariGun,
                        Aciklama = result.Data.Aciklama,
                        OtomatikSenkronize = result.Data.OtomatikSenkronize
                    };
                }
                else
                {
                    NotFound = true;
                    await _toastService.ShowErrorAsync(result.Message ?? "Tatil bulunamadı!");
                }
            }
            catch (Exception ex)
            {
                NotFound = true;
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            IsSaving = true;
            try
            {
                if (IsEditMode)
                {
                    var updateRequest = new ResmiTatilUpdateRequestDto
                    {
                        TatilId = FormModel.TatilId,
                        TatilAdi = FormModel.TatilAdi,
                        Tarih = FormModel.Tarih,
                        TatilTipi = FormModel.TatilTipi,
                        YariGun = FormModel.YariGun,
                        Aciklama = FormModel.Aciklama
                    };

                    var result = await _resmiTatilService.UpdateAsync(updateRequest);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Tatil başarıyla güncellendi!");
                        NavigateToList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Tatil güncellenemedi!");
                    }
                }
                else
                {
                    var createRequest = new ResmiTatilCreateRequestDto
                    {
                        TatilAdi = FormModel.TatilAdi,
                        Tarih = FormModel.Tarih,
                        TatilTipi = FormModel.TatilTipi,
                        YariGun = FormModel.YariGun,
                        Aciklama = FormModel.Aciklama
                    };

                    var result = await _resmiTatilService.CreateAsync(createRequest);

                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("Tatil başarıyla eklendi!");
                        NavigateToList();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Tatil eklenemedi!");
                    }
                }
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

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        private void NavigateToList()
        {
            _navigationManager.NavigateTo("/common/resmitatil");
        }

        public class ResmiTatilFormModel
        {
            public int TatilId { get; set; }
            public string TatilAdi { get; set; } = string.Empty;
            public DateTime Tarih { get; set; }
            public TatilTipi TatilTipi { get; set; }
            public bool YariGun { get; set; }
            public string? Aciklama { get; set; }
            public bool OtomatikSenkronize { get; set; }
        }
    }
}
