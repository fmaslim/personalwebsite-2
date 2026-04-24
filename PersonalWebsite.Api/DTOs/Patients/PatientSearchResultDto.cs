namespace PersonalWebsite.Api.DTOs.Patients
{
    public class PatientSearchResultDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;        
    }
}
