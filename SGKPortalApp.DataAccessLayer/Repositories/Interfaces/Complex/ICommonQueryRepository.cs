using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex
{
    /// <summary>
    /// Common (Ortak) modüller için kompleks sorguları barındıran repository interface
    /// HizmetBinası, İl, İlçe gibi ortak tablolar için join'li, aggregate sorgular
    /// </summary>
    public interface ICommonQueryRepository
    {
        /// <summary>
        /// Hizmet Binasında çalışan personellerin servislerini DISTINCT olarak getirir
        /// Her servisin o binada kaç personeli olduğu bilgisi ile birlikte döner
        /// </summary>
        /// <param name="hizmetBinasiId">Hizmet Binası ID</param>
        /// <returns>Servis listesi (ServisId, ServisAdi, PersonelSayisi)</returns>
        Task<IEnumerable<(int ServisId, string ServisAdi, int PersonelSayisi)>> GetServislerByHizmetBinasiIdAsync(int hizmetBinasiId);
    }
}
