using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    /// <summary>
    /// Tüm modüllerde kullanılacak generic action tipi
    /// Yetki-İşlem sayfasında "Buton" seçildiğinde bu enum değerleri listelenir
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// Kayıt detaylarını görüntüleme yetkisi
        /// </summary>
        [Display(Name = "Detay Görüntüle", Description = "Kayıt detaylarını görüntüleme yetkisi")]
        DETAIL = 1,

        /// <summary>
        /// Yeni kayıt ekleme yetkisi
        /// </summary>
        [Display(Name = "Yeni Ekle", Description = "Yeni kayıt ekleme yetkisi")]
        ADD = 2,

        /// <summary>
        /// Kayıt düzenleme yetkisi
        /// </summary>
        [Display(Name = "Düzenle", Description = "Kayıt düzenleme yetkisi")]
        EDIT = 3,

        /// <summary>
        /// Kayıt silme yetkisi
        /// </summary>
        [Display(Name = "Sil", Description = "Kayıt silme yetkisi")]
        DELETE = 4,

        /// <summary>
        /// Kayıt durumunu değiştirme yetkisi (Aktif/Pasif)
        /// </summary>
        [Display(Name = "Aktif/Pasif Yap", Description = "Kayıt durumunu değiştirme yetkisi")]
        TOGGLE = 5,

        /// <summary>
        /// Dışa aktarma yetkisi (Excel, PDF, vb.)
        /// </summary>
        [Display(Name = "Dışa Aktar", Description = "Excel, PDF vb. formatlarda dışa aktarma yetkisi")]
        EXPORT = 6,

        /// <summary>
        /// Toplu veri içe aktarma yetkisi
        /// </summary>
        [Display(Name = "İçe Aktar", Description = "Toplu veri içe aktarma yetkisi")]
        IMPORT = 7,

        /// <summary>
        /// Yazdırma yetkisi
        /// </summary>
        [Display(Name = "Yazdır", Description = "Yazdırma yetkisi")]
        PRINT = 8,

        /// <summary>
        /// İşlem onaylama yetkisi
        /// </summary>
        [Display(Name = "Onayla", Description = "İşlem onaylama yetkisi")]
        APPROVE = 9,

        /// <summary>
        /// İşlem reddetme yetkisi
        /// </summary>
        [Display(Name = "Reddet", Description = "İşlem reddetme yetkisi")]
        REJECT = 10,

        /// <summary>
        /// Kayıt arşivleme yetkisi
        /// </summary>
        [Display(Name = "Arşivle", Description = "Kayıt arşivleme yetkisi")]
        ARCHIVE = 11,

        /// <summary>
        /// Toplu güncelleme yapma yetkisi
        /// </summary>
        [Display(Name = "Toplu Güncelleme", Description = "Toplu güncelleme yapma yetkisi")]
        BULK_UPDATE = 12
    }
}
