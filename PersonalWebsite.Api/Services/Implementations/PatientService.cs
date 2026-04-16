using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly AdventureWorksContext _context;
        public PatientService(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PatientSearchResultDto>> SearchPatientsAsync(string? firstName, string? lastName, int pageNumber, int pageSize)
        {
            var query = _context.People.AsNoTracking().AsQueryable();

            // filter by firstName, check if null
            if ((!string.IsNullOrEmpty(firstName)))
            {
                query = query.Where(p => p.FirstName.Contains(firstName));
            }
            // filter by lastName, check if null
            if ((!string.IsNullOrEmpty(lastName)))
            {
                query = query.Where(p => p.LastName.Contains(lastName));
            }
            // paging
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var result = query.Select(p => new PatientSearchResultDto
            {
                Id = p.BusinessEntityId,
                FirstName = p.FirstName,
                LastName = p.LastName                
            }).ToListAsync();

            return await result;
        }
    }
}
