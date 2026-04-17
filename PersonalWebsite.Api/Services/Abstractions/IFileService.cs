using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IFileService
    {
        Task<ServiceResult<FileUploadResponseDto>> UploadFileAsync(IFormFile file);
        Task<ServiceResult<FileDownloadResponseDto>> DownloadFileAsync(string fileName);
        Task<ServiceResult<string>> DeleteFileAsync(string fileName);
    }
}
