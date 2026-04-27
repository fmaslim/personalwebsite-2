namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Patients
{
    public class PatientSearchResultDto
    {
        public int PatientId { get; set; }

        public string? FullName { get; set; }

        public int Age { get; set; }

        public string? Gender { get; set; }

        public string? Status { get; set; }

        public DateTime LastVisitDate { get; set; }
    }
}
