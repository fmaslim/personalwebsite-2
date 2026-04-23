using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IOrderServiceV2
    {
        string GetVersionMessage();
        Task<ServiceResult<CreateOrderResponseV2Dto>> CreateOrderAsync(CreateOrderRequestV2Dto dto);

        Task<ServiceResult<CreateOrderResponseV2Dto>> CreateOrderMultiErrorAsync(CreateOrderRequestV2Dto dto);
    }
}
