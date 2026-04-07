namespace PersonalWebsite.Api.DTOs
{
    public class EmployeeLookupDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateOnly HireDate { get; set; }
        public bool CurrentFlag { get; set; }
    }
}
