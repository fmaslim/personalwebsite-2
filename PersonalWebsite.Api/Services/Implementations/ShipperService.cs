using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
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

        public async Task<ServiceResult<PagedResponse<ShipperDto>>> SearchShippersAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir)
        {
            var errors = new List<string>();

            IQueryable<ShipMethod> query = _context.ShipMethods.AsNoTracking();

            //page = page <= 0 ? 1 : page;
            //pageSize = pageSize <= 0 ? 10 : pageSize;
            //pageSize = pageSize >= 50 ? 50 : pageSize;

            if (page <= 0)
            {
                errors.Add("Page number must be greater than 0.");
            }
            if (pageSize <= 0)
            {
                errors.Add("Page size must be greater than 0.");
            }
            else if (pageSize > 50)
            {
                errors.Add("Page size cannot exceed 50.");
            }

            //name = string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();
            //sortBy = sortBy?.Trim().ToLower();
            //sortDir = sortDir?.Trim().ToLower();

            name = string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy.Trim().ToLower();
            sortDir = string.IsNullOrWhiteSpace(sortDir) ? "asc" : sortDir.Trim().ToLower();

            var allowedSortBy = new[] { "id", "name" };
            var allowedSortDir = new[] { "asc", "desc" };
            if (!allowedSortBy.Contains(sortBy))
            {
                errors.Add($"Invalid sortBy value. Allowed values are: {string.Join(", ", allowedSortBy)}.");
            }
            
            if (!allowedSortDir.Contains(sortDir))
            {
                errors.Add($"Invalid sortDir value. Allowed values are: {string.Join(", ", allowedSortDir)}.");
            }

            if (errors.Any())
            {
                return ServiceResult<PagedResponse<ShipperDto>>.Fail(errors);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            //if (sortBy == "name")
            //{
            //    query = desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            //}
            //else
            //{
            //    query = desc ? query.OrderByDescending(x => x.ShipMethodId) : query.OrderBy(x => x.ShipMethodId);
            //}

            query = (sortBy, sortDir) switch
            {
                ("name", "asc") => query.OrderBy(x => x.Name),
                ("name", "desc") => query.OrderByDescending(x => x.Name),
                ("id", "asc") => query.OrderBy(x => x.ShipMethodId),
                ("id", "desc") => query.OrderByDescending(x => x.ShipMethodId),
                _ => query.OrderBy(x => x.ShipMethodId)
            };

            var totalCount = await query.CountAsync();
            var skip = (page - 1) * pageSize;

            var shippers = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(s => new ShipperDto
                {
                    ShipperId = s.ShipMethodId,
                    ShipperName = s.Name
                })
                .ToListAsync();

            return ServiceResult<PagedResponse<ShipperDto>>.Ok(new PagedResponse<ShipperDto>
            {
                Items = shippers,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
    }
}
