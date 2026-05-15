using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;
using PersonalWebsite.Api.Services.Abstractions;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<ServiceResult<VendorDto>> GetVendorByIdV2Async(int vendorId)
        {
            if (vendorId <= 0)
            {
                return ServiceResult<VendorDto>.Fail(
                    "VendorId must be greater than 0.", 
                    Models.Errors.ServiceErrorType.Validation);
            }

            var vendor = await _context.Vendors
                .AsNoTracking()
                .Where(v => v.BusinessEntityId == vendorId)
                .Select(v => new VendorDto
                {
                    VendorId = v.BusinessEntityId,
                    VendorName = v.Name
                })
                .FirstOrDefaultAsync();

            if (vendor == null)
            {
                return ServiceResult<VendorDto>.NotFound(
                $"Vendor with ID {vendorId} does not exist.",
                "VendorId");
            }

            return ServiceResult<VendorDto>.Ok(vendor);
        }

        public async Task<ServiceResult<PagedResponse<VendorDto>>> SearchVendorsByNameAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir)
        {
            // collect validation errors 
            var errorList = new List<string>();
            if (page < 1)
            {
                errorList.Add("Page number must be greater than 0.");
            }
            if (pageSize < 1)
            {
                errorList.Add("Page size must be greater than 0.");
            }
            if (pageSize > 50)
            {
                errorList.Add("Page size cannot exceed 50.");
            }
            var validSortBy = new[] { "vendorid", "name" };
            if (!validSortBy.Contains(sortBy?.Trim().ToLower()))
            {
                errorList.Add("SortBy must be either 'vendorid' or 'name'.");
            }
            var validSortDir = new[] { "asc", "desc" };
            if (!validSortDir.Contains(sortDir?.Trim().ToLower()))
            {
                errorList.Add("SortDir must be either 'asc' or 'desc'.");
            }
            if (errorList.Any())
            {
                return ServiceResult<PagedResponse<VendorDto>>.Fail(errorList);
            }
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

            var totalRecords = await query.CountAsync();
            var vendors = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(v => new VendorDto
                {
                    VendorId = v.BusinessEntityId,
                    VendorName = v.Name
                })
                .ToListAsync();

            var pagedResponse = new PagedResponse<VendorDto>
            {
                Data = vendors,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
            
            return ServiceResult<PagedResponse<VendorDto>>.Ok(pagedResponse);
        }
    }
}
