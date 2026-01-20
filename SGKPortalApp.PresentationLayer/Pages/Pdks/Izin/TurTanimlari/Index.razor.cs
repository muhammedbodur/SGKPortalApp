using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SGKPortalApp.PresentationLayer.Services.StateServices;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin.TurTanimlari
{
    public partial class Index
    {
        [Inject] private IIzinMazeretTuruTanimApiService ApiService { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private PermissionStateService PermissionService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private List<IzinMazeretTuruResponseDto> allTurler = new();
        private List<IzinMazeretTuruResponseDto> filteredTurler = new();
        private bool isLoading = true;
        private bool showAddForm = false;
        private bool showEditForm = false;

        private IzinMazeretTuruResponseDto newTur = new();
        private IzinMazeretTuruResponseDto editTur = new();

        private string searchTerm = string.Empty;
        private bool? filterActive = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                isLoading = true;
                var result = await ApiService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    allTurler = result.Data;
                    ApplyFilters();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Veriler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ApplyFilters()
        {
            var query = allTurler.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t =>
                    t.TuruAdi.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (t.KisaKod != null && t.KisaKod.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (filterActive.HasValue)
            {
                query = query.Where(t => t.IsActive == filterActive.Value);
            }

            filteredTurler = query.OrderBy(t => t.Sira).ThenBy(t => t.TuruAdi).ToList();
        }

        private void OnSearchChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            ApplyFilters();
        }

        private void OnFilterActiveChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                filterActive = null;
            }
            else
            {
                filterActive = bool.Parse(e.Value.ToString()!);
            }
            ApplyFilters();
        }

        private void ClearFilters()
        {
            searchTerm = string.Empty;
            filterActive = null;
            ApplyFilters();
        }

        private void ToggleAddForm()
        {
            showAddForm = !showAddForm;
            if (showAddForm)
            {
                showEditForm = false;
                newTur = new IzinMazeretTuruResponseDto
                {
                    BirinciOnayciGerekli = true,
                    IkinciOnayciGerekli = true,
                    PlanliIzinMi = true,
                    IsActive = true,
                    Sira = allTurler.Any() ? allTurler.Max(t => t.Sira) + 1 : 1
                };
            }
        }

        private void OpenEditForm(IzinMazeretTuruResponseDto tur)
        {
            showEditForm = true;
            showAddForm = false;
            editTur = new IzinMazeretTuruResponseDto
            {
                IzinMazeretTuruId = tur.IzinMazeretTuruId,
                TuruAdi = tur.TuruAdi,
                KisaKod = tur.KisaKod,
                Aciklama = tur.Aciklama,
                BirinciOnayciGerekli = tur.BirinciOnayciGerekli,
                IkinciOnayciGerekli = tur.IkinciOnayciGerekli,
                PlanliIzinMi = tur.PlanliIzinMi,
                Sira = tur.Sira,
                IsActive = tur.IsActive,
                RenkKodu = tur.RenkKodu
            };
        }

        private void CloseEditForm()
        {
            showEditForm = false;
            editTur = new();
        }

        private async Task SaveNewAsync()
        {
            try
            {
                var result = await ApiService.CreateAsync(newTur);

                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync("İzin türü başarıyla oluşturuldu");
                    showAddForm = false;
                    await LoadDataAsync();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "İzin türü oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task SaveEditAsync()
        {
            try
            {
                var result = await ApiService.UpdateAsync(editTur.IzinMazeretTuruId, editTur);

                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync("İzin türü başarıyla güncellendi");
                    CloseEditForm();
                    await LoadDataAsync();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "İzin türü güncellenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task ToggleActiveAsync(int id)
        {
            try
            {
                var result = await ApiService.ToggleActiveAsync(id);

                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync(result.Message ?? "Durum değiştirildi");
                    await LoadDataAsync();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Durum değiştirilemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task DeleteAsync(int id, string turuAdi)
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", 
                $"'{turuAdi}' izin türünü silmek istediğinizden emin misiniz?");

            if (!confirmed) return;

            try
            {
                var result = await ApiService.DeleteAsync(id);

                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync("İzin türü başarıyla silindi");
                    await LoadDataAsync();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "İzin türü silinemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private int GetTotalCount() => allTurler.Count;
        private int GetActiveCount() => allTurler.Count(t => t.IsActive);
        private int GetPassiveCount() => allTurler.Count(t => !t.IsActive);
        private int GetFilteredCount() => filteredTurler.Count;
    }
}
