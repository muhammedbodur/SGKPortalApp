using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri
{
    public interface ISpecialCardRepository : IGenericRepository<SpecialCard>
    {
        Task<SpecialCard?> GetByCardNumberAsync(long cardNumber);
        Task<SpecialCard?> GetByEnrollNumberAsync(string enrollNumber);
        Task<List<SpecialCard>> GetByCardTypeAsync(CardType cardType);
        Task<List<SpecialCard>> GetActiveCardsAsync();
        Task<List<SpecialCard>> GetAvailableCardsAsync();
        
        // EnrollNumber yönetimi
        Task<string> GetNextAvailableEnrollNumberAsync();
    }
}
