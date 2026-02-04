using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Pages.Haberler
{
    public partial class Haberler
    {
        [Inject] private IHaberApiService HaberApi { get; set; } = default!;

        private bool IsLoading = true;
        private List<HaberResponseDto> HaberListe = new();
        private string SearchTerm = "";
        private string ActiveSearchTerm = "";

        private int CurrentPage = 1;
        private int TotalCount = 0;
        private int TotalPages = 0;
        private const int PageSize = 12;

        protected override async Task OnInitializedAsync()
        {
            await LoadHaberler();
        }

        private async Task LoadHaberler()
        {
            IsLoading = true;
            try
            {
                var response = await HaberApi.GetHaberListeAsync(CurrentPage, PageSize, ActiveSearchTerm);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Haberler yükleme hatası: {ex.Message}");
                HaberListe = new();
            }
            finally
            {
                IsLoading = false;
            }
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
            CurrentPage = 1;
            await LoadHaberler();
        }

        private async Task GoToPage(int page)
        {
            if (page < 1 || page > TotalPages) return;
            CurrentPage = page;
            await LoadHaberler();
        }

        // Sayfalama: gösterilecek sayfa numaraları aralığı
        private int GetStartPage() => Math.Max(1, CurrentPage - 2);
        private int GetEndPage() => Math.Min(TotalPages, CurrentPage + 2);
    }
}
