namespace PersonalWebsite.Api.Models
{
    public class FileRecord
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = null!;
        public string StoredFileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Size { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
