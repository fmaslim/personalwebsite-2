using Microsoft.EntityFrameworkCore;
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

        public Task<ServiceResult<string>> DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Task.FromResult(new ServiceResult<string>
                {
                    Success = false,
                    Message = "File name is required.",
                    StatusCode = 400
                });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return Task.FromResult(new ServiceResult<string>
                {
                    Success = false,
                    Message = "File not found.",
                    StatusCode = 404
                });
            }

            System.IO.File.Delete(filePath);
            return Task.FromResult(new ServiceResult<string>
            {
                Success = true,
                Message = "File deleted successfully.",
                StatusCode = 200
            });
        }

        public async Task<ServiceResult<bool>> DeleteFileByIdAsync(int id)
        {
            var fileRecord = _context.FileRecords.FirstOrDefault(f => f.Id == id);
            if (fileRecord == null)
            {
                return await Task.FromResult(new ServiceResult<bool>
                {
                    Success = false,
                    Message = "File record not found.",
                    StatusCode = 404,
                    Data = false
                });
            }

            _context.FileRecords.Remove(fileRecord);
            await _context.SaveChangesAsync();
            return new ServiceResult<bool>
            {
                Success = true,
                Message = "File record deleted successfully.",
                StatusCode = 200,
                Data = true
            };
        }

        public async Task<ServiceResult<FileDownloadResponseDto>> DownloadFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new ServiceResult<FileDownloadResponseDto>
                {
                    Success = false,
                    Message = "File name is required.",
                    StatusCode = 400
                };
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return new ServiceResult<FileDownloadResponseDto>
                {
                    Success = false,
                    Message = "File not found.",
                    StatusCode = 404
                };
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            var response = new FileDownloadResponseDto
            {
                FileBytes = fileBytes,
                FileName = fileName,
                ContentType = GetContentType(fileExtension)
            };
            return new ServiceResult<FileDownloadResponseDto>
            {
                Success = true,
                Data = response,
                StatusCode = 200
            };
        }

        public async Task<List<FileListItemDto>> GetAllFilesAsync()
        {
            var query = _context.FileRecords.AsNoTracking().Select(f => new FileListItemDto
            {
                Id = f.Id,
                OriginalFileName = f.OriginalFileName,
                StoredFileName = f.StoredFileName,
                Size = f.Size,
                ContentType = f.ContentType,
                UploadedAt = f.UploadedAt
            });

            query = query.OrderByDescending(f => f.UploadedAt);
            return await query.ToListAsync();
        }

        public async Task<FileRecord?> GetFileByIdAsync(int id)
        {
            return await _context.FileRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FileDetailsResponseDto?> GetFileDetailsByIdAsync(int id)
        {
            var item = _context.FileRecords
                .AsNoTracking()
                .Where(f => f.Id == id)
                .Select(f => new FileDetailsResponseDto
                {
                    Id = f.Id,
                    OriginalFileName = f.OriginalFileName,
                    StoredFileName = f.StoredFileName,
                    FilePath = f.FilePath,
                    ContentType = f.ContentType,
                    FileSize = f.Size,
                    UploadedAt = f.UploadedAt
                }).FirstOrDefaultAsync();

            return await item;
        }

        public async Task<ServiceResult<FileUploadResponseDto>> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
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
            // Thursday, 04/17/2026 - Added save metadata to db
            var fileRecord = new FileRecord
            {
                OriginalFileName = file.FileName,
                StoredFileName = uniqueFileName,
                FilePath = filePath,
                ContentType = file.ContentType,
                Size = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            _context.FileRecords.Add(fileRecord);
            await _context.SaveChangesAsync();

            var response = new FileUploadResponseDto
            {
                FilePath = filePath,
                FileName = uniqueFileName,
                FileSize = file.Length,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType
            };
            return new ServiceResult<FileUploadResponseDto>
            {
                Success = true,
                Data = response,
                StatusCode = 200
            };
        }

        private string GetContentType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}
