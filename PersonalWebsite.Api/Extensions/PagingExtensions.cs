using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.Extensions
{
    public static class PagingExtensions
    {
        public static PagedResponse<T> ToPagedResponse<T>(this List<T> source, int pageNumber, int pageSize) where T : class
        {
            var pagedData = source.Skip(pageNumber - 1).Take(pageSize).ToList();
            return new PagedResponse<T>
            {
                Data = pagedData,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = source.Count(),
                TotalPages = (int)Math.Ceiling(source.Count / (double)pageSize)
            };
        }
    }
}
