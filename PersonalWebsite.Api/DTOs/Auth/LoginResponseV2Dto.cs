namespace PersonalWebsite.Api.DTOs.Auth
{
    public class LoginResponseV2Dto
    {
        public string Username { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }

        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
