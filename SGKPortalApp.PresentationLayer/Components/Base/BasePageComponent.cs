using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SGKPortalApp.PresentationLayer.Components.Base
{
    /// <summary>
    /// Tüm sayfa component'leri için base class
    /// Select2, Flatpickr gibi JS kütüphanelerini otomatik initialize eder
    /// ⭐ Authentication kontrolü yapar
    /// </summary>
    public abstract class BasePageComponent : ComponentBase
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        /// <summary>
        /// Component'i yeniden render et (async context için)
        /// </summary>
        protected void RefreshUI()
        {
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// ⭐ Sayfa yüklenmeden önce authentication kontrolü yap
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            
            // Authentication kontrolü
            await CheckAuthenticationAsync();
        }

        /// <summary>
        /// ⭐ Authentication durumunu kontrol et
        /// </summary>
        protected virtual async Task CheckAuthenticationAsync()
        {
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user?.Identity?.IsAuthenticated != true)
                {
                    // Kullanıcı logout olmuş, login sayfasına yönlendir
                    NavigationManager.NavigateTo("/auth/login", forceLoad: true);
                }
            }
            catch (Exception)
            {
                // Hata durumunda login sayfasına yönlendir
                NavigationManager.NavigateTo("/auth/login", forceLoad: true);
            }
        }

        /// <summary>
        /// Sayfa render edildikten sonra Select2'leri initialize et
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InitializeJsComponents();
            }
        }

        /// <summary>
        /// JS component'lerini initialize et (Select2, Flatpickr, vb.)
        /// </summary>
        protected virtual async Task InitializeJsComponents()
        {
            try
            {
                // 100ms bekle (DOM'un hazır olması için)
                await Task.Delay(100);

                // Select2'leri initialize et
                await JSRuntime.InvokeVoidAsync("Select2Blazor.initializeAll");
            }
            catch (Exception)
            {
                // JS henüz yüklenmemiş olabilir, sessizce devam et
            }
        }

        /// <summary>
        /// Select2'leri manuel olarak yeniden initialize et
        /// Veri yüklendikten sonra çağrılmalı
        /// </summary>
        protected async Task RefreshSelect2()
        {
            try
            {
                await Task.Delay(50);
                await JSRuntime.InvokeVoidAsync("Select2Blazor.initializeAll");
            }
            catch (Exception)
            {
                // Sessizce devam et
            }
        }

        /// <summary>
        /// Belirli bir Select2'yi initialize et
        /// </summary>
        protected async Task InitializeSelect2(string elementId, object? value = null)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("Select2Blazor.initialize", elementId, value);
            }
            catch (Exception)
            {
                // Sessizce devam et
            }
        }

        /// <summary>
        /// Select2 değerini güncelle
        /// </summary>
        protected async Task UpdateSelect2Value(string elementId, object value)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("Select2Blazor.updateValue", elementId, value);
            }
            catch (Exception)
            {
                // Sessizce devam et
            }
        }

        /// <summary>
        /// Enum değerinin Display attribute'undaki Name değerini döndürür.
        /// Display attribute yoksa enum'un ToString() değerini döndürür.
        /// </summary>
        /// <param name="value">Enum değeri</param>
        /// <returns>Display name veya enum string değeri</returns>
        protected string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        /// <summary>
        /// Ad Soyad'dan nickname oluşturur.
        /// Örnek: "Muhammed Ali Bodur" → "M.A.BODUR"
        /// </summary>
        /// <param name="adSoyad">Ad Soyad</param>
        /// <returns>Nickname (M.A.BODUR formatında)</returns>
        protected string GenerateNickName(string adSoyad)
        {
            if (string.IsNullOrWhiteSpace(adSoyad))
                return string.Empty;

            var parts = adSoyad.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return string.Empty;

            if (parts.Length == 1)
                return parts[0].ToUpper();

            // Son kelime soyad, diğerleri ad
            var soyad = parts[^1].ToUpper();
            var adIlkHarfler = string.Join(".", parts.Take(parts.Length - 1).Select(p => p[0].ToString().ToUpper()));

            return $"{adIlkHarfler}.{soyad}";
        }
    }
}
