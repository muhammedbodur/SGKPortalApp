using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SGKPortalApp.PresentationLayer.Pages.Test
{
    public partial class DomainInfoTest
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private string? serverInfo;
        private string? emailResult;
        private string? clientInfo;
        private string testEmail = "ahmet.yilmaz@sgk.gov.tr";

        private async Task GetServerInfo()
        {
            try
            {
                var response = await Http.GetFromJsonAsync<object>("/api/TestDomainInfo/server-info");
                serverInfo = System.Text.Json.JsonSerializer.Serialize(response,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                serverInfo = $"Hata: {ex.Message}";
            }
        }

        private async Task TestEmailToDomain()
        {
            try
            {
                var response = await Http.GetFromJsonAsync<object>($"/api/TestDomainInfo/ad-query?email={testEmail}");
                emailResult = System.Text.Json.JsonSerializer.Serialize(response,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                emailResult = $"Hata: {ex.Message}";
            }
        }

        private async Task GetClientInfo()
        {
            try
            {
                var jsInfo = await JS.InvokeAsync<ClientJsInfo>("getClientInfo");

                var clientRequest = new
                {
                    UserAgent = jsInfo.UserAgent,
                    Platform = jsInfo.Platform,
                    Language = jsInfo.Language,
                    ScreenResolution = jsInfo.ScreenResolution,
                    TimeZone = jsInfo.TimeZone
                };

                var response = await Http.PostAsJsonAsync("/api/TestDomainInfo/client-info", clientRequest);
                var result = await response.Content.ReadAsStringAsync();

                clientInfo = System.Text.Json.JsonSerializer.Serialize(
                    System.Text.Json.JsonSerializer.Deserialize<object>(result),
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                clientInfo = $"Hata: {ex.Message}";
            }
        }

        public class ClientJsInfo
        {
            public string UserAgent { get; set; } = "";
            public string Platform { get; set; } = "";
            public string Language { get; set; } = "";
            public string ScreenResolution { get; set; } = "";
            public string TimeZone { get; set; } = "";
        }
    }
}
