using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IOrderServiceV2
    {
        string GetVersionMessage();
        Task<CreateOrderResponseV2Dto> CreateOrderAsync(CreateOrderRequestV2Dto dto);
    }
}
