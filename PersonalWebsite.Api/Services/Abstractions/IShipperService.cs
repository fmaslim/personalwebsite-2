using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IShipperService
    {
        Task<IEnumerable<ShipperDto>> GetAllShippersAsync();
        Task<ShipperDto?> GetShipperByIdAsync(int shipperId);
        Task<IEnumerable<ShipperDto>> SearchShippersAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir);
    }
}
