namespace PersonalWebsite.Api.DTOs
{
    public class LoginResponseV2Dto
    {
        public string Username { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
}
