namespace PersonalWebsite.Api.Models.Errors
{
    public enum ServiceErrorType
    {
        None = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unexpected = 4
    }
}
