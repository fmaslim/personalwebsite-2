using Microsoft.AspNetCore.Http;

namespace PersonalWebsite.Api.DTOs.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public List<ServiceError> Errors { get; set; } = new();
        public int StatusCode { get; set; }
        public T? Data { get; set; }


        public static ServiceResult<T> Ok(T data, string message = "Operation successful", int statusCode = 200)
        {
            return new ServiceResult<T>
            {
                Success = true,
                
                StatusCode = statusCode,
                Data = data
            };
        }

        public static ServiceResult<T> Fail(string message, int statusCode = 400)
        {
            return new ServiceResult<T>
            {
                Success = false,
                
                StatusCode = statusCode,
                Data = default
            };
        }
    }
}
