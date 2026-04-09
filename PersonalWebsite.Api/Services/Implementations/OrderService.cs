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
        public async Task<int> CreateOrderAsync(CreateOrderDto dto)
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

            return order.SalesOrderId;
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

        public async Task<IEnumerable<OrderDetailsDto>> SearchOrdersAsync(int? customerId, byte? status)
        {
            var query = _context.SalesOrderHeaders
                .AsNoTracking();

            if (customerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

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
