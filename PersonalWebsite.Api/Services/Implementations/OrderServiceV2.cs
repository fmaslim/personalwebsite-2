using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class OrderServiceV2 : IOrderServiceV2
    {
        private readonly AdventureWorksContext _context;
        public OrderServiceV2(AdventureWorksContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<CreateOrderResponseV2Dto>> CreateOrderAsync(CreateOrderRequestV2Dto dto)
        {
            /*
             * Service will:
                combine product names into one string
                save one Order row
                return CreateOrderResponseV2Dto
             */

            // 1. check item list is not empty
            // 2. check every quantity > 0
            if (dto.Items == null || !dto.Items.Any())
            {
                // throw new ArgumentException("Order must contain at least one item.");
                return new ServiceResult<CreateOrderResponseV2Dto>
                {
                    Success = false,
                    Message = "Order must contain at least one item.",
                    StatusCode = 400
                };
            }

            // 2.1 added new rule: duplicate product id is not allowed in the same order
            var hasDuplicateProducts = dto.Items.GroupBy(i => i.ProductId).Any(g => g.Count() > 1);
            if (hasDuplicateProducts)
            {
                // throw new ArgumentException("Duplicate product IDs are not allowed in the same order.");
                return new ServiceResult<CreateOrderResponseV2Dto>
                {
                    Success = false,
                    Message = "Duplicate product IDs are not allowed in the same order.",
                    StatusCode = 400
                };
            }

            // 3. check customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == dto.CustomerId);
            if (!customerExists)
            {
                // throw new ArgumentException($"Customer with ID {dto.CustomerId} does not exist.");
                return new ServiceResult<CreateOrderResponseV2Dto>
                {
                    Success = false,
                    Message = $"Customer with ID {dto.CustomerId} does not exist.",
                    StatusCode = 400
                };
            }

            // 4. check employee exists
            var employeeExists = await _context.Employees.AnyAsync(e => e.BusinessEntityId == dto.EmployeeId);
            if (!employeeExists)
            {
                // throw new ArgumentException($"Employee with ID {dto.EmployeeId} does not exist.");
                return new ServiceResult<CreateOrderResponseV2Dto>
                {
                    Success = false,
                    Message = $"Employee with ID {dto.EmployeeId} does not exist.",
                    StatusCode = 400
                };
            }

            // 5. check every product exists and calculate total
            decimal totalAmount = 0;
            var productNames = new List<string>();
            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                {
                    // throw new ArgumentException("Quantity must be greater than zero.");
                    return new ServiceResult<CreateOrderResponseV2Dto>
                    {
                        Success = false,
                        Message = "Quantity must be greater than zero.",
                        StatusCode = 400
                    };
                }
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                if (product == null)
                {
                    // throw new ArgumentException($"Product with ID {item.ProductId} does not exist.");
                    return new ServiceResult<CreateOrderResponseV2Dto>
                    {
                        Success = false,
                        Message = $"Product with ID {item.ProductId} does not exist.",
                        StatusCode = 400
                    };
                }
                totalAmount += product.ListPrice * item.Quantity;
                productNames.Add(product.Name);
            }

            // 6. create Order
            var order = new Order
            {
                UserId = dto.CustomerId, // Assuming UserId is same as CustomerId for simplicity
                ProductName = string.Join(", ", productNames),
                TotalAmount = totalAmount,
            };

            /*
             * Comment out temporarily to avoid affecting existing data. In real implementation, we would save the order and return the response.
             * _context.Orders.Add(order);
             * await _context.SaveChangesAsync();
             */

            // 7. return response
            var response = new CreateOrderResponseV2Dto
            {
                //OrderId = order.Id,
                OrderId = 0, // Since we are not actually saving to DB, we can return 0 or a dummy value
                CustomerId = dto.CustomerId,
                EmployeeId = dto.EmployeeId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount
            };

            return ServiceResult<CreateOrderResponseV2Dto>.Ok(response);
        }

        public string GetVersionMessage()
        {
            return "OrderServiceV2 is working.";
        }
    }
}
