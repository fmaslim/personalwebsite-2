using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Orders;
using PersonalWebsite.Api.DTOs.PerformanceTraining;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;
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
                return ServiceResult<CreateOrderResponseV2Dto>.Fail(
                    "Order must contain at least one item.",
                    ServiceErrorType.Validation
                    );                
            }

            // 2.1 added new rule: duplicate product id is not allowed in the same order
            var hasDuplicateProducts = dto.Items.GroupBy(i => i.ProductId).Any(g => g.Count() > 1);
            if (hasDuplicateProducts)
            {
                return ServiceResult<CreateOrderResponseV2Dto>.Fail(
                    "Duplicate product IDs are not allowed in the same order.",
                    ServiceErrorType.Validation
                    );                
            }

            // 3. check customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == dto.CustomerId);
            if (!customerExists)
            {
                return ServiceResult<CreateOrderResponseV2Dto>.NotFound(
                    $"Customer with ID {dto.CustomerId} does not exist.",
                    "CustomerId"
                    );                
            }

            // 4. check employee exists
            var employeeExists = await _context.Employees.AnyAsync(e => e.BusinessEntityId == dto.EmployeeId);
            if (!employeeExists)
            {
                return ServiceResult<CreateOrderResponseV2Dto>.NotFound(
                $"Employee with ID {dto.EmployeeId} does not exist.",
                "EmployeeId");                
            }

            // 5. check every product exists and calculate total
            decimal totalAmount = 0;
            var productNames = new List<string>();
            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                {
                    return ServiceResult<CreateOrderResponseV2Dto>.Fail(
                "Quantity must be greater than zero.",
                ServiceErrorType.Validation);                    
                }
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                if (product == null)
                {
                    return ServiceResult<CreateOrderResponseV2Dto>.NotFound(
                    $"Product with ID {item.ProductId} does not exist.",
                    "ProductId");                    
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

            return ServiceResult<CreateOrderResponseV2Dto>.Created(response);
        }

        public async Task<ServiceResult<CreateOrderResponseV2Dto>> CreateOrderMultiErrorAsync(CreateOrderRequestV2Dto dto)
        {
            // var errors = new List<ServiceError>();
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
                return ServiceResult<CreateOrderResponseV2Dto>.Fail(
                errors,
                statusCode: 400);
            }

            // If no errors, proceed with order creation
            return await CreateOrderAsync(dto);
        }

        public async Task<ServiceResult<CreateOrderResponseV3Dto>> CreateOrderV3Async(CreateOrderRequestV3Dto dto)
        {
            // Added multi-error validation
            var fieldErrors = new List<FieldError>();
            // validation
            if (dto == null) // for this one, return immediately since we cant proceed without request body, for other validation we will collect all errors and return together
            {
                FieldError error = new FieldError
                {
                    Field = "requestBody",
                    Message = "Request body cannot be null."
                };
                fieldErrors.Add(error);

                return ServiceResult<CreateOrderResponseV3Dto>.ValidationFail(fieldErrors);
            }

            if (dto.UserId <= 0)
            {
                FieldError error = new FieldError
                {
                    Field = "userId",
                    Message = "UserId must be greater than 0."
                };
                fieldErrors.Add(error);
            }

            if (dto.Items == null || !dto.Items.Any())
            {
                FieldError error = new FieldError
                {
                    Field = "items",
                    Message = "Order must contain at least one item."
                };
                fieldErrors.Add(error);
            }
            else
            {
                if (dto.Items.Any(i => i.Quantity <= 0))
                {
                    var fieldError = new FieldError
                    {
                        Field = "items.quantity",
                        Message = "Quantity must be greater than zero for all items."
                    };
                    fieldErrors.Add(fieldError);
                }
                if (dto.Items.Any(i => i.ProductId <= 0))
                {
                    fieldErrors.Add(new FieldError
                    {
                        Field = "items.productId",
                        Message = "ProductId must be greater than 0 for all items."
                    });
                }
            }

            if (fieldErrors.Any())
            {
                return ServiceResult<CreateOrderResponseV3Dto>.ValidationFail(fieldErrors);
            }

            // user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                return ServiceResult<CreateOrderResponseV3Dto>.NotFound(
                $"User with ID {dto.UserId} does not exist.",
                "userId");                
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
                return ServiceResult<CreateOrderResponseV3Dto>.NotFound(
                $"One or more products do not exist: {string.Join(", ", missingProductIds)}.",
                "items.productId");
            }

            // stock validation
            // using SafetyStock Level for training purpose, in real scenario we should have a separate stock quantity field and update it when order is created.
            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.ProductId == item.ProductId);
                if (item.Quantity > product.SafetyStockLevel)
                {
                    //return ServiceResult<CreateOrderResponseV3Dto>.Fail(
                    //    $"Only {product.SafetyStockLevel} items left in stock for product ID {item.ProductId}.", 
                    //    ServiceErrorType.Validation);
                    return ServiceResult<CreateOrderResponseV3Dto>.Conflict(
                    $"Only {product.SafetyStockLevel} items left in stock for product ID {item.ProductId}.",
                    "items.quantity");
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

            var response = new CreateOrderResponseV3Dto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                CreatedAtUtc = order.CreatedAtUtc
            };
            return ServiceResult<CreateOrderResponseV3Dto>.Created(response, "Order created successfully.");
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

            var sortByValidation = ValidateSortBy(queryDto.SortBy);
            if (sortByValidation != null)
            {
                return sortByValidation;
            }

            var sortDirValidation = ValidateSortDir(queryDto.SortDir);
            if (sortDirValidation != null)
            {
                return sortDirValidation;
            }

            // Friday, 04/26/2026 - FromDate cant be greater than ToDate validation
            var fromDate = queryDto.FromDate?.Date;
            var toDate = queryDto.ToDate?.Date.AddDays(1).AddTicks(-1); // include the entire ToDate day            
            var validateDateRange = ValidateDateRange(fromDate, toDate);
            if (validateDateRange != null)
            { 
                return validateDateRange; 
            }

            var query = _context.Orders
                .AsNoTracking()                
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
            
            var validateAmountRange = ValidateAmountRange(queryDto.MinTotalAmount, queryDto.MaxTotalAmount);
            if (validateAmountRange != null)
            {
                return validateAmountRange;
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
            
            query = query                         
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

            var pagedResult = new PagedOrderSummaryResponseDto
            {
                Items = orderSummaries,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            
            return ServiceResult<PagedOrderSummaryResponseDto>.Ok(pagedResult);
        }

        public async Task<ServiceResult<GetOrderByIdResponseDto>> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                return ServiceResult<GetOrderByIdResponseDto>.ValidationFail(
    new List<FieldError>
                {
                    new FieldError
                    {
                        Field = "orderId",
                        Message = "OrderId must be greater than 0."
                    }
                });
            }

            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderDetails)                        
                .AsQueryable();
            query = query.Where(o => o.Id == orderId);
            var order = await query.FirstOrDefaultAsync();
            if (order == null)
            {
                return ServiceResult<GetOrderByIdResponseDto>.Fail(
                    $"Order with ID {orderId} does not exist.",
                    ServiceErrorType.NotFound);
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

            return ServiceResult<GetOrderByIdResponseDto>.Ok(response);
        }

        public string GetVersionMessage()
        {
            return "OrderServiceV2 is working.";
        }

        public async Task<ServiceResult<UpdateOrderStatusResponseDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequestDto dto)
        {
            if (orderId <= 0)
            {
                return ServiceResult<UpdateOrderStatusResponseDto>.Fail(
                    "OrderId must be greater than 0.",
                    ServiceErrorType.Validation);
            }

            if (dto == null)
            {
                return ServiceResult<UpdateOrderStatusResponseDto>.Fail(
                    "Request body is required.",
                    ServiceErrorType.Validation);
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return ServiceResult<UpdateOrderStatusResponseDto>.Fail(
                    $"Order with ID {orderId} does not exist.",
                    ServiceErrorType.NotFound);
            }

            var oldStatus = order.Status;
            var newStatus = dto.Status;
            if (!IsValidStatusTransition(oldStatus, newStatus))
            {
                return ServiceResult<UpdateOrderStatusResponseDto>.Fail(
                    $"Invalid status transition from {oldStatus} to {newStatus}.",
                    ServiceErrorType.Validation);
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();
            var data = new UpdateOrderStatusResponseDto
            {
                OrderId = order.Id,
                OldStatus = oldStatus.ToString(),
                NewStatus = newStatus.ToString()
            };
            return ServiceResult<UpdateOrderStatusResponseDto>.Ok(data);
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

        private ServiceResult<PagedOrderSummaryResponseDto>? ValidateSortDir(string? sortDir)
        {
            var allowedSortDir = new List<string> { "desc", "asc" };
            var requestedSortDir = sortDir?.ToLower() ?? "desc";

            if (!allowedSortDir.Contains(requestedSortDir))
            {
                return ServiceResult<PagedOrderSummaryResponseDto>.Fail(
                    message: "Invalid SortDir value",
                    field: "SortDir", 
                    statusCode: 400);
            }

            return null;
        }

        private ServiceResult<PagedOrderSummaryResponseDto>? ValidateSortBy(string? sortBy)
        {
            var allowedSortBy = new List<string> { "orderdate", "createdatutc", "status", "userid" };
            var requestedSortBy = sortBy?.ToLower() ?? "createdatutc";

            if (!allowedSortBy.Contains(requestedSortBy))
            {
                return ServiceResult<PagedOrderSummaryResponseDto>.Fail(
                    message: "Invalid SortBy value",
                    field: "SortBy",
                    statusCode: 400);
            }

            return null;
        }

        private ServiceResult<PagedOrderSummaryResponseDto>? ValidateAmountRange(decimal? min, decimal? max)
        {
            if (min.HasValue 
                && max.HasValue
                && min.Value> max.Value)
            {
                return ServiceResult<PagedOrderSummaryResponseDto>.Fail(
                    message: "MinTotalAmount cannot be greater than MaxTotalAmount",
                    field: "MinTotalAmount",
                    statusCode: 400);
            }

            return null;
        }

        private ServiceResult<PagedOrderSummaryResponseDto>? ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue
                && endDate.HasValue
                && startDate.Value > endDate.Value)
            {
                return ServiceResult<PagedOrderSummaryResponseDto>.Fail(
                message: "FromDate cannot be greater than ToDate.",
                field: "FromDate",
                statusCode: 400);
            }

            return null;
        }

        public async Task<ServiceResult<PagedResultDto<OrderSearchResponseDto>>> SearchOrdersAsync(DTOs.PerformanceTraining.OrderSearchRequestDto request)
        {
            // Sunday, 04/25/2026 - added protection against bad requests
            if (request.PageNumber <= 0)
            {
                return ServiceResult<PagedResultDto<OrderSearchResponseDto>>.Fail(
                    message: "PageNumber must be greater than 0.",
                    statusCode: 400
                    );
            }
            if (request.PageSize <= 0 || request.PageSize > 100)
            {
                return ServiceResult<PagedResultDto<OrderSearchResponseDto>>.Fail(
                    message: "PageSize must be between 1 and 100.",
                    statusCode: 400
                    );
            }

            // sortBy validation
            var sortBy = request.SortBy?.ToLower() ?? "createdatutc";
            var sortDir = request.SortDirection?.ToLower() ?? "desc";

            var allowedFields = new List<string> { "createdatutc", "totalamount", "status" };
            if (!allowedFields.Contains(sortBy))
            {
                return ServiceResult<PagedResultDto<OrderSearchResponseDto>>.Fail(
                    message: "SortBy must be one of: createdAtUtc, totalAmount, status.",
                    statusCode: 400
                    );
            }

            if (sortDir != "asc" && sortDir != "desc")
            {
                return ServiceResult<PagedResultDto<OrderSearchResponseDto>>.Fail(
                    message: "SortDirection must be either 'asc' or 'desc'.",
                    statusCode: 400
                    );
            }

            var query = _context.Orders
                .AsNoTracking()
                .AsQueryable();

            // add paging guardrails
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;

            // add filters
            if (request.CustomerId.HasValue)
            {
                query = query.Where(x => x.UserId == request.CustomerId.Value);
            }
            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == (OrderStatus)request.Status.Value);
            }
            if (request.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAtUtc >= request.FromDate.Value);
            }
            if (request.ToDate.HasValue)
            {
                query = query.Where(x => x.CreatedAtUtc <= request.ToDate.Value);
            }

            // after all filters
            var totalCount = await query.CountAsync();

            // Add sorting
            query = sortBy switch
            {
                "totalamount" => sortDir == "asc"
                                                ? query.OrderBy(x => x.TotalAmount)
                                                : query.OrderByDescending(x => x.TotalAmount),
                "status" => sortDir == "asc"
                                    ? query.OrderBy(x => x.Status)
                                    : query.OrderByDescending(x => x.Status),
                _ => sortDir == "asc"
                        ? query.OrderBy(x => x.CreatedAtUtc)
                        : query.OrderByDescending(x => x.CreatedAtUtc)            
            };

            // Add paging
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Add projection
            var orders = await query.Select(x => new OrderSearchResponseDto
            {
                OrderId = x.Id,
                CustomerId = x.UserId,
                CustomerName = null,
                Status = (int)x.Status,
                CreatedAtUtc = x.CreatedAtUtc,
                TotalAmount = x.TotalAmount
            }).ToListAsync();

            // return ServiceResult<List<OrderSearchResponseDto>>.Ok(orders);

            var pagedResultDto = new PagedResultDto<OrderSearchResponseDto>();
            pagedResultDto.Items = orders;
            pagedResultDto.PageNumber = pageNumber;
            pagedResultDto.PageSize = pageSize;
            pagedResultDto.TotalCount = totalCount;
            pagedResultDto.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return ServiceResult<PagedResultDto<OrderSearchResponseDto>>.Ok(pagedResultDto);
        }

        public async Task<ServiceResult<PagedResponse<OrderSearchResponseDto>>> SearchOrdersAsync(DTOs.Orders.OrderSearchRequestDto requestDto)
        {
            var query = _context.SalesOrderHeaders.AsNoTracking().AsQueryable();

            if (requestDto == null)
            {
                return ServiceResult<PagedResponse<OrderSearchResponseDto>>.Fail(
                    "Request object cannot be null.",
                    ServiceErrorType.Validation);
            }

            if (requestDto.PageNumber <= 0)
            {
                return ServiceResult<PagedResponse<OrderSearchResponseDto>>.Fail(
                    "PageNumber must be greater than 0.",
                    ServiceErrorType.Validation);
            }

            if (requestDto.PageSize <= 0)
            {
                return ServiceResult<PagedResponse<OrderSearchResponseDto>>.Fail(
                    "PageSize must be greater than 0.",
                    ServiceErrorType.Validation);
            }

            if (!string.IsNullOrWhiteSpace(requestDto.CustomerName))
            {
                query = query.Where(o =>
                    o.Customer != null &&
                    (
                        o.Customer.Person != null &&
                        (
                            o.Customer.Person.FirstName.Contains(requestDto.CustomerName) ||
                            o.Customer.Person.LastName.Contains(requestDto.CustomerName)
                        )
                        ||
                        o.Customer.Store != null &&
                        o.Customer.Store.Name.Contains(requestDto.CustomerName)
                    ));
            }

            // Added filters
            if (requestDto.OrderDateFrom.HasValue)
            {
                query = query.Where(o => o.OrderDate >=  requestDto.OrderDateFrom.Value);
            }

            if (requestDto.OrderDateTo.HasValue)
            {
                query = query.Where(o => o.OrderDate <=  requestDto.OrderDateTo.Value);
            }

            if (requestDto.MinTotalDue.HasValue)
            {
                query = query.Where(o => o.TotalDue >= requestDto.MinTotalDue.Value);
            }

            var totalCount = await query.CountAsync();

            // Sorting goes here
            var sortBy = requestDto.SortBy?.Trim().ToLower() ?? "orderdate";
            var sortDir = requestDto.SortDirection?.Trim().ToLower() ?? "desc";

            // Validate sortBy
            string[] validSortBy = ["orderdate", "totaldue", "customername"];
            //var sortBy = requestDto.SortBy?.ToLower();
            if (!validSortBy.Contains(sortBy))
            {
                return ServiceResult<PagedResponse<OrderSearchResponseDto>>.Fail(
                    "SortBy must be either orderdate, totaldue, or customername",
                    ServiceErrorType.Validation);
            }

            // Validate sortDirection
            if (sortDir != "asc" && sortDir != "desc")
            {
                return ServiceResult<PagedResponse<OrderSearchResponseDto>>.Fail(
                    "SortDirection must be either asc or desc",
                    ServiceErrorType.Validation);
            }

            // Apply sorting
            query = sortBy switch
            {
                "orderdate" => sortDir == "asc" ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate),
                "totaldue" => sortDir == "asc" ? query.OrderBy(o => o.TotalDue) : query.OrderByDescending(o => o.TotalDue),
                "customername" => sortDir == "asc"
                ? query.OrderBy(o =>
                    o.Customer.Person != null
                        ? o.Customer.Person.FirstName + " " + o.Customer.Person.LastName
                        : o.Customer.Store != null
                            ? o.Customer.Store.Name
                            : "")
                : query.OrderByDescending(o =>
                    o.Customer.Person != null
                        ? o.Customer.Person.FirstName + " " + o.Customer.Person.LastName
                        : o.Customer.Store != null
                            ? o.Customer.Store.Name
                            : ""),

                _ => query.OrderByDescending(o => o.OrderDate)
            };

            var data = await query
            .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
            .Take(requestDto.PageSize)
            .Select(o => new OrderSearchResponseDto
            {
                OrderId = o.SalesOrderId,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Person != null
                    ? o.Customer.Person.FirstName + " " + o.Customer.Person.LastName
                    : o.Customer.Store != null
                        ? o.Customer.Store.Name
                        : null,
                TotalAmount = o.TotalDue,
                Status = o.Status,
                CreatedAtUtc = o.OrderDate
            })
            .ToListAsync();

            var pagedResponse = new PagedResponse<OrderSearchResponseDto>
            {
                Items = data,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)requestDto.PageSize)
            };

            return ServiceResult<PagedResponse<OrderSearchResponseDto>>.Ok(pagedResponse, "Orders retrieved successfully");
        }
    }
}
