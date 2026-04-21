namespace PersonalWebsite.Api.DTOs
{
    public class LoginV2ResultDto
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public LoginResponseV2Dto? Data { get; set; }
    }
}
