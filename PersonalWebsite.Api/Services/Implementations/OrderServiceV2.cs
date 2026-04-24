using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Orders;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;
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
                    Errors = new List<ServiceError> 
                    { 
                        new ServiceError 
                        { 
                            Field = "Items", 
                            Message = "Order must contain at least one item.",
                            Code = "EmptyItems"
                        } 
                    },
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
                    Errors = new List<ServiceError> 
                    { 
                        new ServiceError 
                        { 
                            Field = "Items", 
                            Message = "Duplicate product IDs are not allowed in the same order.",
                            Code = "DuplicateProducts"
                        } 
                    },
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
                    Errors = new List<ServiceError> 
                    { 
                        new ServiceError 
                        { 
                            Field = "CustomerId", 
                            Message = $"Customer with ID {dto.CustomerId} does not exist.",
                            Code = "CustomerNotFound"
                        } 
                    },
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
                    Errors = new List<ServiceError> 
                    { 
                        new ServiceError 
                        { 
                            Field = "EmployeeId", 
                            Message = $"Employee with ID {dto.EmployeeId} does not exist.",
                            Code = "EmployeeNotFound"
                        } 
                    },
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
                        Errors = new List<ServiceError> 
                        { 
                            new ServiceError 
                            { 
                                Field = "Quantity", 
                                Message = "Quantity must be greater than zero.",
                                Code = "InvalidQuantity"
                            } 
                        },
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
                        Errors = new List<ServiceError> 
                        { 
                            new ServiceError 
                            { 
                                Field = "ProductId", 
                                Message = $"Product with ID {item.ProductId} does not exist.",
                                Code = "ProductNotFound"
                            } 
                        },
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
                //ProductName = string.Join(", ", productNames),
                TotalAmount = totalAmount,
            };

            // *Comment out temporarily to avoid affecting existing data. In real implementation, we would save the order and return the response.
            
             _context.Orders.Add(order);
            await _context.SaveChangesAsync();


            // 7. return response
            var response = new CreateOrderResponseV2Dto
            {
                //OrderId = order.Id,
                OrderId = order.Id,
                CustomerId = dto.CustomerId,
                EmployeeId = dto.EmployeeId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount
            };

            return ServiceResult<CreateOrderResponseV2Dto>.Ok(response);
        }

        public async Task<ServiceResult<CreateOrderResponseV2Dto>> CreateOrderMultiErrorAsync(CreateOrderRequestV2Dto dto)
        {
            var errors = new List<ServiceError>();
            if (dto.Items == null || !dto.Items.Any())
            {
                errors.Add(new ServiceError
                {
                    Field = "Items",
                    Message = "Order must contain at least one item.",
                    Code = "EmptyItems"
                });
            }
            if(dto.Items != null && dto.Items.Any())
            {
                var hasDuplicateProducts = dto.Items.GroupBy(i => i.ProductId).Any(g => g.Count() > 1);
                if (hasDuplicateProducts)
                {
                    errors.Add(new ServiceError
                    {
                        Field = "Items",
                        Message = "Duplicate product IDs are not allowed in the same order.",
                        Code = "DuplicateProducts"
                    });
                }
            }
            for(int i = 0; i < dto.Items?.Count; i++)
            {
                var item = dto.Items[i];
                if (item.Quantity <= 0)
                {
                    errors.Add(new ServiceError
                    {
                        Field = $"Items[{i}].Quantity",
                        Message = "Quantity must be greater than zero.",
                        Code = "InvalidQuantity"
                    });
                }
                var productExists = await _context.Products.AnyAsync(p => p.ProductId == item.ProductId);
                if (!productExists)
                {
                    errors.Add(new ServiceError
                    {
                        Field = $"Items[{i}].ProductId",
                        Message = $"Product with ID {item.ProductId} does not exist.",
                        Code = "ProductNotFound"
                    });
                }
            }
            if(errors.Any())
            {
                return new ServiceResult<CreateOrderResponseV2Dto>
                {
                    Success = false,
                    Errors = errors,
                    StatusCode = 400
                };
            }

            // If no errors, proceed with order creation
            return await CreateOrderAsync(dto);
        }

        public async Task<ServiceResult<CreateOrderResponseV3Dto>> CreateOrderV3Async(CreateOrderRequestV3Dto dto)
        {
            // validation
            if (dto == null)
            {                 
                return new ServiceResult<CreateOrderResponseV3Dto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "Request",
                            Message = "Request body cannot be null.",
                            Code = "NullRequest"
                        }
                    },
                    StatusCode = 400
                };
            }

            if (dto.Items == null || !dto.Items.Any())
            {
                return new ServiceResult<CreateOrderResponseV3Dto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "Items",
                            Message = "Order must contain at least one item.",
                            Code = "EmptyItems"
                        }
                    },
                    StatusCode = 400
                };
            }

            if (dto.Items.Any(i => i.Quantity <= 0))
            {
                return new ServiceResult<CreateOrderResponseV3Dto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "Quantity",
                            Message = "Quantity must be greater than zero.",
                            Code = "InvalidQuantity"
                        }
                    },
                    StatusCode = 400
                };
            }

            // user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                return new ServiceResult<CreateOrderResponseV3Dto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "UserId",
                            Message = $"User with ID {dto.UserId} does not exist.",
                            Code = "UserNotFound"
                        }
                    },
                    StatusCode = 400
                };
            }

            // Load products
            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();

            var products = await _context.Products
             .Where(p => productIds.Contains(p.ProductId))
             .ToListAsync();

            if (products.Count != productIds.Count)
            {
                var existingProductIds = products.Select(p => p.ProductId);
                var missingProductIds = productIds.Except(existingProductIds);
                return new ServiceResult<CreateOrderResponseV3Dto>
                {
                    Success = false,
                    Errors = missingProductIds.Select(id => new ServiceError
                    {
                        Field = "Items.ProductId",
                        Message = $"Product with ID {id} does not exist.",
                        Code = "ProductNotFound"
                    }).ToList(),
                    StatusCode = 400
                };
            }

            // stock validation
            // using SafetyStock Level for training purpose, in real scenario we should have a separate stock quantity field and update it when order is created.
            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.ProductId == item.ProductId);
                if (item.Quantity > product.SafetyStockLevel)
                {
                    return new ServiceResult<CreateOrderResponseV3Dto>
                    {
                        Success = false,
                        Errors = new List<ServiceError>
                        {
                            new ServiceError
                            {
                                Field = $"Items[ProductId={item.ProductId}].Quantity",
                                Message = $"Only {product.SafetyStockLevel} items left in stock for product ID {item.ProductId}.",
                                Code = "InsufficientStock"
                            }
                        },
                        StatusCode = 400
                    };
                }
            }

            // create order

            var order = new Order
            {
                UserId = dto.UserId,
                CreatedAtUtc = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 0
            };

            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.ProductId == item.ProductId);

                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.ListPrice
                };

                order.OrderDetails.Add(orderDetail);

                totalAmount += item.Quantity * product.ListPrice;
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new ServiceResult<CreateOrderResponseV3Dto>
            {
                Success = true,
                Data = new CreateOrderResponseV3Dto
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalAmount,
                    CreatedAtUtc = order.CreatedAtUtc
                },
                StatusCode = 201
            };
        }

        public async Task<ServiceResult<PagedOrderSummaryResponseDto>> GetAllOrdersAsync(OrderQueryParamsDto queryDto)
        {
            var userId = queryDto.UserId;
            var status = queryDto.Status;
            var pageNumber = queryDto.PageNumber;
            var pageSize = queryDto.PageSize;
            var sortBy = queryDto.SortBy;
            var sortOrder = queryDto.SortDir;
            
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }
            if (pageSize > 100)
            {
                pageSize = 100; // max page size limit
            }

            // Friday, 04/24/2026 - added allowable Sort Fields
            var allowedSortFields = new List<string> { "orderdate", "createdatutc", "status", "userid" };
            var requestedSortBy = queryDto.SortBy?.ToLower() ?? "orderdate";
            if (!allowedSortFields.Contains(requestedSortBy))
            {
                return ServiceResult<PagedOrderSummaryResponseDto>.Fail(
        message: $"Invalid SortBy value. Allowed values: {string.Join(", ", allowedSortFields)}",        
        field: "SortBy", 
        statusCode: 400);
            }

            // Friday, 04/24/2026 - added sorting direction validation
            var allowedSortDir = new List<string> { "asc", "desc" };
            var requestedSortDir = queryDto.SortDir?.ToLower() ?? "desc";
            if (!allowedSortDir.Contains(requestedSortDir))
            {
                return ServiceResult<PagedOrderSummaryResponseDto>.Fail(
                message: "Invalid SortDir value",
                field: "SortDir",
                statusCode: 400);
            }

            // Friday, 04/26/2026 - FromDate cant be greater than ToDate validation
            var fromDate = queryDto.FromDate?.Date;
            var toDate = queryDto.ToDate?.Date.AddDays(1).AddTicks(-1); // include the entire ToDate day

            // Future option:
            // Use exclusive end date instead of AddTicks(-1):
            // var toDateExclusive = queryDto.ToDate?.Date.AddDays(1);
            // query = query.Where(o => o.CreatedAtUtc < toDateExclusive.Value);

            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            {
                return ServiceResult<PagedOrderSummaryResponseDto>.Fail(
        message: "FromDate cannot be greater than ToDate.",
        field: "FromDate",
        statusCode: 400);
            }

            var query = _context.Orders
                .AsNoTracking()
                //.Include(o => o.OrderDetails)
                .AsQueryable();

            if (queryDto.MinTotalAmount.HasValue)
            {
                // add filter for minimum total amount
                query = query.Where(o => o.TotalAmount >= queryDto.MinTotalAmount.Value);
            }
            if(queryDto.MaxTotalAmount.HasValue)
            {
                // add filter for maximum total amount
                query = query.Where(o => o.TotalAmount <= queryDto.MaxTotalAmount.Value);
            }
             if (queryDto.FromDate.HasValue)
            {
                // add filter for order created date from
                query = query.Where(o => o.CreatedAtUtc >= queryDto.FromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(o => o.CreatedAtUtc <= toDate.Value);
            }
            // Friday, 04/24/2026 - added validation MinTotalAmount cannot be greater than MaxTotalAmount
            if (queryDto.MinTotalAmount.HasValue
                && queryDto.MaxTotalAmount.HasValue
                && queryDto.MinTotalAmount > queryDto.MaxTotalAmount)
            {
                return ServiceResult<PagedOrderSummaryResponseDto>.Fail(
                    message:"MinTotalAmount cannot be greater than MaxTotalAmount",
                    field: "MinTotalAmount",
                    statusCode: 400);
            }

            if (!string.IsNullOrWhiteSpace(queryDto.Search))
            {
                var search = queryDto.Search.ToLower();

                query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.UserId.ToString().Contains(search));
            }
            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var isDesc = sortOrder?.ToLower() != "asc";
            query = requestedSortBy switch
            {
                "orderdate" or "createdatutc" => isDesc ? query.OrderByDescending(o => o.CreatedAtUtc) : query.OrderBy(o => o.CreatedAtUtc),
                "status" => isDesc ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),
                "userid" => isDesc ? query.OrderByDescending(o => o.UserId) : query.OrderBy(o => o.UserId),
                _ => query.OrderByDescending(o => o.CreatedAtUtc)
            };

            query = query
                         // .OrderByDescending(o => o.CreatedAtUtc)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

            var orders = await query.ToListAsync();

            var orderSummaries = orders.Select(o => new OrderSummaryResponseDto
            {
                OrderId = o.Id,
                UserId = o.UserId,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                CreatedAtUtc = o.CreatedAtUtc
            }).ToList();

            //var serviceResult = new ServiceResult<List<OrderSummaryResponseDto>>
            //{
            //    Success = true,
            //    Data = orderSummaries,
            //    StatusCode = 200
            //};
            // return serviceResult;
            var pagedResult = new PagedOrderSummaryResponseDto
            {
                Items = orderSummaries,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return new ServiceResult<PagedOrderSummaryResponseDto>
            {
                Success = true,
                Data = pagedResult,
                StatusCode = 200
            };
        }

        public async Task<ServiceResult<GetOrderByIdResponseDto>> GetOrderByIdAsync(int orderId)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderDetails)                        
                .AsQueryable();
            query = query.Where(o => o.Id == orderId);
            var order = await query.FirstOrDefaultAsync();
            if (order == null)
            {
                return new ServiceResult<GetOrderByIdResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "OrderId",
                            Message = $"Order with ID {orderId} does not exist.",
                            Code = "OrderNotFound"
                        }
                    },
                    StatusCode = 404
                };
            }

            var response = new GetOrderByIdResponseDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                CreatedAtUtc = order.CreatedAtUtc,
                Items = order.OrderDetails.Select(od => new OrderDetailResponseDto
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };

            return new ServiceResult<GetOrderByIdResponseDto>
            {
                Success = true,
                Data = response,
                StatusCode = 200
            };
        }

        public string GetVersionMessage()
        {
            return "OrderServiceV2 is working.";
        }

        public async Task<ServiceResult<UpdateOrderStatusResponseDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequestDto dto)
        {
            if (dto == null)
            {
                return new ServiceResult<UpdateOrderStatusResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                    {
                        new ServiceError
                        {
                            Field = "Request",
                            Message = "Request body cannot be null.",
                            Code = "NullRequest"
                        }
                    },
                    StatusCode = 400
                };
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return new ServiceResult<UpdateOrderStatusResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                        {
                            new ServiceError
                            {
                                Field = "OrderId",
                                Message = $"Order with ID {orderId} does not exist.",
                                Code = "OrderNotFound"
                            }
                        },
                    StatusCode = 404
                };
            }
            var oldStatus = order.Status;
            var newStatus = dto.Status;
            if (!IsValidStatusTransition(oldStatus, newStatus))
            {
                return new ServiceResult<UpdateOrderStatusResponseDto>
                {
                    Success = false,
                    Errors = new List<ServiceError>
                        {
                            new ServiceError
                            {
                                Field = "Status",
                                Message = $"Invalid status transition from {oldStatus} to {newStatus}.",
                                Code = "InvalidStatusTransition"
                            }
                        },
                    StatusCode = 400
                };
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return new ServiceResult<UpdateOrderStatusResponseDto>
            {
                Success = true,
                Data = new UpdateOrderStatusResponseDto
                {
                    OrderId = order.Id,
                    OldStatus = oldStatus.ToString(),
                    NewStatus = newStatus.ToString()
                },
                StatusCode = 200
            };
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return currentStatus switch
            { 
                OrderStatus.Pending => newStatus == OrderStatus.Paid || newStatus == OrderStatus.Cancelled,
                OrderStatus.Paid => newStatus == OrderStatus.Shipped || newStatus == OrderStatus.Cancelled,
                OrderStatus.Shipped => newStatus == OrderStatus.Delivered,
                OrderStatus.Delivered => false,
                OrderStatus.Cancelled => false,
                _ => false
            };
        }
    }
}
