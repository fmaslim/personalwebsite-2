using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<ActionResult<FileUploadResponseDto>> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }
            if(file.Length > 1 * 1024 * 1024) // Limit file size to 1 MB
            {
                return BadRequest("File size exceeds the limit of 1 MB");
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Unsupported file type. Only .jpg, .jpeg, .png, .pdf, .docx files are allowed.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var response = new FileUploadResponseDto
            {
                FilePath = filePath,
                FileName = file.FileName,
                FileSize = file.Length,
                Message = "File uploaded successfully"
            };
            return Ok(response);
        }
    }
}
