using Microsoft.AspNetCore.Http;

namespace PersonalWebsite.Api.DTOs
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public T? Data { get; set; }


        public static ServiceResult<T> Ok(T data, string message = "Operation successful", int statusCode = 200)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Message = message,
                StatusCode = statusCode,
                Data = data
            };
        }

        public static ServiceResult<T> Fail(string message, int statusCode = 400)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Data = default
            };
        }
    }
}
