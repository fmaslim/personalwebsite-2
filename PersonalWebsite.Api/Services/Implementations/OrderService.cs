using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AdventureWorksContext _context;
        public OrderService(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<int>> CreateOrderAsync(CreateOrderDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            if(dto.CustomerId <= 0)
            {
                throw new ArgumentException("CustomerId must be greater than zero.");
            }
            if(dto.BillToAddressId <= 0)
            {
                throw new ArgumentException("BillToAddressId must be greater than zero.");
            }
            if (dto.ShipMethodId <= 0)
            {
                throw new ArgumentException("ShipMethodId must be greater than zero.");
            }
            if (dto.TotalAmount <= 0)
            {
                throw new ArgumentException("TotalAmount must be greater than zero.");
            }
            // business rule: check if customer exists. If not, return error
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == dto.CustomerId);
            if (!customerExists)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = $"Customer with id {dto.CustomerId} does not exist.",
                    StatusCode = 404
                };
            }
            // check for same/duplicate order - same customer, same order date, same total amount
            var duplicateOrderExists = await _context.SalesOrderHeaders.AnyAsync(o =>
            o.CustomerId == dto.CustomerId &&
            o.OrderDate == dto.OrderDate &&
            o.SubTotal == dto.TotalAmount &&
            o.BillToAddressId == dto.BillToAddressId &&
            o.ShipToAddressId == dto.ShipToAddressId &&
            o.ShipMethodId == dto.ShipMethodId
            );
            if (duplicateOrderExists)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "A similar order already exists..",
                    StatusCode = 409
                };
            }
            var order = new SalesOrderHeader
            {
                CustomerId = dto.CustomerId,
                OrderDate = dto.OrderDate,
                DueDate = dto.OrderDate.AddDays(7),
                Status = 1,
                OnlineOrderFlag = true,
                BillToAddressId = dto.BillToAddressId,
                ShipToAddressId = dto.ShipToAddressId,
                ShipMethodId = dto.ShipMethodId,
                SubTotal = dto.TotalAmount,
                TaxAmt = 0,
                Freight = 0,
                //rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow
            };

            _context.SalesOrderHeaders.Add(order);
            await _context.SaveChangesAsync();

            // return order.SalesOrderId;
            return new ServiceResult<int>
            {
                Success = true,
                Message = "Order created successfully.",
                StatusCode = 201,
                Data = order.SalesOrderId
            };
        }

        public async Task<OrderDetailsDto?> GetOrderByIdAsync(int orderId)
        {
            var query = _context.SalesOrderHeaders
                .AsNoTracking()
                .Where(o => o.SalesOrderId == orderId)
                .Select(o => new OrderDetailsDto
                {
                    SalesOrderId = o.SalesOrderId,
                    CustomerId = o.CustomerId,
                    OrderDate = o.OrderDate,
                    DueDate = o.DueDate,
                    Status = o.Status,
                    OnlineOrderFlag = o.OnlineOrderFlag,
                    BillToAddressId = o.BillToAddressId,
                    ShipToAddressId = o.ShipToAddressId,
                    ShipMethodId = o.ShipMethodId,
                    SubTotal = o.SubTotal,
                    TaxAmt = o.TaxAmt,
                    Freight = o.Freight,
                    TotalDue = o.TotalDue
                });

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderDetailsDto>> SearchOrdersAsync(int? customerId, byte? status, DateTime? orderDateFrom, DateTime? orderDateTo, int? page, int? pageSize, string? sortBy, string? sortDir)
        {
            var query = _context.SalesOrderHeaders
                .AsNoTracking();

            // filter
            if (customerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }
            // filter
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }
            // filter - orderdatefrom
            if (orderDateFrom.HasValue)
            {
                             query = query.Where(o => o.OrderDate >= orderDateFrom.Value);
            }
            // filter - orderdateto
            if (orderDateTo.HasValue)
            {
                query = query.Where(o => o.OrderDate <= orderDateTo.Value);
            }
            // sort
            if (!string.IsNullOrEmpty(sortBy))
            {
                sortBy = sortBy?.ToLower();
                sortDir = sortDir?.ToLower();
                if (sortBy == "orderdate")
                {
                    query = sortDir == "desc" ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate);
                }
                else if (sortBy == "totaldue")
                {
                    query = sortDir == "desc" ? query.OrderByDescending(o => o.TotalDue) : query.OrderBy(o => o.TotalDue);
                }
                else
                {
                    query = query.OrderByDescending(o => o.OrderDate);
                }
            }
            else
            {
                query = query.OrderByDescending(o => o.OrderDate);
            }
            // skip
            if (page.HasValue)
            {
                int skip = (page.Value - 1) * (pageSize ?? 10);
                query = query.Skip(skip);
            }
            // take
            if (pageSize.HasValue)
            {
                query = query.Take(pageSize.Value);
            }
            
            // project
                return await  query.Select(o => new OrderDetailsDto
                {
                    SalesOrderId = o.SalesOrderId,
                    CustomerId = o.CustomerId,
                    OrderDate = o.OrderDate,
                    DueDate = o.DueDate,
                    Status = o.Status,
                    OnlineOrderFlag = o.OnlineOrderFlag,
                    BillToAddressId = o.BillToAddressId,
                    ShipToAddressId = o.ShipToAddressId,
                    ShipMethodId = o.ShipMethodId,
                    SubTotal = o.SubTotal,
                    TaxAmt = o.TaxAmt,
                    Freight = o.Freight,
                    TotalDue = o.TotalDue,
                })
                .ToListAsync();
        }
    }
}
