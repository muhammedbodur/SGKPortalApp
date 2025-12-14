using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.YetkiAtama
{
    public partial class Index : ComponentBase
    {
        [Inject] private IPersonelApiService _personelService { get; set; } = default!;
        [Inject] private IYetkiApiService _yetkiService { get; set; } = default!;
        [Inject] private IPersonelYetkiApiService _personelYetkiService { get; set; } = default!;
        [Inject] private IModulControllerIslemApiService _modulControllerIslemService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private bool IsLoading { get; set; } = false;
        private bool IsSaving { get; set; } = false;

        private string TcKimlikNo { get; set; } = string.Empty;
        private PersonelResponseDto? SelectedPersonel { get; set; }

        private List<DropdownItemDto> YetkiDropdown { get; set; } = new();
        private List<DropdownItemDto> ModulControllerIslemDropdown { get; set; } = new();

        private int NewYetkiId { get; set; } = 0;
        private int NewModulControllerIslemId { get; set; } = 0;
        private YetkiTipleri NewYetkiTipi { get; set; } = YetkiTipleri.View;

        private List<PersonelYetkiRowModel> Assignments { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdowns();
        }

        private async Task LoadDropdowns()
        {
            try
            {
                var yetkiResult = await _yetkiService.GetDropdownAsync();
                YetkiDropdown = yetkiResult.Success && yetkiResult.Data != null
                    ? yetkiResult.Data
                    : new List<DropdownItemDto>();
                if (!yetkiResult.Success)
                    await _toastService.ShowErrorAsync(yetkiResult.Message);

                var islemResult = await _modulControllerIslemService.GetDropdownAsync();
                ModulControllerIslemDropdown = islemResult.Success && islemResult.Data != null
                    ? islemResult.Data
                    : new List<DropdownItemDto>();
                if (!islemResult.Success)
                    await _toastService.ShowErrorAsync(islemResult.Message);
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Dropdown yüklenemedi: {ex.Message}");
            }
        }

        private async Task LoadPersonel()
        {
            if (string.IsNullOrWhiteSpace(TcKimlikNo))
            {
                await _toastService.ShowWarningAsync("TC Kimlik No zorunludur");
                return;
            }

            IsLoading = true;
            try
            {
                var result = await _personelService.GetByTcKimlikNoAsync(TcKimlikNo.Trim());
                if (result.Success && result.Data != null)
                {
                    SelectedPersonel = result.Data;
                    await LoadAssignments();
                }
                else
                {
                    SelectedPersonel = null;
                    Assignments = new();
                    await _toastService.ShowErrorAsync(result.Message ?? "Personel bulunamadı");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadAssignments()
        {
            if (SelectedPersonel == null)
                return;

            IsLoading = true;
            try
            {
                var result = await _personelYetkiService.GetByTcKimlikNoAsync(SelectedPersonel.TcKimlikNo);
                if (result.Success && result.Data != null)
                {
                    Assignments = result.Data
                        .Select(x => new PersonelYetkiRowModel(x))
                        .ToList();
                }
                else
                {
                    Assignments = new();
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

        private async Task CreateAssignment()
        {
            if (SelectedPersonel == null)
                return;

            if (NewYetkiId <= 0 || NewModulControllerIslemId <= 0)
            {
                await _toastService.ShowWarningAsync("Yetki ve İşlem seçiniz");
                return;
            }

            IsSaving = true;
            try
            {
                var dto = new PersonelYetkiCreateRequestDto
                {
                    TcKimlikNo = SelectedPersonel.TcKimlikNo,
                    YetkiId = NewYetkiId,
                    ModulControllerIslemId = NewModulControllerIslemId,
                    YetkiTipleri = NewYetkiTipi
                };

                var result = await _personelYetkiService.CreateAsync(dto);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "Yetki atandı");
                    await LoadAssignments();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Yetki atanamadı");
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

        private async Task UpdateAssignment(PersonelYetkiRowModel row)
        {
            row.IsRowSaving = true;
            try
            {
                var dto = new PersonelYetkiUpdateRequestDto
                {
                    YetkiId = row.YetkiId,
                    ModulControllerIslemId = row.ModulControllerIslemId,
                    YetkiTipleri = row.YetkiTipleri
                };

                var result = await _personelYetkiService.UpdateAsync(row.PersonelYetkiId, dto);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "Güncellendi");
                    await LoadAssignments();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Güncellenemedi");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                row.IsRowSaving = false;
            }
        }

        private async Task DeleteAssignment(int id)
        {
            try
            {
                var result = await _personelYetkiService.DeleteAsync(id);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync(result.Message ?? "Silindi");
                    await LoadAssignments();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Silinemedi");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private class PersonelYetkiRowModel
        {
            public PersonelYetkiRowModel(PersonelYetkiResponseDto dto)
            {
                PersonelYetkiId = dto.PersonelYetkiId;
                TcKimlikNo = dto.TcKimlikNo;
                YetkiId = dto.YetkiId;
                YetkiAdi = dto.YetkiAdi;
                ModulControllerIslemId = dto.ModulControllerIslemId;
                ModulControllerIslemAdi = dto.ModulControllerIslemAdi;
                YetkiTipleri = dto.YetkiTipleri;
            }

            public int PersonelYetkiId { get; set; }
            public string TcKimlikNo { get; set; } = string.Empty;

            public int YetkiId { get; set; }
            public string? YetkiAdi { get; set; }

            public int ModulControllerIslemId { get; set; }
            public string? ModulControllerIslemAdi { get; set; }

            public YetkiTipleri YetkiTipleri { get; set; }

            public bool IsRowSaving { get; set; } = false;
        }
    }
}
