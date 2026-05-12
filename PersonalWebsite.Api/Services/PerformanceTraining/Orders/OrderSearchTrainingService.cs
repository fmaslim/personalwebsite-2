using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Orders
{
    public class OrderSearchTrainingService : IOrderSearchTrainingService
    {
        private readonly AdventureWorksContext _context;
        public OrderSearchTrainingService(AdventureWorksContext context)
        {
            _context = context;
        }
        
        //public async Task<PagedResponse<SearchOrderResultDto>> SearchOrdersAsync(SearchOrderRequestDto dto)
        //{
        //    dto.PageNumber = dto.PageNumber < 1 ? 1 : dto.PageNumber;
        //    dto.PageSize = dto.PageSize < 1 ? 10 : dto.PageSize;
        //    dto.PageSize = dto.PageSize > 100 ? 100 : dto.PageSize;
        //    var query = _context.Orders.AsNoTracking().AsQueryable();

        //    // Add filters
        //    if (dto.CustomerId.HasValue)
        //    {
        //        query = query.Where(o => o.UserId == dto.CustomerId.Value);
        //    }
        //    if (!string.IsNullOrWhiteSpace(dto.Status))
        //    {
        //        if (Enum.TryParse<OrderStatus>(dto.Status, true, out var status))
        //        {
        //            query = query.Where(o => o.Status == status);
        //        }
        //    }
        //    if (dto.FromDate.HasValue)
        //    {
        //        query = query.Where(o => o.CreatedAtUtc >= dto.FromDate.Value);
        //    }
        //    if (dto.ToDate.HasValue)
        //    {
        //        query = query.Where(o => o.CreatedAtUtc <= dto.ToDate.Value);
        //    }
        //    if (dto.MinTotal.HasValue)
        //    {
        //        query = query.Where(o => o.TotalAmount >= dto.MinTotal.Value);
        //    }
        //    if (dto.MaxTotal.HasValue)
        //    {
        //        query = query.Where(o => o.TotalAmount <= dto.MaxTotal.Value);
        //    }

        //    // Add sorting
        //    var sortBy = dto.SortBy?.ToLower();
        //    var sortDir = dto.SortDir?.ToLower();

        //    query = sortBy switch
        //    {
        //        "totalamount" => sortDir == "asc" ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount),
        //        "status" => sortDir == "asc" ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
        //        "createdatutc" => sortDir == "asc" ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc),
        //        _ => sortDir == "asc" ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc)
        //    };

        //    var totalCount = await query.CountAsync();

        //    var items = await query
        //        .Skip((dto.PageNumber - 1) * dto.PageSize)
        //        .Take(dto.PageSize)
        //        .Select(o => new SearchOrderResultDto
        //        {
        //            OrderId = o.Id,
        //            CustomerId = o.UserId,
        //            CustomerName = "", // fill later if you join/include user
        //            Status = o.Status.ToString(),
        //            OrderDate = o.CreatedAtUtc,
        //            TotalAmount = o.TotalAmount
        //        }).ToListAsync();

        //    // return items.ToPagedResponse(dto.PageNumber, dto.PageSize);
        //    return new PagedResponse<SearchOrderResultDto>
        //    {
        //        Data = items,
        //        PageNumber = dto.PageNumber,
        //        PageSize = dto.PageSize,
        //        TotalRecords = totalCount,
        //        TotalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize),
        //    };
        //}

        public async Task<ServiceResult<PagedResponse<SearchOrderResultDto>>> SearchOrdersAsync(SearchOrderRequestDto dto)
        {
            if (dto == null)
            {
                return ServiceResult<PagedResponse<SearchOrderResultDto>>.Fail(
                    "Request object is required", 
                    ServiceErrorType.Validation);
            }
            // Page validation
            if (dto.PageNumber <= 0)
            {
                return ServiceResult<PagedResponse<SearchOrderResultDto>>.Fail(
                    "PageNumber must be greater than 0.",
                    ServiceErrorType.Validation);
            }
            if (dto.PageSize <= 0)
            {
                return ServiceResult<PagedResponse<SearchOrderResultDto>>.Fail(
                    "PageSize must be greater than 0.",
                    ServiceErrorType.Validation);
            }

            // start search
            var query = _context.Orders.AsNoTracking().AsQueryable();

            if (dto.CustomerId.HasValue)
            {
                if (dto.CustomerId.Value <= 0)
                {
                    return ServiceResult<PagedResponse<SearchOrderResultDto>>.Fail(
                    "CustomerId must be greater than 0.",
                    ServiceErrorType.Validation);
                }
                query = query.Where(o => o.UserId == dto.CustomerId.Value);
            }

            // Count
            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.CreatedAtUtc)
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(o => new SearchOrderResultDto 
                { 
                    OrderId = o.Id,
                    CustomerId = o.UserId,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    
                })
                .ToListAsync();


            var response = new PagedResponse<SearchOrderResultDto>()
            {
                Data = items,
                TotalRecords = totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize)
            };

            return ServiceResult<PagedResponse<SearchOrderResultDto>>.Ok(response);
        }
    }
}
