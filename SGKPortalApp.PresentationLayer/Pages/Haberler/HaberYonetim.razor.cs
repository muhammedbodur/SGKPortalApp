using Microsoft.AspNetCore.Components;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Pages.Haberler
{
    public partial class HaberYonetim : FieldPermissionPageBase
    {
        [Inject] private IHaberApiService HaberApi { get; set; } = default!;

        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private List<HaberResponseDto> HaberListe { get; set; } = new();
        private List<HaberResponseDto> FilteredList { get; set; } = new();

        private string SearchTerm { get; set; } = "";
        private string ActiveSearchTerm { get; set; } = "";
        private string StatusFilter { get; set; } = "";

        private int CurrentPage { get; set; } = 1;
        private int TotalCount { get; set; } = 0;
        private int TotalPages { get; set; } = 0;
        private const int PageSize = 12;

        // Modal
        private bool ShowDeleteModal { get; set; } = false;
        private int DeleteModalId { get; set; }
        private string DeleteModalTitle { get; set; } = "";

        // Toast
        private string ToastMessage { get; set; } = "";
        private string ToastType { get; set; } = "success";

        protected override async Task OnInitializedAsync()
        {
            await LoadHaberler();
        }

        private async Task LoadHaberler()
        {
            IsLoading = true;
            try
            {
                var response = await HaberApi.GetAdminHaberListeAsync(CurrentPage, PageSize, ActiveSearchTerm);
                if (response.Success && response.Data != null)
                {
                    HaberListe = response.Data.Items;
                    TotalCount = response.Data.TotalCount;
                    TotalPages = response.Data.TotalPages;
                }
                else
                {
                    HaberListe = new();
                    TotalCount = 0;
                    TotalPages = 0;
                }

                ApplyStatusFilter();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Haber yönetim listesi yükleme hatası: {ex.Message}");
                HaberListe = new();
                FilteredList = new();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyStatusFilter()
        {
            var now = DateTime.Now;

            FilteredList = StatusFilter switch
            {
                "Aktif" => HaberListe.Where(h =>
                    h.YayinTarihi <= now &&
                    (h.BitisTarihi == null || h.BitisTarihi >= now)).ToList(),
                "Pasif" => HaberListe.Where(h =>
                    h.YayinTarihi <= now &&
                    (h.BitisTarihi == null || h.BitisTarihi >= now)).ToList(), // Aktiflik enum bilgisi response'ta yok, durumu tarihten hesaplıyoruz
                "Taslak" => HaberListe.Where(h => h.YayinTarihi > now).ToList(),
                "Bitti" => HaberListe.Where(h => h.BitisTarihi.HasValue && h.BitisTarihi < now).ToList(),
                _ => HaberListe
            };
        }

        private async Task Search()
        {
            ActiveSearchTerm = SearchTerm;
            CurrentPage = 1;
            await LoadHaberler();
        }

        private async Task ClearSearch()
        {
            SearchTerm = "";
            ActiveSearchTerm = "";
            StatusFilter = "";
            CurrentPage = 1;
            await LoadHaberler();
        }

        private async Task GoToPage(int page)
        {
            if (page < 1 || page > TotalPages) return;
            CurrentPage = page;
            await LoadHaberler();
        }

        private int GetStartPage() => Math.Max(1, CurrentPage - 2);
        private int GetEndPage() => Math.Min(TotalPages, CurrentPage + 2);

        // ─── Durum Badge ─────────────────────────────────────

        private string GetStatusText(HaberResponseDto haber)
        {
            var now = DateTime.Now;
            if (haber.YayinTarihi > now) return "Taslak";
            if (haber.BitisTarihi.HasValue && haber.BitisTarihi < now) return "Bitti";
            return "Aktif";
        }

        private string GetStatusBadgeClass(HaberResponseDto haber)
        {
            var now = DateTime.Now;
            if (haber.YayinTarihi > now) return "bg-secondary";
            if (haber.BitisTarihi.HasValue && haber.BitisTarihi < now) return "bg-danger";
            return "bg-success";
        }

        // ─── Delete Flow ─────────────────────────────────────

        private void ConfirmDelete(int haberId, string baslik)
        {
            DeleteModalId = haberId;
            DeleteModalTitle = baslik;
            ShowDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            ShowDeleteModal = false;
        }

        private async Task DeleteHaber()
        {
            IsSaving = true;
            try
            {
                var result = await HaberApi.DeleteHaberAsync(DeleteModalId);
                if (result.Success)
                {
                    ShowToast("Haber başarıyla silindi.", "success");
                    ShowDeleteModal = false;
                    await LoadHaberler();
                }
                else
                {
                    ShowToast("Silme işlemi başarısız oldu.", "danger");
                }
            }
            catch (Exception ex)
            {
                ShowToast($"Hata: {ex.Message}", "danger");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void ShowToast(string message, string type)
        {
            ToastMessage = message;
            ToastType = type;
            // Auto-hide after 3 sec
            _ = Task.Delay(3000).ContinueWith(_ =>
            {
                ToastMessage = "";
                InvokeAsync(StateHasChanged);
            });
        }
    }
}
