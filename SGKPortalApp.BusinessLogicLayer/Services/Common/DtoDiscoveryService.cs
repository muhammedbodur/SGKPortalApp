using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    /// <summary>
    /// DTO Discovery Service - Reflection ile DTO tiplerini ve property'lerini keşfeder
    /// </summary>
    public class DtoDiscoveryService : IDtoDiscoveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DtoDiscoveryService> _logger;

        public DtoDiscoveryService(IUnitOfWork unitOfWork, ILogger<DtoDiscoveryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Tüm *RequestDto sınıflarını listeler (Reflection ile)
        /// </summary>
        public ApiResponseDto<List<DtoTypeInfo>> GetAllDtoTypes()
        {
            try
            {
                // Assembly'yi al - PersonelCreateRequestDto'nun bulunduğu assembly
                var assembly = Assembly.GetAssembly(typeof(PersonelCreateRequestDto));

                if (assembly == null)
                {
                    return ApiResponseDto<List<DtoTypeInfo>>.ErrorResult("DTO assembly bulunamadı");
                }

                // Assembly'deki tüm tipleri tara ve RequestDto olanları filtrele
                var dtoTypes = assembly.GetTypes()
                    .Where(t =>
                        t.Name.EndsWith("RequestDto") &&  // İsim kontrolü
                        !t.IsAbstract &&                   // Abstract değil
                        t.IsClass &&                       // Class olmalı
                        t.IsPublic &&                      // Public olmalı
                        t.Namespace != null)               // Namespace var
                    .Select(t => new DtoTypeInfo
                    {
                        TypeName = t.Name,
                        FullName = t.FullName ?? t.Name,
                        DisplayName = FormatDisplayName(t.Name),
                        Category = GetCategory(t.Namespace),
                        Namespace = t.Namespace
                    })
                    .OrderBy(d => d.Category)
                    .ThenBy(d => d.DisplayName)
                    .ToList();

                _logger.LogDebug("GetAllDtoTypes: {Count} DTO bulundu", dtoTypes.Count);

                return ApiResponseDto<List<DtoTypeInfo>>.SuccessResult(dtoTypes, $"{dtoTypes.Count} DTO bulundu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllDtoTypes hatası");
                return ApiResponseDto<List<DtoTypeInfo>>.ErrorResult($"DTO'lar yüklenirken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Belirtilen DTO'nun tüm property'lerini döner (Reflection ile)
        /// </summary>
        public ApiResponseDto<List<DtoPropertyInfo>> GetDtoProperties(string dtoTypeName)
        {
            try
            {
                // Assembly'yi al
                var assembly = Assembly.GetAssembly(typeof(PersonelCreateRequestDto));

                if (assembly == null)
                {
                    return ApiResponseDto<List<DtoPropertyInfo>>.ErrorResult("DTO assembly bulunamadı");
                }

                // Tip adıyla DTO'yu bul
                var dtoType = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name == dtoTypeName);

                if (dtoType == null)
                {
                    return ApiResponseDto<List<DtoPropertyInfo>>.ErrorResult($"'{dtoTypeName}' bulunamadı");
                }

                // DTO'nun tüm property'lerini al (Reflection)
                var properties = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p =>
                        !p.PropertyType.IsGenericType ||  // List<> gibi collection'ları atla
                        !p.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                    .Select(p => new DtoPropertyInfo
                    {
                        PropertyName = p.Name,
                        PropertyType = GetFriendlyTypeName(p.PropertyType),
                        DisplayName = FormatDisplayName(p.Name),
                        IsRequired = p.GetCustomAttribute<RequiredAttribute>() != null,
                        MaxLength = p.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength
                    })
                    .OrderBy(p => p.DisplayName)
                    .ToList();

                _logger.LogDebug("GetDtoProperties: {DtoTypeName} için {Count} property bulundu", dtoTypeName, properties.Count);

                return ApiResponseDto<List<DtoPropertyInfo>>.SuccessResult(properties, $"{properties.Count} property bulundu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDtoProperties hatası: {DtoTypeName}", dtoTypeName);
                return ApiResponseDto<List<DtoPropertyInfo>>.ErrorResult($"Property'ler yüklenirken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Field Analysis - DTO'daki tüm field'ları + hangilerinin korumalı olduğunu döner
        /// </summary>
        public async Task<ApiResponseDto<FieldAnalysisResult>> GetFieldAnalysisAsync(string pageKey, string dtoTypeName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pageKey) || string.IsNullOrWhiteSpace(dtoTypeName))
                {
                    return ApiResponseDto<FieldAnalysisResult>.ErrorResult("pageKey ve dtoTypeName parametreleri zorunludur");
                }

                // 1️⃣ DTO'nun tüm property'lerini al (Reflection)
                var assembly = Assembly.GetAssembly(typeof(PersonelCreateRequestDto));
                if (assembly == null)
                {
                    return ApiResponseDto<FieldAnalysisResult>.ErrorResult("DTO assembly bulunamadı");
                }

                var dtoType = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name == dtoTypeName);

                if (dtoType == null)
                {
                    return ApiResponseDto<FieldAnalysisResult>.ErrorResult($"'{dtoTypeName}' bulunamadı");
                }

                var allFields = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p =>
                        !p.PropertyType.IsGenericType ||
                        !p.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                    .Select(p => new
                    {
                        PropertyName = p.Name,
                        PropertyType = GetFriendlyTypeName(p.PropertyType),
                        DisplayName = FormatDisplayName(p.Name),
                        IsRequired = p.GetCustomAttribute<RequiredAttribute>() != null
                    })
                    .ToList();

                // 2️⃣ Veritabanından korumalı field'ları al
                var protectedFields = await _unitOfWork.Repository<BusinessObjectLayer.Entities.Common.ModulControllerIslem>()
                    .FindAsync(i => i.PermissionKey != null &&
                                i.PermissionKey.StartsWith(pageKey + ".FIELD.") &&
                                i.DtoFieldName != null);

                var protectedFieldsList = protectedFields.Select(i => new
                {
                    i.DtoFieldName,
                    i.PermissionKey,
                    i.ModulControllerIslemAdi,
                    i.MinYetkiSeviyesi
                }).ToList();

                // 3️⃣ Birleştir - Her field için korumalı mı değil mi?
                var analysisResult = allFields.Select(f => new FieldAnalysisInfo
                {
                    FieldName = f.PropertyName,
                    DisplayName = f.DisplayName,
                    PropertyType = f.PropertyType,
                    IsRequired = f.IsRequired,
                    IsProtected = protectedFieldsList.Any(pf =>
                        pf.DtoFieldName != null &&
                        pf.DtoFieldName.Equals(f.PropertyName, StringComparison.OrdinalIgnoreCase)),
                    CanAddPermission = !protectedFieldsList.Any(pf =>
                        pf.DtoFieldName != null &&
                        pf.DtoFieldName.Equals(f.PropertyName, StringComparison.OrdinalIgnoreCase)),
                    ExistingPermissionKey = protectedFieldsList
                        .FirstOrDefault(pf =>
                            pf.DtoFieldName != null &&
                            pf.DtoFieldName.Equals(f.PropertyName, StringComparison.OrdinalIgnoreCase))
                        ?.PermissionKey
                }).ToList();

                var result = new FieldAnalysisResult
                {
                    PageKey = pageKey,
                    DtoTypeName = dtoTypeName,
                    TotalFields = analysisResult.Count,
                    ProtectedFields = analysisResult.Count(f => f.IsProtected),
                    AvailableFields = analysisResult.Count(f => !f.IsProtected),
                    Fields = analysisResult
                };

                _logger.LogDebug("GetFieldAnalysisAsync: {PageKey}/{DtoTypeName} - {Total} field ({Protected} korumalı)", 
                    pageKey, dtoTypeName, result.TotalFields, result.ProtectedFields);

                return ApiResponseDto<FieldAnalysisResult>.SuccessResult(result, 
                    $"{result.TotalFields} field bulundu ({result.ProtectedFields} korumalı, {result.AvailableFields} eklenebilir)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFieldAnalysisAsync hatası: {PageKey}, {DtoTypeName}", pageKey, dtoTypeName);
                return ApiResponseDto<FieldAnalysisResult>.ErrorResult($"Field analysis sırasında hata: {ex.Message}");
            }
        }

        #region Helper Methods

        /// <summary>
        /// DTO adını kullanıcı dostu formata çevirir
        /// Örnek: "PersonelCreateRequestDto" → "Personel - Create"
        /// </summary>
        private static string FormatDisplayName(string name)
        {
            // "RequestDto" sonekini kaldır
            if (name.EndsWith("RequestDto"))
            {
                name = name.Substring(0, name.Length - "RequestDto".Length);
            }

            // PascalCase'i boşluklarla ayır
            var spaced = System.Text.RegularExpressions.Regex.Replace(
                name,
                "([a-z])([A-Z])",
                "$1 $2"
            );

            // "Create", "Update" gibi kelimeleri ayır
            spaced = spaced.Replace("Create", "- Create")
                          .Replace("Update", "- Update")
                          .Replace("Delete", "- Delete");

            return spaced.Trim();
        }

        /// <summary>
        /// Namespace'den kategori çıkarır
        /// Örnek: "SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri" → "Personel İşlemleri"
        /// </summary>
        private static string GetCategory(string? ns)
        {
            if (string.IsNullOrEmpty(ns))
                return "Diğer";

            var parts = ns.Split('.');
            var lastPart = parts.LastOrDefault() ?? "Diğer";

            // "PersonelIslemleri" → "Personel İşlemleri"
            return System.Text.RegularExpressions.Regex.Replace(
                lastPart,
                "([a-z])([A-Z])",
                "$1 $2"
            );
        }

        /// <summary>
        /// Type adını kullanıcı dostu formata çevirir
        /// Örnek: "System.String" → "string", "System.Int32" → "int"
        /// </summary>
        private static string GetFriendlyTypeName(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(DateTime)) return "DateTime";
            if (type == typeof(DateTime?)) return "DateTime?";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(double)) return "double";
            if (type == typeof(int?)) return "int?";

            // Nullable check
            if (Nullable.GetUnderlyingType(type) != null)
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return GetFriendlyTypeName(underlyingType!) + "?";
            }

            // Enum
            if (type.IsEnum)
            {
                return type.Name;
            }

            return type.Name;
        }

        #endregion
    }
}
