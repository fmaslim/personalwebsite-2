namespace PersonalWebsite.Api.DTOs
{
    public class FileUploadResponseDto
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
