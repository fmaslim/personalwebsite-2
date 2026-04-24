namespace PersonalWebsite.Api.DTOs.Files
{
    public class FileListItemDto
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = null!;
        public string StoredFileName { get; set; } = null!;
        public long Size { get; set; }
        public string ContentType { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
