namespace PersonalWebsite.Api.DTOs.Patients
{
    public class PagedPatientSearchResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<PatientSearchResultDto> Items { get; set; } = new();
    }
}
