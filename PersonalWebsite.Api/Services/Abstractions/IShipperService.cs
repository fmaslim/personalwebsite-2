using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IShipperService
    {
        Task<IEnumerable<ShipperDto>> GetAllShippersAsync();
    }
}
