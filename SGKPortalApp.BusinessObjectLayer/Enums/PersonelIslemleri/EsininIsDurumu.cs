using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum EsininIsDurumu
    {
        [Display(Name = "Belirtilmemiş")]
        belirtilmemis,

        [Display(Name = "Çalışmıyor")]
        calismiyor,

        [Display(Name = "Ev Hanımı - Çalışmıyor")]
        evhanimi,

        [Display(Name = "Emekli - Çalışmıyor")]
        emekli,

        [Display(Name = "Çalışıyor - Özel")]
        ozel,

        [Display(Name = "Çalışıyor - Kamu")]
        kamu,
    }
}
