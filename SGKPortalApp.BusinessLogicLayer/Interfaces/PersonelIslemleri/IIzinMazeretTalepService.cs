using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri
{
    /// <summary>
    /// İzin/Mazeret talepleri business service interface
    /// Çakışma kontrolü, onay workflow ve raporlama içerir
    /// </summary>
    public interface IIzinMazeretTalepService
    {
        // ═══════════════════════════════════════════════════════
        // CRUD İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Tüm talepleri getir
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetAllAsync();

        /// <summary>
        /// ID'ye göre talep getir
        /// </summary>
        /// <param name="id">Talep ID</param>
        /// <param name="currentUserTc">İsteği yapan kullanıcının TC'si (ownership kontrolü için)</param>
        Task<ApiResponseDto<IzinMazeretTalepResponseDto>> GetByIdAsync(int id, string? currentUserTc = null);

        /// <summary>
        /// Yeni izin/mazeret talebi oluştur
        /// Otomatik çakışma kontrolü yapar!
        /// </summary>
        Task<ApiResponseDto<IzinMazeretTalepResponseDto>> CreateAsync(IzinMazeretTalepCreateRequestDto request);

        /// <summary>
        /// İzin/mazeret talebini güncelle
        /// Sadece beklemedeki talepler güncellenebilir
        /// </summary>
        /// <param name="id">Talep ID</param>
        /// <param name="request">Güncelleme bilgileri</param>
        /// <param name="currentUserTc">İsteği yapan kullanıcının TC'si (ownership kontrolü için)</param>
        Task<ApiResponseDto<IzinMazeretTalepResponseDto>> UpdateAsync(int id, IzinMazeretTalepUpdateRequestDto request, string? currentUserTc = null);

        /// <summary>
        /// İzin/mazeret talebini sil (soft delete)
        /// </summary>
        /// <param name="id">Talep ID</param>
        /// <param name="currentUserTc">İsteği yapan kullanıcının TC'si (ownership kontrolü için)</param>
        Task<ApiResponseDto<bool>> DeleteAsync(int id, string? currentUserTc = null);

        /// <summary>
        /// İzin/mazeret talebini iptal et (talep sahibi iptal edebilir)
        /// </summary>
        Task<ApiResponseDto<bool>> CancelAsync(int id, string iptalNedeni);

        // ═══════════════════════════════════════════════════════
        // PERSONEL BAZINDA SORGULAR
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Personele ait tüm talepler
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetByPersonelTcAsync(string tcKimlikNo, bool includeInactive = false);

        /// <summary>
        /// Personele ait bekleyen talepler
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingByPersonelTcAsync(string tcKimlikNo);

        /// <summary>
        /// Personele ait onaylanmış talepler (tarih aralığı ile)
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetApprovedByPersonelTcAsync(
            string tcKimlikNo,
            DateTime? startDate = null,
            DateTime? endDate = null);

        // ═══════════════════════════════════════════════════════
        // ONAY İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// 1. veya 2. onayci olarak talebi onayla/reddet
        /// </summary>
        Task<ApiResponseDto<bool>> ApproveOrRejectAsync(
            int talepId,
            string onayciTcKimlikNo,
            IzinMazeretTalepOnayRequestDto request);

        /// <summary>
        /// 1. Onayci için bekleyen talepler
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingForFirstApproverAsync(string onayciTcKimlikNo);

        /// <summary>
        /// 2. Onayci için bekleyen talepler
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingForSecondApproverAsync(string onayciTcKimlikNo);

        /// <summary>
        /// Departman bazında bekleyen talepler (yönetici görünümü)
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingByDepartmanAsync(int departmanId);

        /// <summary>
        /// Servis bazında bekleyen talepler
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetPendingByServisAsync(int servisId);

        /// <summary>
        /// Kullanıcının seçebileceği onaycıları getir
        /// Kurallar:
        /// - Unvan ID = 7: Tüm departmanlarda görünür
        /// - Servis ID = 33: Sadece kendi departmanında görünür
        /// - Unvan ID IN (5,87): Aynı departman VE aynı servis
        /// </summary>
        Task<ApiResponseDto<List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.PersonelResponseDto>>> GetAvailableApproversAsync(string tcKimlikNo);

        /// <summary>
        /// Aktif izin türlerini getir
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTuruResponseDto>>> GetAvailableLeaveTypesAsync();

        // ═══════════════════════════════════════════════════════
        // RAPORLAMA VE FİLTRELEME
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Gelişmiş filtreleme ile talepleri getir (raporlama için)
        /// Permission-based filtering destekler
        /// </summary>
        Task<ApiResponseDto<IzinMazeretTalepFilterResponseDto>> GetFilteredAsync(
            IzinMazeretTalepFilterRequestDto filter);

        /// <summary>
        /// Tarih aralığındaki tüm izin/mazeret talepleri (rapor için)
        /// </summary>
        Task<ApiResponseDto<List<IzinMazeretTalepListResponseDto>>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int? departmanId = null,
            int? servisId = null);

        // ═══════════════════════════════════════════════════════
        // İSTATİSTİKLER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Personelin yıllık izin kullanım sayısı
        /// </summary>
        Task<ApiResponseDto<int>> GetTotalYillikIzinDaysAsync(string tcKimlikNo, int year);

        /// <summary>
        /// Personelin toplam kullanılan izin günü (türe göre)
        /// </summary>
        Task<ApiResponseDto<int>> GetTotalUsedDaysAsync(
            string tcKimlikNo,
            int? izinTuruValue = null,
            int? year = null);

        // ═══════════════════════════════════════════════════════
        // ÇAKIŞMA KONTROLÜ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirtilen tarih aralığında personelin başka bir izni/mazereti var mı?
        /// Çakışma kontrolü için (ÖNEMLİ!)
        /// </summary>
        Task<ApiResponseDto<bool>> CheckOverlapAsync(
            string tcKimlikNo,
            DateTime? baslangicTarihi,
            DateTime? bitisTarihi,
            DateTime? mazeretTarihi,
            int? excludeTalepId = null);

        // ═══════════════════════════════════════════════════════
        // SGK İŞLEM TAKİBİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// SGK sistemine işle veya işlemi geri al
        /// </summary>
        Task<ApiResponseDto<bool>> ProcessSgkIslemAsync(IzinSgkIslemRequestDto request, string kullaniciTc);
    }
}
