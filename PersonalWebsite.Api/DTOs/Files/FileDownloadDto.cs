namespace PersonalWebsite.Api.DTOs.Files
{
    public class FileDownloadDto
    {
        public byte[] FileBytes { get; set; } = [];
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
