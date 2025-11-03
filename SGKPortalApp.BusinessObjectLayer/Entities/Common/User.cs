using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// User Tablosu - Personellerin SADECE dinamik (sık değişen) verilerini tutar
    /// - Şifre, oturum, aktiflik, giriş başarısızlık sayısı gibi işlemsel veriler
    /// - Personel tablosu ile 1-1 ilişkili (TcKimlikNo üzerinden)
    /// 
    /// NOT: Email, KullaniciAdi, TelefonNo gibi statik veriler Personel tablosunda!
    /// </summary>
    public class User : AuditableEntity
    {
        [Key]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        // Personel ile One-to-One ilişki
        [ForeignKey(nameof(TcKimlikNo))]
        public Personel? Personel { get; set; }

        // Güvenlik Bilgileri (Dinamik)
        [Required]
        [StringLength(255)]
        public string PassWord { get; set; } = string.Empty;

        [StringLength(100)]
        public string? SessionID { get; set; }

        // Durum Bilgileri (Dinamik)
        [Required]
        public bool AktifMi { get; set; } = false;

        public DateTime? SonGirisTarihi { get; set; }
        public int BasarisizGirisSayisi { get; set; } = 0;
        public DateTime? HesapKilitTarihi { get; set; }

        // SignalR Bağlantısı (One-to-One)
        public HubConnection? HubConnection { get; set; }

        public User()
        {
            // Varsayılan şifre TC Kimlik No olacak
            PassWord = TcKimlikNo;
        }
    }
}