using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using System.Collections.Concurrent;
using System.Reflection;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    /// <summary>
    /// Field-level permission validation service
    /// Validates DTO field changes based on user permissions
    /// </summary>
    public class FieldPermissionValidationService : IFieldPermissionValidationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FieldPermissionValidationService> _logger;

        // Cache: Key = "DtoTypeName:FieldName", Value = (PermissionKey, MinYetkiSeviyesi)
        private readonly ConcurrentDictionary<string, (string PermissionKey, YetkiSeviyesi MinYetkiSeviyesi)> _fieldPermissionCache;

        public FieldPermissionValidationService(
            IUnitOfWork unitOfWork,
            ILogger<FieldPermissionValidationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _fieldPermissionCache = new ConcurrentDictionary<string, (string, YetkiSeviyesi)>();
        }

        /// <summary>
        /// Loads all field permissions from DB and caches them
        /// Should be called at startup
        /// </summary>
        public async Task LoadFieldPermissionsAsync()
        {
            try
            {
                _logger.LogInformation("Loading field permissions from database...");

                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var islemler = await repo.GetAllAsync();

                var fieldPermissions = islemler
                    .Where(i => !string.IsNullOrEmpty(i.DtoTypeName) && !string.IsNullOrEmpty(i.DtoFieldName))
                    .ToList();

                _fieldPermissionCache.Clear();

                foreach (var perm in fieldPermissions)
                {
                    var key = $"{perm.DtoTypeName}:{perm.DtoFieldName}";
                    _fieldPermissionCache[key] = (perm.PermissionKey, perm.MinYetkiSeviyesi);
                }

                _logger.LogInformation("Loaded {Count} field permissions into cache", _fieldPermissionCache.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading field permissions");
            }
        }

        /// <summary>
        /// Validates if user has permission to modify a specific DTO field
        /// </summary>
        public bool CanEditField(string dtoTypeName, string fieldName, Dictionary<string, YetkiSeviyesi> userPermissions, out string? permissionKey)
        {
            var key = $"{dtoTypeName}:{fieldName}";
            permissionKey = null;

            if (!_fieldPermissionCache.TryGetValue(key, out var fieldPerm))
            {
                // No field-level permission defined → allow (sayfa seviyesi kontrolü olmalı)
                return true;
            }

            permissionKey = fieldPerm.PermissionKey;

            // Check user's permission level for this field
            if (!userPermissions.TryGetValue(fieldPerm.PermissionKey, out var userLevel))
            {
                // User has no permission for this field
                return false;
            }

            // User must have at least the minimum required level
            return userLevel >= fieldPerm.MinYetkiSeviyesi;
        }

        /// <summary>
        /// Validates multiple fields in a DTO
        /// Returns list of fields that user cannot edit
        /// </summary>
        public async Task<List<string>> ValidateFieldPermissionsAsync<TDto>(
            TDto dto,
            Dictionary<string, YetkiSeviyesi> userPermissions,
            TDto? originalDto = null) where TDto : class
        {
            var unauthorizedFields = new List<string>();
            var dtoTypeName = typeof(TDto).Name;

            // Get all properties that have changed (if originalDto provided)
            var properties = typeof(TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                // Skip if not writable
                if (!prop.CanWrite || !prop.CanRead)
                    continue;

                // Check if field has changed (dirty checking)
                if (originalDto != null)
                {
                    var currentValue = prop.GetValue(dto);
                    var originalValue = prop.GetValue(originalDto);

                    // Skip unchanged fields
                    if (Equals(currentValue, originalValue))
                        continue;
                }

                // Validate permission for this field
                if (!CanEditField(dtoTypeName, prop.Name, userPermissions, out var permissionKey))
                {
                    _logger.LogWarning(
                        "User attempted to edit field without permission. DTO: {DtoType}, Field: {Field}, RequiredPermission: {Permission}",
                        dtoTypeName, prop.Name, permissionKey ?? "Unknown");

                    unauthorizedFields.Add(prop.Name);
                }
            }

            return unauthorizedFields;
        }

        /// <summary>
        /// Reverts unauthorized field changes back to original values
        /// </summary>
        public void RevertUnauthorizedFields<TDto>(
            TDto dto,
            TDto originalDto,
            List<string> unauthorizedFields) where TDto : class
        {
            foreach (var fieldName in unauthorizedFields)
            {
                var prop = typeof(TDto).GetProperty(fieldName);
                if (prop != null && prop.CanWrite && prop.CanRead)
                {
                    var originalValue = prop.GetValue(originalDto);
                    prop.SetValue(dto, originalValue);

                    _logger.LogInformation(
                        "Reverted unauthorized field change. DTO: {DtoType}, Field: {Field}",
                        typeof(TDto).Name, fieldName);
                }
            }
        }

        /// <summary>
        /// Refreshes the cache (call when permissions are updated)
        /// </summary>
        public async Task RefreshCacheAsync()
        {
            await LoadFieldPermissionsAsync();
        }
    }
}
