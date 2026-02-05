using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Pages.Haberler
{
    public partial class HaberDetay : FieldPermissionPageBase
    {
        [Parameter] public int Id { get; set; }

        [Inject] private IHaberApiService HaberApi { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private bool IsLoading = true;
        private bool IsDownloading = false;
        private HaberResponseDto? Haber;

        protected override async Task OnInitializedAsync()
        {
            await LoadHaber();
        }

        private async Task LoadHaber()
        {
            IsLoading = true;
            try
            {
                var response = await HaberApi.GetHaberByIdAsync(Id);
                if (response.Success)
                    Haber = response.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Haber detay yükleme hatası: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DownloadWord()
        {
            if (Haber == null) return;

            IsDownloading = true;
            try
            {
                var fileBytes = await HaberApi.DownloadHaberWordAsync(Id);
                if (fileBytes != null && fileBytes.Length > 0)
                {
                    var fileName = $"Haber_{Id}_{SanitizeFileName(Haber.Baslik)}.docx";
                    await JS.InvokeVoidAsync("downloadFile", fileBytes, fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Word indir hatası: {ex.Message}");
            }
            finally
            {
                IsDownloading = false;
            }
        }

        private List<string> GetIcerikLines()
        {
            if (string.IsNullOrEmpty(Haber?.Icerik)) return new();
            return Haber.Icerik.Split('\n').ToList();
        }

        private static string SanitizeFileName(string name)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                name = name.Replace(c.ToString(), "");
            return name.Length > 40 ? name.Substring(0, 40) : name;
        }
    }
}
