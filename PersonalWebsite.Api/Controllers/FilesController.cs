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
