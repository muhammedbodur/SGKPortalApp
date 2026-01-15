using System.ComponentModel;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri
{
    /// <summary>
    /// İzin ve Mazeret türleri
    /// Personel devamsızlık nedenleri için kullanılır
    /// </summary>
    public enum IzinMazeretTuru
    {
        /// <summary>
        /// Yıllık izin (hafıza kodlama: 1)
        /// Planlı izin, onay gerektirir
        /// </summary>
        [Description("Yıllık İzin")]
        YillikIzin = 1,

        /// <summary>
        /// Ücretsiz izin (hafıza kodlama: 2)
        /// Maaş kesilir, onay gerektirir
        /// </summary>
        [Description("Ücretsiz İzin")]
        UcretsizIzin = 2,

        /// <summary>
        /// Hastalık izni (hafıza kodlama: 3)
        /// Rapor ile belgelenir
        /// </summary>
        [Description("Hastalık İzni")]
        HastalikIzni = 3,

        /// <summary>
        /// Babalık izni (hafıza kodlama: 4)
        /// 5 gün ücretli izin (4857 sayılı kanun)
        /// </summary>
        [Description("Babalık İzni")]
        BabalikIzni = 4,

        /// <summary>
        /// Evlilik izni (hafıza kodlama: 5)
        /// 3 gün ücretli izin
        /// </summary>
        [Description("Evlilik İzni")]
        EvlilikIzni = 5,

        /// <summary>
        /// Ölüm izni (hafıza kodlama: 6)
        /// 3 gün ücretli izin (yakın akraba ölümü)
        /// </summary>
        [Description("Ölüm İzni")]
        OlumIzni = 6,

        /// <summary>
        /// Doğum izni (hafıza kodlama: 7)
        /// Anne için doğum öncesi/sonrası izin
        /// </summary>
        [Description("Doğum İzni")]
        DogumIzni = 7,

        /// <summary>
        /// Mazeret (hafıza kodlama: 8)
        /// Genel mazeret durumları (geç kalma, erken çıkış vb.)
        /// Genelde geriye dönük girilir
        /// </summary>
        [Description("Mazeret")]
        Mazeret = 8,

        /// <summary>
        /// Saatlik izin (hafıza kodlama: 9)
        /// Kısa süreli izinler (2-4 saat)
        /// </summary>
        [Description("Saatlik İzin")]
        SaatlikIzin = 9,

        /// <summary>
        /// Resmi tatil (hafıza kodlama: 10)
        /// Ulusal bayramlar, dini günler
        /// </summary>
        [Description("Resmi Tatil")]
        ResmiTatil = 10
    }
}
