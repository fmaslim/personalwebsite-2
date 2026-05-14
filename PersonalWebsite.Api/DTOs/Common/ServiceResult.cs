using Microsoft.AspNetCore.Http;
using PersonalWebsite.Api.Models.Errors;
using System.Reflection.Metadata.Ecma335;

namespace PersonalWebsite.Api.DTOs.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public List<ServiceError> Errors { get; set; } = new();
        public List<FieldError> FieldErrors { get; set; } = new();
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        // Added these properties for AppInsight testing
        public bool IsSuccess { get; set; }
        public string Code { get; set; } = string.Empty;
        public string ErrorType { get; set; } = string.Empty;
        public ServiceErrorType ServiceErrorType { get; set; } = ServiceErrorType.None;

        public string Message { get; set; } = string.Empty;


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
                Errors = new List<ServiceError>
                {
                    new ServiceError
                    {
                        Message = message,
                        // Field = field,
                        Code = "Error",
                        Type = ServiceErrorType.Validation
                    }
                }
            };
        }

        public static ServiceResult<T> Fail(
    string message,
    ServiceErrorType type = ServiceErrorType.Validation)
        {
            return new ServiceResult<T>
            {
                Success = false,
                StatusCode = type switch
                {
                    ServiceErrorType.Validation => StatusCodes.Status400BadRequest,
                    ServiceErrorType.NotFound => StatusCodes.Status404NotFound,
                    ServiceErrorType.Conflict => StatusCodes.Status409Conflict,
                    ServiceErrorType.Unexpected => StatusCodes.Status500InternalServerError,
                    _ => StatusCodes.Status500InternalServerError
                },
                Data = default,
                Errors = new List<ServiceError>
        {
            new ServiceError
            {
                Message = message,
                Code = type.ToString(),
                Type = type
            }
        },
                ServiceErrorType = type,
                Message = message
            };
        }

        public static ServiceResult<T> Fail(string message, int statusCode = 400, string? field = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                StatusCode = statusCode,
                Data = default,
                Errors = new List<ServiceError>
                {
                    new ServiceError
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
                Errors = errors.Select(error => new ServiceError
                {
                    Message = error,
                    Code = "Error",
                    Type = ServiceErrorType.Validation
                }).ToList()
            };
        }

        public static ServiceResult<T> Fail(
        List<ServiceError> errors,
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

        public static ServiceResult<T> Fail(string code, string message, int statusCode)
        {
            return new ServiceResult<T>
            {
                Success = false,
                StatusCode = statusCode,
                Errors = new List<ServiceError>
                {
                    new ServiceError
                    {
                        Code = code,
                        Message = message
                    }
                }
            };
        }

        public static ServiceResult<T> NotFound(string message, string? field = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                ServiceErrorType = ServiceErrorType.NotFound,
                Message = message,
                Data = default,
                Errors = new List<ServiceError>()
                {
                    new ServiceError
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
                ServiceErrorType = ServiceErrorType.Conflict,
                Message = message,
                Data = default,
                Errors = new List<ServiceError>()
                {
                    new ServiceError
                    {
                        Code = "Conflict",
                        Message = message,
                        Field = field,
                        Type = ServiceErrorType.Conflict
                    }
                }
            };
        }

        public static ServiceResult<T> ValidationFail(List<FieldError> fieldErrors)
        {
            return new ServiceResult<T>
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                ServiceErrorType = ServiceErrorType.Validation,
                ErrorType = "Validation",
                Message = "one or more validation errors occurred",
                Errors = new List<ServiceError>
                {
                    new ServiceError
                    {
                        Code = "ValidationError",
                        Message = "one or more validation errors occurred"
                    }
                },
                FieldErrors = fieldErrors
            };
        }

        public static ServiceResult<T> Created(T data, string message = "")
        {
            return new ServiceResult<T>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Data = data,
                Message = message
            };
        }

        public static ServiceResult<T> NoContent(string message = "")
        {
            return new ServiceResult<T>
            {
                Success = true,
                StatusCode = StatusCodes.Status204NoContent,
                Data = default,
                Message = message
            };
        }
    }
}
