using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class VendorService : IVendorService
    {
        private readonly AdventureWorksContext _context;
        public VendorService(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync()
        {
            var query =  _context.Vendors
                .AsNoTracking()
                .OrderBy(v => v.Name)
                .Take(5)
                .Select(v => new VendorDto
                {
                    VendorId = v.BusinessEntityId,
                    VendorName = v.Name
                });

            return await query.ToListAsync();
        }
    }
}
