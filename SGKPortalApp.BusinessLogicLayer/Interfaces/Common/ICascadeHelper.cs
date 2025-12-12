using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    /// <summary>
    /// Generic Cascade Helper Interface
    /// Parent-Child ilişkilerinde Aktiflik ve Soft Delete işlemlerini yönetir
    /// EF Core tracking conflict'lerini otomatik olarak handle eder
    /// </summary>
    public interface ICascadeHelper
    {
        /// <summary>
        /// Belirtilen koşula uyan kayıtların Aktiflik durumunu günceller
        /// Zaten track edilen entity'leri otomatik olarak atlar
        /// </summary>
        /// <typeparam name="T">Entity tipi</typeparam>
        /// <param name="predicate">Filtreleme koşulu</param>
        /// <param name="yeniAktiflik">Yeni aktiflik durumu</param>
        /// <param name="excludeIds">Hariç tutulacak ID'ler (opsiyonel)</param>
        /// <returns>Güncellenen kayıt ID'leri</returns>
        Task<List<int>> CascadeAktiflikUpdateAsync<T>(
            Expression<Func<T, bool>> predicate,
            Aktiflik yeniAktiflik,
            HashSet<int>? excludeIds = null) where T : class;

        /// <summary>
        /// Belirtilen koşula uyan kayıtları soft delete yapar
        /// Zaten track edilen entity'leri otomatik olarak atlar
        /// </summary>
        /// <typeparam name="T">Entity tipi</typeparam>
        /// <param name="predicate">Filtreleme koşulu</param>
        /// <param name="excludeIds">Hariç tutulacak ID'ler (opsiyonel)</param>
        /// <returns>Silinen kayıt ID'leri</returns>
        Task<List<int>> CascadeSoftDeleteAsync<T>(
            Expression<Func<T, bool>> predicate,
            HashSet<int>? excludeIds = null) where T : class;

        /// <summary>
        /// Belirtilen koşula uyan kayıtları hem soft delete yapar hem de pasif eder
        /// </summary>
        /// <typeparam name="T">Entity tipi</typeparam>
        /// <param name="predicate">Filtreleme koşulu</param>
        /// <param name="excludeIds">Hariç tutulacak ID'ler (opsiyonel)</param>
        /// <returns>İşlem yapılan kayıt ID'leri</returns>
        Task<List<int>> CascadeDeleteWithPassiveAsync<T>(
            Expression<Func<T, bool>> predicate,
            HashSet<int>? excludeIds = null) where T : class;
    }
}
