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

        public async Task<IEnumerable<ShipperDto>> SearchShippersAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir)
        {
            var query = _context.ShipMethods.AsNoTracking();

            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize >= 50 ? 50 : pageSize;

            name = string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();
            sortBy = sortBy?.Trim().ToLower();
            sortDir = sortDir?.Trim().ToLower();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            bool desc = sortDir == "desc";
            if (sortBy == "name")
            {
                query = desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            }
            else
            {
                query = desc ? query.OrderByDescending(x => x.ShipMethodId) : query.OrderBy(x => x.ShipMethodId);
            }

            var shippers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ShipperDto
                {
                    ShipperId = s.ShipMethodId,
                    ShipperName = s.Name
                })
                .ToListAsync();

            return shippers;
        }
    }
}
