using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex
{
    public interface ISiramatikQueryRepository
    {
        // Kanal Alt İşlem Sorguları
        Task<List<KanalAltIslemResponseDto>> GetAllKanalAltIslemlerAsync();
        Task<List<KanalAltIslemResponseDto>> GetKanalAltIslemlerByHizmetBinasiIdAsync(int hizmetBinasiId);
        Task<KanalAltIslemResponseDto?> GetKanalAltIslemByIdWithDetailsAsync(int kanalAltIslemId);
        Task<List<KanalAltIslemResponseDto>> GetKanalAltIslemlerByKanalIslemIdAsync(int kanalIslemId);
        
        // Kanal İşlem Sorguları
        Task<List<KanalIslemResponseDto>> GetKanalIslemlerByHizmetBinasiIdAsync(int hizmetBinasiId);
        Task<KanalIslemResponseDto?> GetKanalIslemByIdWithDetailsAsync(int kanalIslemId);
        
        // Kanal Personel Sorguları
        Task<List<KanalPersonelResponseDto>> GetKanalPersonellerByHizmetBinasiIdAsync(int hizmetBinasiId);
        Task<List<KanalPersonelResponseDto>> GetKanalPersonellerByKanalAltIslemIdAsync(int kanalAltIslemId);
        
        // İstatistik ve Dashboard Sorguları
        Task<Dictionary<int, int>> GetKanalAltIslemPersonelSayilariAsync(int hizmetBinasiId);
        Task<List<KanalAltIslemResponseDto>> GetEslestirmeYapilmamisKanalAltIslemlerAsync(int hizmetBinasiId);

        Task<List<KanalPersonelResponseDto>> GetPersonelKanalAtamalarByHizmetBinasiIdAsync(int hizmetBinasiId);
        
        // Personel Atama Matrix Sorgusu (Yeni Yapı)
        Task<List<PersonelAtamaMatrixDto>> GetPersonelAtamaMatrixByHizmetBinasiIdAsync(int hizmetBinasiId);

        // Banko Sıra Çağırma Paneli
        Task<List<SiraCagirmaResponseDto>> GetBankoPanelBekleyenSiralarAsync(string tcKimlikNo);

        // SignalR Broadcast için etkilenen personelleri bul
        /// <summary>
        /// Belirli bir sıranın çağrılması/tamamlanması durumunda etkilenen personellerin TC listesini döner.
        /// Aynı KanalAltIslem'e atanmış ve banko modunda olan personeller etkilenir.
        /// </summary>
        Task<List<string>> GetSiraEtkilenenPersonellerAsync(int siraId);

        /// <summary>
        /// Belirli bir HizmetBinasi ve KanalAltIslem için banko modunda olan personellerin TC listesini döner.
        /// </summary>
        Task<List<string>> GetBankoModundakiPersonellerAsync(int hizmetBinasiId, int kanalAltIslemId);

        /// <summary>
        /// Belirli bir HizmetBinasi ve KanalAltIslem için banko modunda olan ve en az Yrd.Uzman yetkisine sahip personellerin TC listesini döner.
        /// Kiosk sıra alma için kullanılır - sadece işlem yapabilecek personel varsa sıra alınabilir.
        /// </summary>
        Task<List<string>> GetBankoModundakiYetkiliPersonellerAsync(int hizmetBinasiId, int kanalAltIslemId);
    }
}
