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
    }
}
