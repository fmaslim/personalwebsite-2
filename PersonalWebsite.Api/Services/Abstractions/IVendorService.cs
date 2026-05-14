using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IVendorService
    {
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto?> GetVendorByIdAsync(int vendorId);
        Task<ServiceResult<VendorDto>> GetVendorByIdV2Async(int vendorId);
        Task<IEnumerable<VendorDto>> SearchVendorsByNameAsync(string? name, int page, int pageSize, string? sortBy, string? SortDir);
    }
}
