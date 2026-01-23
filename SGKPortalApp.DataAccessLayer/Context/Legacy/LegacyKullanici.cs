using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.DataAccessLayer.Context.Legacy
{
    [Table("kullanici")]
    public class LegacyKullanici
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("edit")]
        public long? Edit { get; set; }

        [Column("edit_date")]
        public DateTime? EditDate { get; set; }

        [Column("username")]
        public long? Username { get; set; } // TC Kimlik No

        [Column("password")]
        [StringLength(32)]
        public string? Password { get; set; }

        [Column("kullaniciadi")]
        [StringLength(45)]
        public string KullaniciAdi { get; set; } = string.Empty; // AdSoyad

        [Column("sicilno")]
        public int SicilNo { get; set; }

        [Column("unvan")]
        [StringLength(255)]
        public string? Unvan { get; set; }

        [Column("unite")]
        [StringLength(30)]
        public string? Unite { get; set; }

        [Column("birim")]
        public int Birim { get; set; } // DepartmanId

        [Column("bina")]
        public int Bina { get; set; } // HizmetBinasiId

        [Column("servis")]
        [StringLength(50)]
        public string? Servis { get; set; } // ServisId

        [Column("gorev")]
        [StringLength(50)]
        public string? Gorev { get; set; }

        [Column("dahili")]
        public long Dahili { get; set; }

        [Column("email")]
        [StringLength(30)]
        public string? Email { get; set; }

        [Column("eposta")]
        [StringLength(30)]
        public string? Eposta { get; set; }

        [Column("ceptel")]
        public long CepTel { get; set; }

        [Column("ceptel2")]
        public long CepTel2 { get; set; }

        [Column("evtel")]
        public long EvTel { get; set; }

        [Column("yakinadsoyad")]
        [StringLength(45)]
        public string? YakinAdSoyad { get; set; }

        [Column("yakinderece")]
        [StringLength(25)]
        public string? YakinDerece { get; set; }

        [Column("yakintel")]
        public long YakinTel { get; set; }

        [Column("servis1")]
        public int Servis1 { get; set; } // UlasimServis1

        [Column("servis2")]
        public int Servis2 { get; set; } // UlasimServis2

        [Column("adres")]
        [StringLength(150)]
        public string? Adres { get; set; }

        [Column("semt")]
        [StringLength(20)]
        public string? Semt { get; set; }

        [Column("ilce")]
        [StringLength(25)]
        public string? Ilce { get; set; }

        [Column("il")]
        [StringLength(20)]
        public string? Il { get; set; }

        [Column("eshothat")]
        [StringLength(10)]
        public string? EshotHat { get; set; }

        [Column("durak")]
        [StringLength(20)]
        public string? Durak { get; set; }

        [Column("meddur")]
        public int MedeniDurum { get; set; } // 0=Bekar, 1=Evli

        [Column("dogumtarihi")]
        [StringLength(10)]
        public string? DogumTarihi { get; set; }

        [Column("esdurumu")]
        public int EsDurumu { get; set; } // EsininIsDurumu

        [Column("evdurum")]
        public int EvDurum { get; set; } // 0=Diger, 1=Kiraci, 2=Evsahibi

        [Column("cinsiyet")]
        [StringLength(5)]
        public string? Cinsiyet { get; set; } // '0'=Erkek, '1'=Kadin

        [Column("kan")]
        [StringLength(7)]
        public string? Kan { get; set; } // KanGrubu

        [Column("esisunvan")]
        [StringLength(35)]
        public string? EsIsUnvan { get; set; }

        [Column("esisadres")]
        [StringLength(100)]
        public string? EsIsAdres { get; set; }

        [Column("essemt")]
        [StringLength(25)]
        public string? EsSemt { get; set; }

        [Column("esisilce")]
        [StringLength(20)]
        public string? EsIsIlce { get; set; }

        [Column("esisil")]
        [StringLength(20)]
        public string? EsIsIl { get; set; }

        [Column("esadi")]
        [StringLength(20)]
        public string? EsAdi { get; set; }

        [Column("calisandurum")]
        public int? CalisanDurum { get; set; } // 1=Aktif, 2=Pasif, 3=Emekli

        [Column("emeklisicilno")]
        [StringLength(25)]
        public string? EmekliSicilNo { get; set; }

        [Column("kadroderecesi")]
        [StringLength(20)]
        public string? KadroDerecesi { get; set; }

        [Column("gorevebaslama")]
        [StringLength(20)]
        public string? GoreveBaslama { get; set; }

        [Column("bit_okul")]
        [StringLength(30)]
        public string? BitirdigiOkul { get; set; }

        [Column("bit_bolum")]
        [StringLength(30)]
        public string? BitirdigiBolum { get; set; }

        [Column("ogrenim_suresi")]
        public int? OgrenimSuresi { get; set; }

        [Column("tahsil")]
        [StringLength(20)]
        public string? Tahsil { get; set; }

        [Column("brans")]
        [StringLength(20)]
        public string? Brans { get; set; }

        [Column("sendika")]
        public int? Sendika { get; set; }

        [Column("sehit")]
        public int Sehit { get; set; } // 0=Degil, 1=Evet

        [Column("lastlogin")]
        public DateTime? LastLogin { get; set; }

        [Column("tabldot")]
        public int? Tabldot { get; set; }

        [Column("iban")]
        [StringLength(30)]
        public string? Iban { get; set; }

        [Column("pasifnedeni")]
        [StringLength(100)]
        public string? PasifNedeni { get; set; }

        [Column("kart_no")]
        [StringLength(11)]
        public string? KartNo { get; set; }

        [Column("kart_guncelleme_zamani")]
        public DateTime? KartGuncellemeZamani { get; set; }

        [Column("kart_aktif_tarihi")]
        public DateTime? KartAktifTarihi { get; set; }

        [Column("atanma")]
        public int? Atanma { get; set; }

        [Column("resim_yolu")]
        [StringLength(14)]
        public string? ResimYolu { get; set; }

        [Column("taseron")]
        [StringLength(10)]
        public string? Taseron { get; set; }
    }
}
