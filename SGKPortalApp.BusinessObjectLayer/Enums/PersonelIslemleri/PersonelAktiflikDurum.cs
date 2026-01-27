using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    /// <summary>
    /// Personel aktiflik durumu.
    /// Kurumun Personeli: Aktif, AyliksizIzin, GeciciGorevlendirme
    /// Kurumun Personeli Değil: Pasif, Emekli, Istifa, KurumlararasiNakil, Mustagfi
    /// </summary>
    public enum PersonelAktiflikDurum : int
    {
        // ==========================================
        // KURUMUN PERSONELİ DEĞİL (0-9 arası)
        // ==========================================

        [Display(Name = "Pasif")]
        Pasif = 0,

        [Display(Name = "Emekli")]
        Emekli = 2,

        [Display(Name = "İstifa")]
        Istifa = 3,

        [Display(Name = "Kurumlar Arası Nakil")]
        KurumlararasiNakil = 4,

        [Display(Name = "Müstağfi")]
        Mustagfi = 5,

        [Display(Name = "Vefat")]
        Vefat = 6,

        // ==========================================
        // KURUMUN PERSONELİ (10+ arası)
        // ==========================================

        [Display(Name = "Aktif")]
        Aktif = 1,

        [Display(Name = "Aylıksız İzin")]
        AyliksizIzin = 10,

        [Display(Name = "Geçici Görevlendirme")]
        GeciciGorevlendirme = 11,

        [Display(Name = "Askerlik")]
        Askerlik = 12,

        [Display(Name = "Ücretsiz İzin")]
        UcretsizIzin = 13
    }

    /// <summary>
    /// PersonelAktiflikDurum için extension metodları
    /// </summary>
    public static class PersonelAktiflikDurumExtensions
    {
        /// <summary>
        /// Personel hala kurumun personeli mi? (aktif çalışan, izinli, geçici görevli vb.)
        /// </summary>
        public static bool IsKurumPersoneli(this PersonelAktiflikDurum durum)
        {
            return durum == PersonelAktiflikDurum.Aktif
                || durum == PersonelAktiflikDurum.AyliksizIzin
                || durum == PersonelAktiflikDurum.GeciciGorevlendirme
                || durum == PersonelAktiflikDurum.Askerlik
                || durum == PersonelAktiflikDurum.UcretsizIzin;
        }

        /// <summary>
        /// Personel kurumdan ayrılmış mı? (emekli, istifa, nakil vb.)
        /// </summary>
        public static bool IsAyrilmis(this PersonelAktiflikDurum durum)
        {
            return !durum.IsKurumPersoneli();
        }

        /// <summary>
        /// Personel aktif çalışıyor mu? (izinli/görevli değil, fiilen çalışıyor)
        /// </summary>
        public static bool IsAktifCalisan(this PersonelAktiflikDurum durum)
        {
            return durum == PersonelAktiflikDurum.Aktif;
        }

        /// <summary>
        /// Badge CSS class'ı döndürür
        /// </summary>
        public static string GetBadgeClass(this PersonelAktiflikDurum durum)
        {
            return durum switch
            {
                PersonelAktiflikDurum.Aktif => "bg-success",
                PersonelAktiflikDurum.AyliksizIzin => "bg-warning",
                PersonelAktiflikDurum.GeciciGorevlendirme => "bg-info",
                PersonelAktiflikDurum.Askerlik => "bg-primary",
                PersonelAktiflikDurum.UcretsizIzin => "bg-warning",
                PersonelAktiflikDurum.Emekli => "bg-secondary",
                PersonelAktiflikDurum.Istifa => "bg-danger",
                PersonelAktiflikDurum.KurumlararasiNakil => "bg-dark",
                PersonelAktiflikDurum.Mustagfi => "bg-danger",
                PersonelAktiflikDurum.Vefat => "bg-dark",
                PersonelAktiflikDurum.Pasif => "bg-secondary",
                _ => "bg-secondary"
            };
        }

        /// <summary>
        /// Icon class'ı döndürür
        /// </summary>
        public static string GetIconClass(this PersonelAktiflikDurum durum)
        {
            return durum switch
            {
                PersonelAktiflikDurum.Aktif => "bx bx-check-circle",
                PersonelAktiflikDurum.AyliksizIzin => "bx bx-time",
                PersonelAktiflikDurum.GeciciGorevlendirme => "bx bx-transfer",
                PersonelAktiflikDurum.Askerlik => "bx bx-shield",
                PersonelAktiflikDurum.UcretsizIzin => "bx bx-calendar-x",
                PersonelAktiflikDurum.Emekli => "bx bx-user-check",
                PersonelAktiflikDurum.Istifa => "bx bx-log-out",
                PersonelAktiflikDurum.KurumlararasiNakil => "bx bx-transfer-alt",
                PersonelAktiflikDurum.Mustagfi => "bx bx-x-circle",
                PersonelAktiflikDurum.Vefat => "bx bx-heart",
                PersonelAktiflikDurum.Pasif => "bx bx-pause-circle",
                _ => "bx bx-question-mark"
            };
        }
    }
}
