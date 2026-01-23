using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex
{
    public interface ISiramatikQueryRepository
    {
        // Kanal Alt İşlem Sorguları
        Task<List<KanalAltIslemResponseDto>> GetAllKanalAltIslemlerAsync();
        Task<List<KanalAltIslemResponseDto>> GetKanalAltIslemlerByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId);
        Task<KanalAltIslemResponseDto?> GetKanalAltIslemByIdWithDetailsAsync(int kanalAltIslemId);
        Task<List<KanalAltIslemResponseDto>> GetKanalAltIslemlerByKanalIslemIdAsync(int kanalIslemId);
        
        // Kanal İşlem Sorguları
        Task<List<KanalIslemResponseDto>> GetKanalIslemlerByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId);
        Task<KanalIslemResponseDto?> GetKanalIslemByIdWithDetailsAsync(int kanalIslemId);
        
        // Kanal Personel Sorguları
        Task<List<KanalPersonelResponseDto>> GetKanalPersonellerByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId);
        Task<List<KanalPersonelResponseDto>> GetKanalPersonellerByKanalAltIslemIdAsync(int kanalAltIslemId);
        
        // İstatistik ve Dashboard Sorguları
        Task<Dictionary<int, int>> GetKanalAltIslemPersonelSayilariAsync(int departmanHizmetBinasiId);
        Task<List<KanalAltIslemResponseDto>> GetEslestirmeYapilmamisKanalAltIslemlerAsync(int departmanHizmetBinasiId);

        Task<List<KanalPersonelResponseDto>> GetPersonelKanalAtamalarByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId);
        
        // Personel Atama Matrix Sorgusu (Yeni Yapı)
        Task<List<PersonelAtamaMatrixDto>> GetPersonelAtamaMatrixByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId);

        // Banko Sıra Çağırma Paneli
        Task<List<SiraCagirmaResponseDto>> GetBankoPanelBekleyenSiralarAsync(string tcKimlikNo);

        /// <summary>
        /// ⭐ Personelin ilk çağrılabilir sırasını getirir (sadece tek sıra - performans için)
        /// </summary>
        Task<SiraCagirmaResponseDto?> GetIlkCagrilabilirSiraAsync(string tcKimlikNo);

        /// <summary>
        /// ⭐ Incremental Update: Belirli bir sıra alındığında/yönlendirildiğinde,
        /// o sırayı görebilecek TÜM personellerin güncel sıra listelerini getirir.
        /// SignalR ile her personele kendi listesi gönderilir.
        /// </summary>
        Task<List<SiraCagirmaResponseDto>> GetBankoPanelBekleyenSiralarBySiraIdAsync(int siraId);

        // SignalR Broadcast için etkilenen personelleri bul
        /// <summary>
        /// Belirli bir sıranın çağrılması/tamamlanması durumunda etkilenen personellerin TC listesini döner.
        /// Aynı KanalAltIslem'e atanmış ve banko modunda olan personeller etkilenir.
        /// </summary>
        Task<List<string>> GetSiraEtkilenenPersonellerAsync(int siraId);

        /// <summary>
        /// Belirli bir DepartmanHizmetBinasi ve KanalAltIslem için banko modunda olan personellerin TC listesini döner.
        /// </summary>
        Task<List<string>> GetBankoModundakiPersonellerAsync(int departmanHizmetBinasiId, int kanalAltIslemId);

        /// <summary>
        /// Belirli bir DepartmanHizmetBinasi ve KanalAltIslem için banko modunda olan ve en az Yrd.Uzman yetkisine sahip personellerin TC listesini döner.
        /// Kiosk sıra alma için kullanılır - sadece işlem yapabilecek personel varsa sıra alınabilir.
        /// </summary>
        Task<List<string>> GetBankoModundakiYetkiliPersonellerAsync(int departmanHizmetBinasiId, int kanalAltIslemId);

        // Kiosk Menü Sorguları
        /// <summary>
        /// Belirli bir Kiosk için menüleri detaylı olarak getirir
        /// Menüler için aktif personel sayısı, işlem sayısı gibi istatistiksel bilgileri içerir
        /// Aliağa SGM gibi spesifik kiosk'lar için kullanılır
        /// </summary>
        Task<List<KioskMenuDetayResponseDto>> GetKioskMenulerByKioskIdAsync(int kioskId);

        /// <summary>
        /// Belirli bir Kiosk'taki seçilen menü için alt kanal işlemlerini getirir
        /// Sadece aktif personel (Yrd.Uzman+) olan ve banko modunda bulunan işlemler döner
        /// Menü seçildikten sonra vatandaşın göreceği alt işlemler listesi
        /// </summary>
        Task<List<KioskAltIslemDto>> GetKioskMenuAltIslemleriByKioskIdAsync(int kioskId, int kioskMenuId);

        // Sıra Alma Sorguları
        /// <summary>
        /// KanalAltIslemId üzerinden sıra numarası bilgisini getirir
        /// Eski proje mantığı: KanalIslem bazında BaslangicNumara/BitisNumara kontrolü yapar
        /// BankoKullanici tablosu üzerinden personel kontrolü yapar
        /// </summary>
        Task<SiraNoBilgisiDto?> GetSiraNoAsync(int kanalAltIslemId);
    }
}
