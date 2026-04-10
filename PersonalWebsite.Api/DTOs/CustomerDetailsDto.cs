namespace PersonalWebsite.Api.DTOs
{
    public class CustomerDetailsDto
    {
        public int CustomerId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? TerritoryId { get; set; }
    }
}
