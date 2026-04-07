using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class ShipperService : IShipperService
    {
        private readonly AdventureWorksContext _context;
        public ShipperService(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ShipperDto>> GetAllShippersAsync()
        {
            var shippers = await _context.ShipMethods
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new ShipperDto
                {
                    ShipperId = s.ShipMethodId,
                    ShipperName = s.Name
                })
                .ToListAsync();

            return shippers;
        }

        public async Task<ShipperDto?> GetShipperByIdAsync(int shipperId)
        {
            var shipper = _context.ShipMethods
                .AsNoTracking()
                .Where(s => s.ShipMethodId == shipperId)
                .Select(s => new ShipperDto
                {
                    ShipperId = s.ShipMethodId,
                    ShipperName = s.Name
                })
                .FirstOrDefaultAsync();

            return await shipper;
        }
    }
}
