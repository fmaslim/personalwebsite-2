using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IFileService
    {
        Task<ServiceResult<FileUploadResponseDto>> UploadFileAsync(IFormFile file);
    }
}
