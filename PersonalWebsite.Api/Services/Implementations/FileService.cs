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
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "fileName",
                            Message = "File name cannot be null or empty.",
                            Code = "InvalidFileName"
                        }
                    },
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
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "fileName",
                            Message = $"File '{fileName}' does not exist.",
                            Code = "FileNotFound"
                        }
                    },
                    StatusCode = 404
                });
            }

            System.IO.File.Delete(filePath);
            return Task.FromResult(new ServiceResult<string>
            {
                Success = true,
                Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "SuccessfulDelete",
                            Message = $"File '{fileName}' deleted successfully.",
                            Code = "FileDeleted"
                        }
                    },
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
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "fileRecord",
                            Message = "File record not found.",
                            Code = "FileRecordNotFound"
                        }
                    },
                    StatusCode = 404,
                    Data = false
                });
            }

            _context.FileRecords.Remove(fileRecord);
            await _context.SaveChangesAsync();
            return new ServiceResult<bool>
            {
                Success = true,
                Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "SuccessfulDelete",
                            Message = $"File record with ID '{id}' deleted successfully.",
                            Code = "FileRecordDeleted"
                        }
                    },
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
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "fileName",
                            Message = "File name cannot be null or empty.",
                            Code = "InvalidFileName"
                        }
                    },
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
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "fileName",
                            Message = $"File '{fileName}' does not exist.",
                            Code = "FileNotFound"
                        }
                    },
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

        public async Task<List<FileListItemDto>> GetAllFilesAsync(string? search, string? sortBy, string? sortDirection, int pageNumber, int pageSize)
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

            query = string.IsNullOrWhiteSpace(search)
                ? query
                : query.Where(f => f.OriginalFileName.Contains(search) || f.StoredFileName.Contains(search));

            // normalize sort inputs
            sortBy = sortBy?.Trim().ToLower();
            sortDirection = string.IsNullOrEmpty(sortDirection) ? "desc" : sortDirection.ToLower();

            query = (sortBy, sortDirection) switch
            {
                ("originalfilename", "asc") => query.OrderBy(f => f.OriginalFileName),
                ("originalfilename", "desc") => query.OrderByDescending(f => f.OriginalFileName),
                ("size", "asc") => query.OrderBy(f => f.Size),
                ("size", "desc") => query.OrderByDescending(f => f.Size),
                ("contenttype", "asc") => query.OrderBy(f => f.ContentType),
                ("contenttype", "desc") => query.OrderByDescending(f => f.ContentType),
                ("uploadedat", "asc") => query.OrderBy(f => f.UploadedAt),
                ("uploadedat", "desc") => query.OrderByDescending(f => f.UploadedAt),
                _ => query.OrderByDescending(f => f.UploadedAt)
            };

            // paging
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            
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

        public async Task<ServiceResult<FileDetailsResponseDto>> UpdateFileByIdAsync(int id, IFormFile newFile)
        {
            // 1. find existing record by id
            var file = await _context.FileRecords
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file == null)
            {
                // 2. return 404 if not found
                return new ServiceResult<FileDetailsResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "fileRecord",
                            Message = "File record not found.",
                            Code = "FileRecordNotFound"
                        }
                    },
                    StatusCode = 404
                };
            }
            // 3. validate new file (size, type)
            if (newFile == null || newFile.Length == 0)
            {
                return new ServiceResult<FileDetailsResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "file",
                            Message = "No file uploaded.",
                            Code = "NoFileUploaded"
                        }
                    },
                    StatusCode = 400
                };
            }
            var fileSizeValidationResult = ValidateFileSize(newFile);
            if (fileSizeValidationResult != null)
            {
                return fileSizeValidationResult;
            }
            // 4. validate file type
            var fileExtensionValidationResult = ValidateFileExtension(newFile);
            if (fileExtensionValidationResult != null)
            {
                return fileExtensionValidationResult;
            }
            // 5. generate unique file name and save new file to disk
            try
            {
                var uniqueFileName = GenerateUniqueFileName(newFile);
                var uploadsFolder = EnsureUploadsFolderExists();
                var newFilePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await newFile.CopyToAsync(stream);
                }
                // 6. delete old file from disk
                if (!string.IsNullOrEmpty(file.FilePath) && System.IO.File.Exists(file.FilePath))
                {
                    System.IO.File.Delete(file.FilePath);
                }
                // 7. update db record with new file info
                // FileRecord fileRecord = new FileRecord();
                file.OriginalFileName = newFile.FileName;
                file.StoredFileName = uniqueFileName;
                file.FilePath = newFilePath;
                file.ContentType = newFile.ContentType;
                file.Size = newFile.Length;
                file.UploadedAt = DateTime.UtcNow;

                // _context.FileRecords.Update(file);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new ServiceResult<FileDetailsResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "file",
                            Message = "An error occurred while updating the file.",
                            Code = "FileUpdateError"
                        }
                    },
                    StatusCode = 500
                };
            }

            // 8. return updated file details
            var response = new FileDetailsResponseDto
            {
                Id = file.Id,
                OriginalFileName = file.OriginalFileName,
                StoredFileName = file.StoredFileName,
                FilePath = file.FilePath,
                ContentType = file.ContentType,
                FileSize = file.Size,
                UploadedAt = file.UploadedAt
            };
            return new ServiceResult<FileDetailsResponseDto>
            {
                Success = true,
                Errors = null,
                Data = response,
                StatusCode = 200
            };
        }

        public async Task<ServiceResult<FileUploadResponseDto>> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new ServiceResult<FileUploadResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "file",
                            Message = "No file uploaded.",
                            Code = "NoFileUploaded"
                        }
                    },
                    StatusCode = 400
                };
            }

            var fileSizeValidationResult = ValidateFileSize(file);
            if (fileSizeValidationResult != null)
            {
                return new ServiceResult<FileUploadResponseDto>
                {
                    Success = false,
                    Errors = fileSizeValidationResult.Errors,
                    StatusCode = fileSizeValidationResult.StatusCode
                };
            }

            var fileExtensionValidationResult = ValidateFileExtension(file);
            if (fileExtensionValidationResult != null)
            {
                return new ServiceResult<FileUploadResponseDto>
                {
                    Success = false,
                    Errors = fileExtensionValidationResult.Errors,
                    StatusCode = fileExtensionValidationResult.StatusCode
                };
            }

            var uniqueFileName = GenerateUniqueFileName(file);
            var uploadsFolder = EnsureUploadsFolderExists();
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            try
            {
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
            }
            catch (Exception)
            {
                return new ServiceResult<FileUploadResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "file",
                            Message = "An error occurred while uploading the file.",
                            Code = "FileUploadError"
                        }
                    },
                    StatusCode = 500
                };
            }

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

        private ServiceResult<FileDetailsResponseDto>? ValidateFileSize(IFormFile file)
        {
            var maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize) // Limit file size to 5 MB
            {
                return new ServiceResult<FileDetailsResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "file",
                            Message = $"File size exceeds the limit of {maxFileSize / (1024 * 1024)} MB",
                            Code = "FileSizeExceeded"
                        }
                    },
                    StatusCode = 400
                };
            }
            return null;
        }

        private ServiceResult<FileDetailsResponseDto>? ValidateFileExtension(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", "", ".pdf", ".docx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return new ServiceResult<FileDetailsResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "file",
                            Message = $"File type '{fileExtension}' is not allowed.",
                            Code = "UnsupportedFileType"
                        }
                    },
                    StatusCode = 400
                };
            }
            return null;
        }

        private string GenerateUniqueFileName(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{fileExtension}";

            return uniqueFileName;
        }

        private string EnsureUploadsFolderExists()
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            return uploadsFolder;
        }
    }
}
