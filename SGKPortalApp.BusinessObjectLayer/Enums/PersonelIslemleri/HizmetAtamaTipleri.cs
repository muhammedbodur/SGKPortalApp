using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum HizmetAtamaTipleri
    {
        [Display(Name = "İlk Defa Atama")]
        IlkDefaAtama,

        [Display(Name = "Yeniden Atama")]
        YenidenAtama,

        [Display(Name = "Naklen Atama")]
        NaklenAtama,

        [Display(Name = "Mahkeme Kararı ile Atama")]
        MahkemeKarariIleAtama,

        [Display(Name = "Geçici Görevlendirme")]
        GeciciGorevlendirme,

        [Display(Name = "Görevde Yükselme")]
        GorevdeYukselme,

        [Display(Name = "Unvan Değişikliği")]
        UnvanDegisikligi
    }
}
