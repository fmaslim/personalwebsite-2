using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly AdventureWorksContext _context;
        public FileService(AdventureWorksContext context) 
        { 
            _context = context; 
        }
        public async Task<ServiceResult<FileUploadResponseDto>> UploadFileAsync(IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                return new ServiceResult<FileUploadResponseDto>
                {
                    Success = false,
                    Message = "No file uploaded.",
                    StatusCode = 400
                };
            }

            var maxFileSize = 1 * 1024 * 1024; // 1 MB
            if (file.Length > maxFileSize) // Limit file size to 1 MB
            {
                return new ServiceResult<FileUploadResponseDto>
                {
                    Success = false,
                    Message = $"File size exceeds the limit of {maxFileSize / (1024 * 1024)} MB",
                    StatusCode = 400
                };
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            // Thursday, 04/16/2026
            // added unique identifier to file name to prevent overwriting existing files
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{fileExtension}";

            if (!allowedExtensions.Contains(fileExtension))
            {
                return new ServiceResult<FileUploadResponseDto>
                {
                    Success = false,
                    Message = "Unsupported file type. Only .jpg, .jpeg, .png, .pdf, .docx files are allowed.",
                    StatusCode = 400
                };
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var response = new FileUploadResponseDto
            {
                FilePath = filePath,
                FileName = uniqueFileName,
                FileSize = file.Length,
                // Message = "File uploaded successfully"
            };
            return new ServiceResult<FileUploadResponseDto>
            {
                Success = true,
                Data = response,
                StatusCode = 200
            };
        }
    }
}
