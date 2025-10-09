using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SGKPortalApp.PresentationLayer.Components.Base
{
    /// <summary>
    /// Tüm sayfa component'leri için base class
    /// Select2, Flatpickr gibi JS kütüphanelerini otomatik initialize eder
    /// </summary>
    public abstract class BasePageComponent : ComponentBase
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

        /// <summary>
        /// Component'i yeniden render et
        /// </summary>
        protected new void StateHasChanged()
        {
            base.StateHasChanged();
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
    }
}
