using Microsoft.AspNetCore.Http;
using PersonalWebsite.Api.Models.Errors;

namespace PersonalWebsite.Api.DTOs.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        // public List<ServiceError> Errors { get; set; } = new();
        // public List<ServiceError> Errors { get; set; } = new List<ServiceError>();
        public List<PersonalWebsite.Api.Models.Errors.ServiceError> Errors { get; set; } = new();
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
                Data = default,
                Errors = new List<PersonalWebsite.Api.Models.Errors.ServiceError>
                {
                    new PersonalWebsite.Api.Models.Errors.ServiceError
                    {
                        Message = message,
                        // Field = field,
                        Code = "Error",
                        Type = ServiceErrorType.Validation
                    }
                }
            };
        }

        public static ServiceResult<T> Fail(string message, int statusCode = 400, string? field = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                StatusCode = statusCode,
                Data = default,
                Errors = new List<PersonalWebsite.Api.Models.Errors.ServiceError>
                {
                    new PersonalWebsite.Api.Models.Errors.ServiceError
                    {
                        Message = message,
                        // Field = field,
                        Code = "Error",
                        Type = ServiceErrorType.Validation
                    }
                }
            };
        }

        public static ServiceResult<T> Fail(List<string> errors, int statusCode = 400)
        {
            return new ServiceResult<T>
            {
                Success = false,
                StatusCode = statusCode,
                Data = default,
                Errors = errors.Select(error => new PersonalWebsite.Api.Models.Errors.ServiceError
                {
                    Message = error,
                    Code = "Error",
                    Type = ServiceErrorType.Validation
                }).ToList()
            };
        }

        public static ServiceResult<T> Fail(
        List<PersonalWebsite.Api.Models.Errors.ServiceError> errors,
        int statusCode = 400)
            {
                return new ServiceResult<T>
                {
                    Success = false,
                    StatusCode = statusCode,
                    Data = default,
                    Errors = errors
                };
            }

        public static ServiceResult<T> NotFound(string message, string? field = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Data = default,
                Errors = new List<PersonalWebsite.Api.Models.Errors.ServiceError>()
                {
                    new Models.Errors.ServiceError
                    {
                        Code = "NotFound",
                        Message = message,
                        Field = field,
                        Type = ServiceErrorType.NotFound
                    }
                }
            };
        }

        public static ServiceResult<T> Conflict(string message, string? field = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                StatusCode = StatusCodes.Status409Conflict,
                Data = default,
                Errors = new List<PersonalWebsite.Api.Models.Errors.ServiceError>()
                {
                    new Models.Errors.ServiceError
                    {
                        Code = "Conflict",
                        Message = message,
                        Field = field,
                        Type = ServiceErrorType.Conflict
                    }
                }
            };
        }
    }
}
