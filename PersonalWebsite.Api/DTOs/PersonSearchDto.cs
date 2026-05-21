namespace PersonalWebsite.Api.DTOs;

public class PersonSearchDto
{
    public int BusinessEntityId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
