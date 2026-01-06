using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PdksIslemleri
{
    public class SpecialCardRepository : GenericRepository<SpecialCard>, ISpecialCardRepository
    {
        public SpecialCardRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<SpecialCard?> GetByCardNumberAsync(long cardNumber)
        {
            return await _context.SpecialCards
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber && !c.SilindiMi);
        }

        public async Task<SpecialCard?> GetByEnrollNumberAsync(string enrollNumber)
        {
            return await _context.SpecialCards
                .FirstOrDefaultAsync(c => c.EnrollNumber == enrollNumber && !c.SilindiMi);
        }

        public async Task<List<SpecialCard>> GetByCardTypeAsync(CardType cardType)
        {
            return await _context.SpecialCards
                .Where(c => c.CardType == cardType && !c.SilindiMi)
                .ToListAsync();
        }

        public async Task<List<SpecialCard>> GetActiveCardsAsync()
        {
            return await _context.SpecialCards
                .Where(s => !s.SilindiMi)
                .ToListAsync();
        }

        public async Task<List<SpecialCard>> GetAvailableCardsAsync()
        {
            return await _context.SpecialCards
                .Where(c => !c.SilindiMi)
                .ToListAsync();
        }
    }
}
