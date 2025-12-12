using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    /// <summary>
    /// Generic Cascade Helper Implementation
    /// Parent-Child ilişkilerinde Aktiflik ve Soft Delete işlemlerini yönetir
    /// EF Core tracking conflict'lerini otomatik olarak handle eder
    /// </summary>
    public class CascadeHelper : ICascadeHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CascadeHelper> _logger;

        public CascadeHelper(IUnitOfWork unitOfWork, ILogger<CascadeHelper> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<int>> CascadeAktiflikUpdateAsync<T>(
            Expression<Func<T, bool>> predicate,
            Aktiflik yeniAktiflik,
            HashSet<int>? excludeIds = null) where T : class
        {
            var updatedIds = new List<int>();
            excludeIds ??= new HashSet<int>();

            try
            {
                // Entity'nin Aktiflik property'si var mı kontrol et
                var aktiflikProperty = typeof(T).GetProperty("Aktiflik");
                if (aktiflikProperty == null)
                {
                    _logger.LogDebug("Cascade Aktiflik: {EntityType} Aktiflik property'si yok, atlanıyor", typeof(T).Name);
                    return updatedIds;
                }

                var repository = _unitOfWork.Repository<T>();
                var entities = await repository.FindAsync(predicate);

                foreach (var entity in entities)
                {
                    var entityId = GetEntityId(entity);
                    
                    // Zaten işlenmiş veya track ediliyorsa atla
                    if (excludeIds.Contains(entityId) || _unitOfWork.IsTracked(entity))
                    {
                        _logger.LogDebug("Cascade Aktiflik: {EntityType} ID={Id} atlandı (zaten işlenmiş veya tracked)", 
                            typeof(T).Name, entityId);
                        continue;
                    }

                    // Aktiflik property'sini güncelle
                    aktiflikProperty.SetValue(entity, yeniAktiflik);
                    SetPropertyValue(entity, "DuzenlenmeTarihi", DateTime.Now);

                    repository.Update(entity);
                    updatedIds.Add(entityId);
                }

                _logger.LogInformation("Cascade Aktiflik Update: {EntityType}, Yeni={Aktiflik}, Güncellenen={Count}", 
                    typeof(T).Name, yeniAktiflik, updatedIds.Count);

                return updatedIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cascade Aktiflik Update hatası: {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public async Task<List<int>> CascadeSoftDeleteAsync<T>(
            Expression<Func<T, bool>> predicate,
            HashSet<int>? excludeIds = null) where T : class
        {
            var deletedIds = new List<int>();
            excludeIds ??= new HashSet<int>();

            try
            {
                var repository = _unitOfWork.Repository<T>();
                var entities = await repository.FindAsync(predicate);

                foreach (var entity in entities)
                {
                    var entityId = GetEntityId(entity);
                    
                    // Zaten işlenmiş veya track ediliyorsa atla
                    if (excludeIds.Contains(entityId) || _unitOfWork.IsTracked(entity))
                    {
                        _logger.LogDebug("Cascade SoftDelete: {EntityType} ID={Id} atlandı (zaten işlenmiş veya tracked)", 
                            typeof(T).Name, entityId);
                        continue;
                    }

                    // Soft delete property'lerini güncelle
                    SetPropertyValue(entity, "SilindiMi", true);
                    SetPropertyValue(entity, "DuzenlenmeTarihi", DateTime.Now);
                    SetPropertyValue(entity, "SilinmeTarihi", DateTime.Now);

                    repository.Update(entity);
                    deletedIds.Add(entityId);
                }

                _logger.LogInformation("Cascade Soft Delete: {EntityType}, Silinen={Count}", 
                    typeof(T).Name, deletedIds.Count);

                return deletedIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cascade Soft Delete hatası: {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public async Task<List<int>> CascadeDeleteWithPassiveAsync<T>(
            Expression<Func<T, bool>> predicate,
            HashSet<int>? excludeIds = null) where T : class
        {
            var processedIds = new List<int>();
            excludeIds ??= new HashSet<int>();

            try
            {
                var repository = _unitOfWork.Repository<T>();
                var entities = await repository.FindAsync(predicate);

                foreach (var entity in entities)
                {
                    var entityId = GetEntityId(entity);
                    
                    // Zaten işlenmiş veya track ediliyorsa atla
                    if (excludeIds.Contains(entityId) || _unitOfWork.IsTracked(entity))
                    {
                        _logger.LogDebug("Cascade DeleteWithPassive: {EntityType} ID={Id} atlandı (zaten işlenmiş veya tracked)", 
                            typeof(T).Name, entityId);
                        continue;
                    }

                    // Soft delete + Pasif yap
                    SetPropertyValue(entity, "SilindiMi", true);
                    SetPropertyValue(entity, "Aktiflik", Aktiflik.Pasif);
                    SetPropertyValue(entity, "DuzenlenmeTarihi", DateTime.Now);
                    SetPropertyValue(entity, "SilinmeTarihi", DateTime.Now);

                    repository.Update(entity);
                    processedIds.Add(entityId);
                }

                _logger.LogInformation("Cascade Delete With Passive: {EntityType}, İşlenen={Count}", 
                    typeof(T).Name, processedIds.Count);

                return processedIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cascade Delete With Passive hatası: {EntityType}", typeof(T).Name);
                throw;
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Entity'nin primary key değerini alır
        /// Convention: EntityNameId veya Id
        /// </summary>
        private int GetEntityId<T>(T entity) where T : class
        {
            var type = typeof(T);
            
            // Önce EntityNameId pattern'ini dene (örn: KanalAltIslemId)
            var idPropertyName = $"{type.Name}Id";
            var idProperty = type.GetProperty(idPropertyName);
            
            // Bulunamazsa "Id" dene
            if (idProperty == null)
            {
                idProperty = type.GetProperty("Id");
            }

            if (idProperty == null)
            {
                throw new InvalidOperationException($"Entity {type.Name} için ID property bulunamadı");
            }

            var value = idProperty.GetValue(entity);
            return value != null ? (int)value : 0;
        }

        /// <summary>
        /// Entity'nin belirtilen property'sine değer atar
        /// Property yoksa sessizce atlar
        /// </summary>
        private void SetPropertyValue<T>(T entity, string propertyName, object value) where T : class
        {
            var property = typeof(T).GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(entity, value);
            }
        }

        #endregion
    }
}
