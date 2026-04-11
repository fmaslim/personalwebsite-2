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

        public async Task<VendorDto?> GetVendorByIdAsync(int vendorId)
        {
            var vendor = _context.Vendors
                .AsNoTracking()
                .Where(v => v.BusinessEntityId == vendorId)
                .Select(v => new VendorDto
                {
                    VendorId = v.BusinessEntityId,
                    VendorName = v.Name
                })
                .FirstOrDefaultAsync();

            return await vendor;
        }

        public async Task<IEnumerable<VendorDto>> SearchVendorsByNameAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir)
        {
            // pagination
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;
            var skip = (page - 1) * pageSize;
            sortBy = sortBy?.Trim().ToLower();
            sortDir= sortDir?.Trim().ToLower();

            bool desc = sortDir == "desc";
            

            var query = _context.Vendors
            .AsNoTracking()
            .Where(v => string.IsNullOrEmpty(name) || v.Name.Contains(name));

            if (sortBy == "vendorid")
            {
                query = desc
                    ? query.OrderByDescending(v => v.BusinessEntityId)
                    : query.OrderBy(v => v.BusinessEntityId);
            }
            else
            {
                query = desc
                    ? query.OrderByDescending(v => v.Name)
                    : query.OrderBy(v => v.Name);
            }

            //if (sortDir?.ToLower() == "asc")
            //{
            //    query = query.OrderBy(v => v.Name);
            //}
            //else
            //{
            //    query = query.OrderByDescending(v => v.Name);
            //}

            var vendors = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(v => new VendorDto
                {
                    VendorId = v.BusinessEntityId,
                    VendorName = v.Name
                })
                .ToListAsync();

            return vendors;
        }
    }
}
