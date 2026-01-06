using System.ComponentModel;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri
{
    /// <summary>
    /// Kart tipi enum
    /// Personel kartları ve özel kartlar için kullanılır
    /// </summary>
    public enum CardType
    {
        [Description("Personel Kartı")]
        PersonelKarti = 0,

        [Description("Vizite Kartı")]
        ViziteKarti = 1,

        [Description("Saatlik İzin Kartı")]
        SaatlikIzinKarti = 2,

        [Description("Görev Kartı")]
        GorevKarti = 3,

        [Description("Geçici Personel Kartı")]
        GeciciPersonelKarti = 4,

        [Description("Misafir Kartı")]
        MisafirKarti = 5
    }
}
