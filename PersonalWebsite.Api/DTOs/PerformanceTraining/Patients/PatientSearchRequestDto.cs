using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Patients
{
    public class PatientSearchRequestDto : PagingRequestDto
    {
        public string? Search { get; set; }

        public string? Status { get; set; }

        public string? Gender { get; set; }

        public int? MinAge { get; set; }

        public int? MaxAge { get; set; }

        public string? SortBy { get; set; }

        public string? SortDir { get; set; }
    }
}
