using System.ComponentModel;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    /// <summary>
    /// Kanal personelinin uzmanlık seviyesi
    /// </summary>
    public enum PersonelUzmanlik
    {
        [Description("Normal")]
        Normal = 0,

        [Description("Uzman")]
        Uzman = 1,

        [Description("Kıdemli Uzman")]
        KidemliUzman = 2
    }
}
