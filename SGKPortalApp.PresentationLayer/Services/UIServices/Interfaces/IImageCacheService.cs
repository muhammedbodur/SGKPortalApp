namespace SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces
{
    public interface IImageCacheService
    {
        /// <summary>
        /// Resim için cache-busting parametresi (ETag) döndürür
        /// Örnek: GetCacheBustParameter("/images/avatars/28165202398.jpg") → "A1B2C3D4"
        /// </summary>
        string GetCacheBustParameter(string imagePath);

        /// <summary>
        /// Resim silindiğinde veya güncellendiğinde cache'i invalidate et
        /// </summary>
        void InvalidateCache(string imagePath);
    }
}
