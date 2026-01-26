using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Legacy
{
    public interface ILegacyAtanmaRepository
    {
        Task<List<LegacyAtanma>> GetAllAsync();
    }
}
