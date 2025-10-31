using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Exceptions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using System.Text;
using System.Text.Json;

namespace SGKPortalApp.BusinessLogicLayer.Services.PersonelIslemleri
{
    public class PersonelService : IPersonelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PersonelService> _logger;

        public PersonelService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PersonelService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetAllAsync()
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetAllWithDetailsAsync();
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Personeller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel listesi getirilirken hata oluştu");
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Personeller getirilirken bir hata oluştu", ex.Message);
            }
        }


        public async Task<ApiResponseDto<PersonelResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Personel bulunamadı");
                }

                var personelDto = _mapper.Map<PersonelResponseDto>(personel);

                return ApiResponseDto<PersonelResponseDto>
                    .SuccessResult(personelDto, "Personel başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel getirilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult("Personel getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelResponseDto>> CreateAsync(PersonelCreateRequestDto request)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();

                // TC Kimlik No kontrolü
                var existingPersonel = await personelRepo.GetByTcKimlikNoAsync(request.TcKimlikNo);
                if (existingPersonel != null)
                {
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Bu TC Kimlik No ile kayıtlı personel zaten mevcut");
                }

                var personel = _mapper.Map<Personel>(request);
                await personelRepo.AddAsync(personel);
                await _unitOfWork.SaveChangesAsync();

                var personelDto = _mapper.Map<PersonelResponseDto>(personel);
                return ApiResponseDto<PersonelResponseDto>
                    .SuccessResult(personelDto, "Personel başarıyla oluşturuldu");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel oluşturulurken hata oluştu");
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult("Personel oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelResponseDto>> UpdateAsync(string tcKimlikNo, PersonelUpdateRequestDto request)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Personel bulunamadı");
                }

                _mapper.Map(request, personel);
                personelRepo.Update(personel);
                await _unitOfWork.SaveChangesAsync();

                var personelDto = _mapper.Map<PersonelResponseDto>(personel);
                return ApiResponseDto<PersonelResponseDto>
                    .SuccessResult(personelDto, "Personel başarıyla güncellendi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel güncellenirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<PersonelResponseDto>
                    .ErrorResult("Personel güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(string tcKimlikNo)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<bool>
                        .ErrorResult("Personel bulunamadı");
                }

                personelRepo.Delete(personel);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponseDto<bool>
                    .SuccessResult(true, "Personel başarıyla silindi");
            }
            catch (DatabaseException ex)
            {
                _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                return ApiResponseDto<bool>
                    .ErrorResult(ex.UserFriendlyMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel silinirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<bool>
                    .ErrorResult("Personel silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetActiveAsync()
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetActiveAsync();
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Aktif personeller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif personeller getirilirken hata oluştu");
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Aktif personeller getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>> GetPagedAsync(PersonelFilterRequestDto filter)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var pagedResult = await personelRepo.GetPagedAsync(filter);
                
                return ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>
                    .SuccessResult(pagedResult, "Sayfalı personel listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı personel listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>
                    .ErrorResult("Sayfalı personel listesi getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetByDepartmanAsync(int departmanId)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetByDepartmanAsync(departmanId);
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Departman personelleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman personelleri getirilirken hata oluştu. DepartmanId: {DepartmanId}", departmanId);
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Departman personelleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetByServisAsync(int servisId)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetByServisAsync(servisId);
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Servis personelleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis personelleri getirilirken hata oluştu. ServisId: {ServisId}", servisId);
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Servis personelleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<PersonelResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personeller = await personelRepo.GetByHizmetBinasiIdAsync(hizmetBinasiId);
                var personelDtos = _mapper.Map<List<PersonelResponseDto>>(personeller);

                return ApiResponseDto<List<PersonelResponseDto>>
                    .SuccessResult(personelDtos, "Hizmet binası personelleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binası personelleri getirilirken hata oluştu. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<PersonelResponseDto>>
                    .ErrorResult("Hizmet binası personelleri getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PersonelResponseDto>> CreateCompleteAsync(PersonelCompleteRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    // 1. Personel kaydet
                    var personel = _mapper.Map<Personel>(request.Personel);
                    await _unitOfWork.Repository<Personel>().AddAsync(personel);
                    await _unitOfWork.SaveChangesAsync();

                    var tcKimlikNo = personel.TcKimlikNo;

                    // 2. Çocukları kaydet
                    if (request.Cocuklar?.Any() == true)
                    {
                        foreach (var cocukDto in request.Cocuklar)
                        {
                            cocukDto.PersonelTcKimlikNo = tcKimlikNo;
                            var cocuk = _mapper.Map<PersonelCocuk>(cocukDto);
                            await _unitOfWork.Repository<PersonelCocuk>().AddAsync(cocuk);
                        }
                    }

                    // 3. Hizmetleri kaydet
                    if (request.Hizmetler?.Any() == true)
                    {
                        foreach (var hizmetDto in request.Hizmetler)
                        {
                            hizmetDto.TcKimlikNo = tcKimlikNo;
                            var hizmet = _mapper.Map<PersonelHizmet>(hizmetDto);
                            await _unitOfWork.Repository<PersonelHizmet>().AddAsync(hizmet);
                        }
                    }

                    // 4. Eğitimleri kaydet
                    if (request.Egitimler?.Any() == true)
                    {
                        foreach (var egitimDto in request.Egitimler)
                        {
                            egitimDto.TcKimlikNo = tcKimlikNo;
                            var egitim = _mapper.Map<PersonelEgitim>(egitimDto);
                            await _unitOfWork.Repository<PersonelEgitim>().AddAsync(egitim);
                        }
                    }

                    // 5. İmza Yetkilerini kaydet
                    if (request.ImzaYetkileri?.Any() == true)
                    {
                        foreach (var yetkiDto in request.ImzaYetkileri)
                        {
                            yetkiDto.TcKimlikNo = tcKimlikNo;
                            var yetki = _mapper.Map<PersonelImzaYetkisi>(yetkiDto);
                            await _unitOfWork.Repository<PersonelImzaYetkisi>().AddAsync(yetki);
                        }
                    }

                    // 6. Cezaları kaydet
                    if (request.Cezalar?.Any() == true)
                    {
                        foreach (var cezaDto in request.Cezalar)
                        {
                            cezaDto.TcKimlikNo = tcKimlikNo;
                            var ceza = _mapper.Map<PersonelCeza>(cezaDto);
                            await _unitOfWork.Repository<PersonelCeza>().AddAsync(ceza);
                        }
                    }

                    // 7. Engelleri kaydet
                    if (request.Engeller?.Any() == true)
                    {
                        foreach (var engelDto in request.Engeller)
                        {
                            engelDto.TcKimlikNo = tcKimlikNo;
                            var engel = _mapper.Map<PersonelEngel>(engelDto);
                            await _unitOfWork.Repository<PersonelEngel>().AddAsync(engel);
                        }
                    }

                    // Tüm değişiklikleri kaydet
                    await _unitOfWork.SaveChangesAsync();

                    // Personel bilgisini geri döndür
                    var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                    var savedPersonel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);
                    var personelDto = _mapper.Map<PersonelResponseDto>(savedPersonel);

                    return ApiResponseDto<PersonelResponseDto>
                        .SuccessResult(personelDto, "Personel ve tüm bilgileri başarıyla kaydedildi");
                }
                catch (DatabaseException ex)
                {
                    _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult(ex.UserFriendlyMessage, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Personel toplu kayıt sırasında hata oluştu");
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Personel kaydedilirken bir hata oluştu. Tüm işlemler geri alındı.", ex.Message);
                }
            });
        }

        public async Task<ApiResponseDto<PersonelResponseDto>> UpdateCompleteAsync(string tcKimlikNo, PersonelCompleteRequestDto request)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    // 1. Personel güncelle
                    var personel = await _unitOfWork.Repository<Personel>().GetByIdAsync(tcKimlikNo);
                    if (personel == null)
                        return ApiResponseDto<PersonelResponseDto>.ErrorResult("Personel bulunamadı");

                    _mapper.Map(request.Personel, personel);
                    _unitOfWork.Repository<Personel>().Update(personel);
                    await _unitOfWork.SaveChangesAsync();

                    // 2. Mevcut alt kayıtları sil
                    var mevcutCocuklar = await _unitOfWork.Repository<PersonelCocuk>()
                        .FindAsync(c => c.PersonelTcKimlikNo == tcKimlikNo);
                    foreach (var cocuk in mevcutCocuklar)
                        _unitOfWork.Repository<PersonelCocuk>().Delete(cocuk);

                    var mevcutHizmetler = await _unitOfWork.Repository<PersonelHizmet>()
                        .FindAsync(h => h.TcKimlikNo == tcKimlikNo);
                    foreach (var hizmet in mevcutHizmetler)
                        _unitOfWork.Repository<PersonelHizmet>().Delete(hizmet);

                    var mevcutEgitimler = await _unitOfWork.Repository<PersonelEgitim>()
                        .FindAsync(e => e.TcKimlikNo == tcKimlikNo);
                    foreach (var egitim in mevcutEgitimler)
                        _unitOfWork.Repository<PersonelEgitim>().Delete(egitim);

                    var mevcutYetkiler = await _unitOfWork.Repository<PersonelImzaYetkisi>()
                        .FindAsync(y => y.TcKimlikNo == tcKimlikNo);
                    foreach (var yetki in mevcutYetkiler)
                        _unitOfWork.Repository<PersonelImzaYetkisi>().Delete(yetki);

                    var mevcutCezalar = await _unitOfWork.Repository<PersonelCeza>()
                        .FindAsync(c => c.TcKimlikNo == tcKimlikNo);
                    foreach (var ceza in mevcutCezalar)
                        _unitOfWork.Repository<PersonelCeza>().Delete(ceza);

                    var mevcutEngeller = await _unitOfWork.Repository<PersonelEngel>()
                        .FindAsync(e => e.TcKimlikNo == tcKimlikNo);
                    foreach (var engel in mevcutEngeller)
                        _unitOfWork.Repository<PersonelEngel>().Delete(engel);

                    await _unitOfWork.SaveChangesAsync();

                    // 3. Yeni kayıtları ekle (CreateCompleteAsync ile aynı mantık)
                    if (request.Cocuklar?.Any() == true)
                    {
                        foreach (var cocukDto in request.Cocuklar)
                        {
                            cocukDto.PersonelTcKimlikNo = tcKimlikNo;
                            var cocuk = _mapper.Map<PersonelCocuk>(cocukDto);
                            await _unitOfWork.Repository<PersonelCocuk>().AddAsync(cocuk);
                        }
                    }

                    if (request.Hizmetler?.Any() == true)
                    {
                        foreach (var hizmetDto in request.Hizmetler)
                        {
                            hizmetDto.TcKimlikNo = tcKimlikNo;
                            var hizmet = _mapper.Map<PersonelHizmet>(hizmetDto);
                            await _unitOfWork.Repository<PersonelHizmet>().AddAsync(hizmet);
                        }
                    }

                    if (request.Egitimler?.Any() == true)
                    {
                        foreach (var egitimDto in request.Egitimler)
                        {
                            egitimDto.TcKimlikNo = tcKimlikNo;
                            var egitim = _mapper.Map<PersonelEgitim>(egitimDto);
                            await _unitOfWork.Repository<PersonelEgitim>().AddAsync(egitim);
                        }
                    }

                    if (request.ImzaYetkileri?.Any() == true)
                    {
                        foreach (var yetkiDto in request.ImzaYetkileri)
                        {
                            yetkiDto.TcKimlikNo = tcKimlikNo;
                            var yetki = _mapper.Map<PersonelImzaYetkisi>(yetkiDto);
                            await _unitOfWork.Repository<PersonelImzaYetkisi>().AddAsync(yetki);
                        }
                    }

                    if (request.Cezalar?.Any() == true)
                    {
                        foreach (var cezaDto in request.Cezalar)
                        {
                            cezaDto.TcKimlikNo = tcKimlikNo;
                            var ceza = _mapper.Map<PersonelCeza>(cezaDto);
                            await _unitOfWork.Repository<PersonelCeza>().AddAsync(ceza);
                        }
                    }

                    if (request.Engeller?.Any() == true)
                    {
                        foreach (var engelDto in request.Engeller)
                        {
                            engelDto.TcKimlikNo = tcKimlikNo;
                            var engel = _mapper.Map<PersonelEngel>(engelDto);
                            await _unitOfWork.Repository<PersonelEngel>().AddAsync(engel);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();

                    // Güncellenmiş personel bilgisini döndür
                    var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                    var updatedPersonel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);
                    var personelDto = _mapper.Map<PersonelResponseDto>(updatedPersonel);

                    return ApiResponseDto<PersonelResponseDto>
                        .SuccessResult(personelDto, "Personel ve tüm bilgileri başarıyla güncellendi");
                }
                catch (DatabaseException ex)
                {
                    _logger.LogWarning(ex, "Veritabanı kısıtlama hatası: {ErrorType}", ex.ErrorType);
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult(ex.UserFriendlyMessage, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Personel toplu güncelleme sırasında hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                    return ApiResponseDto<PersonelResponseDto>
                        .ErrorResult("Personel güncellenirken bir hata oluştu. Tüm işlemler geri alındı.", ex.Message);
                }
            });
        }
    }
}
