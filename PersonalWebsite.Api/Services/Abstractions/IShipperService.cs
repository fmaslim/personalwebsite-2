using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IShipperService
    {
        Task<IEnumerable<ShipperDto>> GetAllShippersAsync();
        Task<ShipperDto?> GetShipperByIdAsync(int shipperId);
        Task<ServiceResult<PagedResponse<ShipperDto>>> SearchShippersAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir);
    }
}
