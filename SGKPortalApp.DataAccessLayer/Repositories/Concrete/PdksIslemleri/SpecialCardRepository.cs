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
                .Include(s => s.HizmetBinasi)
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber && !c.SilindiMi);
        }

        public async Task<SpecialCard?> GetByEnrollNumberAsync(string enrollNumber)
        {
            return await _context.SpecialCards
                .Include(s => s.HizmetBinasi)
                .FirstOrDefaultAsync(c => c.EnrollNumber == enrollNumber && !c.SilindiMi);
        }

        public async Task<List<SpecialCard>> GetByCardTypeAsync(CardType cardType)
        {
            return await _context.SpecialCards
                .Include(s => s.HizmetBinasi)
                .Where(c => c.CardType == cardType && !c.SilindiMi)
                .ToListAsync();
        }

        public async Task<List<SpecialCard>> GetActiveCardsAsync()
        {
            return await _context.SpecialCards
                .Include(s => s.HizmetBinasi)
                .Where(s => !s.SilindiMi)
                .ToListAsync();
        }

        public async Task<List<SpecialCard>> GetAvailableCardsAsync()
        {
            return await _context.SpecialCards
                .Where(c => !c.SilindiMi)
                .ToListAsync();
        }

        public async Task<string> GetNextAvailableEnrollNumberAsync()
        {
            // Özel kartlar için EnrollNumber aralığı: 60000-65534
            var maxEnrollNumber = await _context.SpecialCards
                .Where(c => !c.SilindiMi)
                .Select(c => c.EnrollNumber)
                .OrderByDescending(e => e)
                .FirstOrDefaultAsync();

            int nextNumber;
            
            // Eğer hiç özel kart yoksa 60000'den başla
            if (string.IsNullOrEmpty(maxEnrollNumber))
            {
                nextNumber = 60000;
            }
            else
            {
                // Mevcut max değeri parse et ve +1 ekle
                if (int.TryParse(maxEnrollNumber, out var currentMax))
                {
                    nextNumber = currentMax < 60000 ? 60000 : currentMax + 1;
                }
                else
                {
                    // Parse edilemezse 60000'den başla
                    nextNumber = 60000;
                }
            }

            // 65534'ü geçmemeli
            if (nextNumber > 65534)
                throw new InvalidOperationException("Özel kart EnrollNumber kapasitesi doldu (max: 65534)");

            return nextNumber.ToString();
        }
    }
}
