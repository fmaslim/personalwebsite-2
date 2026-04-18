using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IFileService
    {
        Task<ServiceResult<FileUploadResponseDto>> UploadFileAsync(IFormFile file);
        Task<ServiceResult<FileDownloadResponseDto>> DownloadFileAsync(string fileName);
        Task<ServiceResult<string>> DeleteFileAsync(string fileName);
        Task<ServiceResult<bool>> DeleteFileByIdAsync(int id);
        Task<List<FileListItemDto>> GetAllFilesAsync();
        Task<FileRecord?> GetFileByIdAsync(int id);
        Task<FileDetailsResponseDto?> GetFileDetailsByIdAsync(int id);
    }
}
