using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.API.Controllers.Common
{
    [Route("api/common/dto-discovery")]
    [ApiController]
    public class DtoDiscoveryController : ControllerBase
    {
        /// <summary>
        /// Tüm *RequestDto sınıflarını listeler (Reflection ile)
        /// </summary>
        [HttpGet("dto-types")]
        public IActionResult GetAllDtoTypes()
        {
            try
            {
                // 1️⃣ Assembly'yi al - PersonelCreateRequestDto'nun bulunduğu assembly
                var assembly = Assembly.GetAssembly(typeof(PersonelCreateRequestDto));

                if (assembly == null)
                {
                    return BadRequest("DTO assembly bulunamadı");
                }

                // 2️⃣ Assembly'deki tüm tipleri tara ve RequestDto olanları filtrele
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

                return Ok(new ServiceResult<List<DtoTypeInfo>>
                {
                    Success = true,
                    Data = dtoTypes,
                    Message = new[] { $"{dtoTypes.Count} DTO bulundu" }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResult<List<DtoTypeInfo>>
                {
                    Success = false,
                    Message = new[] { $"DTO'lar yüklenirken hata: {ex.Message}" }
                });
            }
        }

        /// <summary>
        /// Belirtilen DTO'nun tüm property'lerini döner (Reflection ile)
        /// </summary>
        [HttpGet("dto-properties/{dtoTypeName}")]
        public IActionResult GetDtoProperties(string dtoTypeName)
        {
            try
            {
                // 1️⃣ Assembly'yi al
                var assembly = Assembly.GetAssembly(typeof(PersonelCreateRequestDto));

                if (assembly == null)
                {
                    return BadRequest("DTO assembly bulunamadı");
                }

                // 2️⃣ Tip adıyla DTO'yu bul
                var dtoType = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name == dtoTypeName);

                if (dtoType == null)
                {
                    return NotFound(new ServiceResult<List<DtoPropertyInfo>>
                    {
                        Success = false,
                        Message = new[] { $"'{dtoTypeName}' bulunamadı" }
                    });
                }

                // 3️⃣ DTO'nun tüm property'lerini al (Reflection)
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

                return Ok(new ServiceResult<List<DtoPropertyInfo>>
                {
                    Success = true,
                    Data = properties,
                    Message = new[] { $"{properties.Count} property bulundu" }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResult<List<DtoPropertyInfo>>
                {
                    Success = false,
                    Message = new[] { $"Property'ler yüklenirken hata: {ex.Message}" }
                });
            }
        }

        #region Helper Methods

        /// <summary>
        /// DTO adını kullanıcı dostu formata çevirir
        /// Örnek: "PersonelCreateRequestDto" → "Personel - Create"
        /// </summary>
        private string FormatDisplayName(string name)
        {
            // "RequestDto" sonekini kaldır
            if (name.EndsWith("RequestDto"))
            {
                name = name.Substring(0, name.Length - "RequestDto".Length);
            }

            // PascalCase'i boşluklarla ayır
            // "PersonelCreate" → "Personel Create"
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
        private string GetCategory(string? ns)
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
        private string GetFriendlyTypeName(Type type)
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

    #region Response Models

    public class DtoTypeInfo
    {
        public string TypeName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Namespace { get; set; }
    }

    public class DtoPropertyInfo
    {
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public int? MaxLength { get; set; }
    }

    #endregion
}
