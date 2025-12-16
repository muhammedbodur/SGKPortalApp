using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    /// <summary>
    /// Yetki seviyesi - Kişinin bir yetki tanımı için sahip olduğu erişim düzeyi
    /// </summary>
    public enum YetkiSeviyesi : int
    {
        /// <summary>
        /// Erişim yok - Eleman görünmez/erişilemez
        /// </summary>
        [Display(Name = "Erişim Yok")]
        None = 0,

        /// <summary>
        /// Görüntüleme - Eleman görünür ama düzenlenemez (readonly)
        /// </summary>
        [Display(Name = "Görüntüle")]
        View = 1,

        /// <summary>
        /// Düzenleme - Eleman görünür ve düzenlenebilir (sadece Input tipi için)
        /// </summary>
        [Display(Name = "Düzenle")]
        Edit = 2
    }
}