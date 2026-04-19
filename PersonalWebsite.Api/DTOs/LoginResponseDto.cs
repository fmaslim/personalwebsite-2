namespace PersonalWebsite.Api.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
