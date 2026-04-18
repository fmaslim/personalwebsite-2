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
            return StatusCode(response.StatusCode, response);
        }

        //[HttpGet("download/{fileName}")]
        //public async Task<IActionResult> DownloadFile(string fileName)
        //{
        //    var response = await _fileService.DownloadFileAsync(fileName);
        //    if (!response.Success || response.Data == null)
        //    {
        //        return StatusCode(response.StatusCode, response);
        //    }

        //    return File(response.Data.FileBytes, response.Data.ContentType, response.Data.FileName);
        //}

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

        //[HttpDelete("delete/{fileName}")]
        //public async Task<IActionResult> DeleteFile(string fileName)
        //{
        //    var response = await _fileService.DeleteFileAsync(fileName);
        //    return StatusCode(response.StatusCode, response);
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileById(int id)
        {
            var response = await _fileService.DeleteFileByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<FileListItemDto>>> GetAllFiles()
        {
            var files = await _fileService.GetAllFilesAsync();
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
 
