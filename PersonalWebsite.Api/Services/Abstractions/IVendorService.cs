using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IVendorService
    {
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto?> GetVendorByIdAsync(int vendorId);
        Task<IEnumerable<VendorDto>> SearchVendorsByNameAsync(string? name, int page, int pageSize, string sortBy, string SortDir);
    }
}
