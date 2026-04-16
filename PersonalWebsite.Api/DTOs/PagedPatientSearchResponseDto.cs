namespace PersonalWebsite.Api.DTOs
{
    public class PagedPatientSearchResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<PatientSearchResultDto> Items { get; set; } = new();
    }
}
