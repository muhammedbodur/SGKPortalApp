using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class IzinSorumluAtama
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private ILogger<IzinSorumluAtama> Logger { get; set; } = default!;

        private List<IzinSorumluResponseDto> sorumlular = new();
        private List<DepartmanDto> departmanList = new();
        private List<ServisDto> servisList = new();
        private List<PersonelDto> personelList = new();

        private int? filterDepartmanId = null;
        private int? filterServisId = null;
        private string? filterAktif = "";

        private bool isLoading = false;
        private bool showModal = false;
        private bool isEditMode = false;
        private bool isSaving = false;
        private string? errorMessage = null;

        private EditModel editModel = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFilterData();
            await LoadSorumlular();
        }

        private async Task LoadFilterData()
        {
            try
            {
                // Departman listesi
                var depResponse = await HttpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanDto>>>("/api/departman/liste");
                if (depResponse?.Success == true && depResponse.Data != null)
                {
                    departmanList = depResponse.Data;
                }

                // Servis listesi
                var srvResponse = await HttpClient.GetFromJsonAsync<ApiResponseDto<List<ServisDto>>>("/api/servis/liste");
                if (srvResponse?.Success == true && srvResponse.Data != null)
                {
                    servisList = srvResponse.Data;
                }

                // Personel listesi (aktif personeller)
                var perRequest = new { SadeceAktifler = true };
                var perResponse = await HttpClient.PostAsJsonAsync("/api/personel-list/liste", perRequest);
                if (perResponse.IsSuccessStatusCode)
                {
                    var result = await perResponse.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelListResponseDto>>>();
                    if (result?.Success == true && result.Data != null)
                    {
                        personelList = result.Data.Select(p => new PersonelDto
                        {
                            TcKimlikNo = p.TcKimlikNo,
                            AdSoyad = p.AdSoyad,
                            SicilNo = int.TryParse(p.SicilNo?.ToString(), out var sicil) ? sicil : 0
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Filtre verileri yüklenirken hata");
            }
        }

        private async Task LoadSorumlular()
        {
            try
            {
                isLoading = true;
                sorumlular.Clear();

                var url = "/api/izin-sorumlu";
                if (!string.IsNullOrEmpty(filterAktif))
                {
                    url = filterAktif == "true" ? "/api/izin-sorumlu/aktif" : "/api/izin-sorumlu";
                }

                var response = await HttpClient.GetFromJsonAsync<ApiResponseDto<List<IzinSorumluResponseDto>>>(url);
                if (response?.Success == true && response.Data != null)
                {
                    sorumlular = response.Data;

                    // Filtrele
                    if (filterDepartmanId.HasValue)
                    {
                        sorumlular = sorumlular.Where(s => s.DepartmanId == filterDepartmanId.Value || !s.DepartmanId.HasValue).ToList();
                    }

                    if (filterServisId.HasValue)
                    {
                        sorumlular = sorumlular.Where(s => s.ServisId == filterServisId.Value || !s.ServisId.HasValue).ToList();
                    }

                    if (!string.IsNullOrEmpty(filterAktif))
                    {
                        var aktif = filterAktif == "true";
                        sorumlular = sorumlular.Where(s => s.Aktif == aktif).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Sorumlu listesi yüklenirken hata");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void OpenCreateModal()
        {
            isEditMode = false;
            editModel = new EditModel { OnaySeviyes = 1, Aktif = true };
            errorMessage = null;
            showModal = true;
        }

        private void OpenEditModal(IzinSorumluResponseDto item)
        {
            isEditMode = true;
            editModel = new EditModel
            {
                IzinSorumluId = item.IzinSorumluId,
                DepartmanId = item.DepartmanId,
                ServisId = item.ServisId,
                SorumluPersonelTcKimlikNo = item.SorumluPersonelTcKimlikNo,
                OnaySeviyes = item.OnaySeviyes,
                Aktif = item.Aktif,
                Aciklama = item.Aciklama
            };
            errorMessage = null;
            showModal = true;
        }

        private void CloseModal()
        {
            showModal = false;
            editModel = new();
            errorMessage = null;
        }

        private async Task SaveSorumlu()
        {
            try
            {
                isSaving = true;
                errorMessage = null;

                // Validasyon
                if (string.IsNullOrEmpty(editModel.SorumluPersonelTcKimlikNo))
                {
                    errorMessage = "Sorumlu personel seçilmelidir";
                    return;
                }

                if (isEditMode)
                {
                    // Güncelleme
                    var updateDto = new IzinSorumluUpdateDto
                    {
                        IzinSorumluId = editModel.IzinSorumluId,
                        DepartmanId = editModel.DepartmanId,
                        ServisId = editModel.ServisId,
                        SorumluPersonelTcKimlikNo = editModel.SorumluPersonelTcKimlikNo,
                        OnaySeviyes = editModel.OnaySeviyes,
                        Aktif = editModel.Aktif,
                        Aciklama = editModel.Aciklama
                    };

                    var response = await HttpClient.PutAsJsonAsync($"/api/izin-sorumlu/{editModel.IzinSorumluId}", updateDto);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinSorumluResponseDto>>();
                        if (result?.Success == true)
                        {
                            CloseModal();
                            await LoadSorumlular();
                        }
                        else
                        {
                            errorMessage = result?.Message ?? "Güncelleme başarısız";
                        }
                    }
                    else
                    {
                        errorMessage = "Güncelleme sırasında hata oluştu";
                    }
                }
                else
                {
                    // Yeni oluşturma
                    var createDto = new IzinSorumluCreateDto
                    {
                        DepartmanId = editModel.DepartmanId,
                        ServisId = editModel.ServisId,
                        SorumluPersonelTcKimlikNo = editModel.SorumluPersonelTcKimlikNo,
                        OnaySeviyes = editModel.OnaySeviyes,
                        Aciklama = editModel.Aciklama
                    };

                    var response = await HttpClient.PostAsJsonAsync("/api/izin-sorumlu", createDto);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinSorumluResponseDto>>();
                        if (result?.Success == true)
                        {
                            CloseModal();
                            await LoadSorumlular();
                        }
                        else
                        {
                            errorMessage = result?.Message ?? "Kayıt başarısız";
                        }
                    }
                    else
                    {
                        errorMessage = "Kayıt sırasında hata oluştu";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Sorumlu kaydetme sırasında hata");
                errorMessage = "İşlem sırasında hata oluştu";
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task DeactivateSorumlu(int id)
        {
            if (!confirm("Bu sorumlu atamasını pasif yapmak istediğinizden emin misiniz?"))
                return;

            try
            {
                var response = await HttpClient.PatchAsync($"/api/izin-sorumlu/{id}/pasif", null);
                if (response.IsSuccessStatusCode)
                {
                    await LoadSorumlular();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Sorumlu pasif yapma sırasında hata: {Id}", id);
            }
        }

        private async Task DeleteSorumlu(int id)
        {
            if (!confirm("Bu sorumlu atamasını silmek istediğinizden emin misiniz?"))
                return;

            try
            {
                var response = await HttpClient.DeleteAsync($"/api/izin-sorumlu/{id}");
                if (response.IsSuccessStatusCode)
                {
                    await LoadSorumlular();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Sorumlu silme sırasında hata: {Id}", id);
            }
        }

        private bool confirm(string message)
        {
            // TODO: Replace with proper modal confirmation
            return true;
        }

        // Helper classes
        private class EditModel
        {
            public int IzinSorumluId { get; set; }
            public int? DepartmanId { get; set; }
            public int? ServisId { get; set; }
            public string SorumluPersonelTcKimlikNo { get; set; } = string.Empty;
            public int OnaySeviyes { get; set; } = 1;
            public bool Aktif { get; set; } = true;
            public string? Aciklama { get; set; }
        }

        private class PersonelDto
        {
            public string TcKimlikNo { get; set; } = string.Empty;
            public string AdSoyad { get; set; } = string.Empty;
            public int SicilNo { get; set; }
        }
    }
}
