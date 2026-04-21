namespace PersonalWebsite.Api.DTOs
{
    public class LoginV3ResultDto
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public LoginResponseV3Dto? Data { get; set; }
    }
}
