using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class AtanmaNedeniRepository : GenericRepository<AtanmaNedenleri>, IAtanmaNedeniRepository
    {
        public AtanmaNedeniRepository(SGKDbContext context) : base(context)
        {
        }
    }
}
