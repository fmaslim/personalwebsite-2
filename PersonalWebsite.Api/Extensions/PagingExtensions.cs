using PersonalWebsite.Api.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace PersonalWebsite.Api.Extensions
{
    public static class PagingExtensions
    {
        public static PagedResponse<T> ToPagedResponse<T>(this List<T> source, int pageNumber, int pageSize) where T : class
        {
            var pagedData = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResponse<T>
            {
                Data = pagedData,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = source.Count(),
                TotalPages = (int)Math.Ceiling(source.Count / (double)pageSize)
            };
        }

        public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize) where T : class
        {
            var totalRecords = await query.CountAsync();

            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResponse<T>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }
    }
}
