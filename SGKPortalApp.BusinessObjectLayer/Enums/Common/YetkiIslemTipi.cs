using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    /// <summary>
    /// ModulControllerIslem kayıt tipi - Yetki tanımının hangi seviyede olduğunu belirtir
    /// </summary>
    public enum YetkiIslemTipi
    {
        /// <summary>
        /// Grup - Mantıksal gruplama (örn: Personel, Departman)
        /// Yetki Seçenekleri: None, View
        /// </summary>
        [Display(Name = "Grup")]
        Grup = 0,

        /// <summary>
        /// Sayfa seviyesi yetki (örn: Index, Detail, Manage)
        /// Yetki Seçenekleri: None, View
        /// </summary>
        [Display(Name = "Sayfa")]
        Page = 1,

        /// <summary>
        /// Sekme/Tab seviyesi yetki (örn: Eş Bilgileri, Çocuk Bilgileri)
        /// Yetki Seçenekleri: None, View
        /// </summary>
        [Display(Name = "Tab")]
        Tab = 2,

        /// <summary>
        /// Buton/Aksiyon seviyesi yetki (örn: Sil butonu, Düzenle butonu)
        /// Yetki Seçenekleri: None, View
        /// </summary>
        [Display(Name = "Buton")]
        Buton = 3,

        /// <summary>
        /// Field - Detail sayfasındaki readonly alan (örn: TC Kimlik görüntüleme)
        /// Yetki Seçenekleri: None, View
        /// </summary>
        [Display(Name = "Field")]
        Field = 4,

        /// <summary>
        /// FormField - Manage sayfasındaki düzenlenebilir form alanı
        /// Input, Select/Dropdown, Checkbox, Radio, DatePicker vb. tüm form elemanlarını kapsar
        /// Yetki Seçenekleri: View (disabled/readonly), Edit (enabled/düzenlenebilir) - None yok!
        /// </summary>
        [Display(Name = "Form Alanı")]
        FormField = 5
    }
}
