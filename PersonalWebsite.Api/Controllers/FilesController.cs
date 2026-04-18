using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
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
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response.Message);
            }
            return Ok(response.Data);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var response = await _fileService.GetFileByIdAsync(id);
            if (response == null)
            {
                return NotFound();
            }
            if (!System.IO.File.Exists(response.FilePath))
            {
                return NotFound("Physical file not found.");
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(response.FilePath);
            return File(fileBytes, response.ContentType, response.OriginalFileName);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileById(int id)
        {
            var response = await _fileService.DeleteFileByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<FileListItemDto>>> GetAllFiles([FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var files = await _fileService.GetAllFilesAsync(search, sortBy, sortDirection, pageNumber, pageSize);
            return Ok(files);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FileDetailsResponseDto>> GetFileDetailsById(int id)
        {
            var fileDetails = await _fileService.GetFileDetailsByIdAsync(id);
            if (fileDetails == null)
            {
                return NotFound();
            }
            return Ok(fileDetails);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFileByIdAsync(int id, IFormFile file)
        {
            var fileDetails = await _fileService.UpdateFileByIdAsync(id, file);
            
            if(!fileDetails.Success)
            {
                return StatusCode(fileDetails.StatusCode, fileDetails.Message);
            }
            return Ok(fileDetails.Data);
        }
    }
}
 
