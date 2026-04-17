using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {

            var response = await _fileService.UploadFileAsync(file);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var response = await _fileService.DownloadFileAsync(fileName);
            if (!response.Success || response.Data == null)
            {
                return StatusCode(response.StatusCode, response);
            }

            return File(response.Data.FileBytes, response.Data.ContentType, response.Data.FileName);
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            var response = await _fileService.DeleteFileAsync(fileName);
            return StatusCode(response.StatusCode, response);
        }
    }
}
 
