using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Orders;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;
using PersonalWebsite.Api.Services.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using OrderSearchResultDto = PersonalWebsite.Api.DTOs.PerformanceTraining.Orders.OrderSearchResultDto;
using PT = PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AdventureWorksContext _context;
        private readonly ILogger<OrderService> _logger;
        public OrderService(AdventureWorksContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<OrderDetailsDto>> GetOrderByIdAsync(int orderId)
        {
            _logger.LogInformation(
                "Getting order by id. OrderId: {OrderId}",
                orderId);

            if (orderId <= 0)
            {
                _logger.LogWarning(
                "Get order by id validation failed. OrderId: {OrderId}",
                orderId);

                return ServiceResult<OrderDetailsDto>.Fail(
                    "OrderId must be greater than 0",
                    ServiceErrorType.Validation);
            }

            var query = _context.SalesOrderHeaders
                .AsNoTracking()
                .AsQueryable()
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

            var order = await query.FirstOrDefaultAsync();
            if (order == null)
            {
                _logger.LogWarning(
                "Order not found. OrderId: {OrderId}",
                orderId);

                return ServiceResult<OrderDetailsDto>.NotFound($"Order with id {orderId} does not exist.");
            }

            _logger.LogInformation(
            "Order found. OrderId: {OrderId}",
            orderId);

            return ServiceResult<OrderDetailsDto>.Ok(order);
        }

        public async Task<ServiceResult<PagedResponse<OrderDetailsDto>>> SearchOrdersAsync(int? customerId, byte? status, DateTime? orderDateFrom, DateTime? orderDateTo, int? page, int? pageSize, string? sortBy, string? sortDir)
        {
            var fieldErrors = new List<FieldError>();
            if (customerId.HasValue && customerId.Value <= 0)
            {
                fieldErrors.Add(new FieldError { Field = "CustomerId", Message = "CustomerId must be greater than 0." });
            }
            if (status.HasValue && status.Value is < 1 or > 6)
            {
                fieldErrors.Add(new FieldError { Field = "status", Message = "Status must be between 1 and 6." });
            }
            if (orderDateFrom.HasValue && orderDateTo.HasValue && orderDateFrom.Value > orderDateTo.Value)
            {
                fieldErrors.Add(new FieldError { Field = "orderDateFrom, orderDateTo", Message = "orderDateFrom cannot be greater than orderDateTo." });
            }
            if (page.HasValue && page.Value <= 0)
            {
                fieldErrors.Add(new FieldError { Field = "page", Message = "Page number must be greater than 0." });
            }
            if (pageSize.HasValue && pageSize.Value <= 0)
            {
                fieldErrors.Add(new FieldError { Field = "pageSize", Message = "Page size must be greater than 0." });
            }
            if (pageSize.HasValue && pageSize.Value > 100)
            {
                fieldErrors.Add(new FieldError { Field = "pageSize", Message = "Page size cannot be greater than 100." });
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                var validSortByValues = new List<string> { "orderdate", "totaldue" };
                if (!validSortByValues.Contains(sortBy.ToLower()))
                {
                    fieldErrors.Add(new FieldError { Field = "sortBy", Message = $"Invalid sortBy value. Valid values are: {string.Join(", ", validSortByValues)}." });
                }
            }
            if (!string.IsNullOrWhiteSpace(sortDir))
            {
                var validSortDirValues = new List<string> { "asc", "desc" };
                if (!validSortDirValues.Contains(sortDir.ToLower()))
                {
                    fieldErrors.Add(new FieldError { Field = "sortDir", Message = $"Invalid sortDir value. Valid values are: {string.Join(", ", validSortDirValues)}." });
                }
            }
            if (fieldErrors.Any())
            {
                return ServiceResult<PagedResponse<OrderDetailsDto>>.ValidationFail(fieldErrors);
            }

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
            if (!string.IsNullOrWhiteSpace(sortBy))
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

            var totalCount = await query.CountAsync();
            var pageNumber = page ?? 1;
            var recordsPerPage = pageSize ?? 10;

            // skip
            int skip = (pageNumber - 1) * recordsPerPage;
            query = query.Skip(skip);

            // take
            query = query.Take(recordsPerPage);


            // project
            var orders = await query.Select(o => new OrderDetailsDto
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

            var pagedResponse = new PagedResponse<OrderDetailsDto>
            {
                Items = orders,
                PageNumber = pageNumber,
                PageSize = recordsPerPage,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)recordsPerPage)
            };

            return ServiceResult<PagedResponse<OrderDetailsDto>>.Ok(pagedResponse);
        }

        public async Task<PagedResponse<OrderSearchResultDto>> SearchOrdersBadN1QueryAsync(DTOs.PerformanceTraining.OrderSearchRequestDto requestDto)
        {
            // Newbie mistake:
            // Load the page of orders first, then query related data inside the loop.

            var orders = await _context.SalesOrderHeaders
                                    .AsNoTracking()
                                    .OrderByDescending(o => o.OrderDate)
                                    .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
                                    .Take(requestDto.PageSize)
                                    .ToListAsync();

            var totalCount = await _context.SalesOrderHeaders.CountAsync();

            var data = new List<OrderSearchResultDto>();

            // Bad: Loop through data to get other data to fill out a prop.
            foreach (var order in orders)
            {
                var customer = await _context.Customers.Where(c => c.CustomerId == order.CustomerId).FirstOrDefaultAsync();

                string? customerName = string.Empty;
                if (customer != null)
                {
                    var person = customer.PersonId != null
                        ? await _context.People.AsNoTracking().FirstOrDefaultAsync(p => p.BusinessEntityId == customer.CustomerId)
                        : null;

                    var store = customer.StoreId != null
                        ? await _context.Stores.AsNoTracking().FirstOrDefaultAsync(s => s.BusinessEntityId == customer.StoreId)
                        : null;

                    customerName = person != null
                        ? person.FirstName + " " + person.LastName
                        : store != null
                            ? store.Name
                            : null;

                    var itemCount = await _context.SalesOrderDetails
                        .AsNoTracking()
                        .CountAsync(s => s.SalesOrderId == order.SalesOrderId);

                    data.Add(new OrderSearchResultDto
                    {
                        //SalesOrderId = order.SalesOrderId,
                        //SalesOrderNumber = order.SalesOrderNumber,
                        OrderDate = order.OrderDate,
                        CustomerName = customerName,
                        //TotalDue = order.TotalDue,
                        //ItemCount = itemCount
                    });
                }
            }

            return new PagedResponse<OrderSearchResultDto>
            {
                Items = data,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)requestDto.PageSize)
            };

        }

        public async Task<PagedResponse<OrderSearchResultDto>> SearchOrdersGoodQueryAsync(DTOs.PerformanceTraining.OrderSearchRequestDto requestDto)
        {
            var query = _context.SalesOrderHeaders.AsNoTracking()
                .Select(o => new OrderSearchResultDto
                {
                    //SalesOrderId = o.SalesOrderId,
                    //SalesOrderNumber = o.SalesOrderNumber,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer != null
                    ? o.Customer.Person != null
                        ? o.Customer.Person.FirstName + " " + o.Customer.Person.LastName
                        : o.Customer.Store != null
                            ? o.Customer.Store.Name
                            : null
                    : null,
                    //TotalDue = o.TotalDue,
                    //ItemCount = o.SalesOrderDetails.Count()
                });

            var totalCount = await query.CountAsync();

            var data = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
            .Take(requestDto.PageSize)
            .ToListAsync();

            return new PagedResponse<OrderSearchResultDto>
            {
                Items = data,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)requestDto.PageSize)
            };
        }

        public async Task<ServiceResult<DTOs.Orders.CreateOrderResponseDto>> CreateOrderAsync(CreateOrderDto dto)
        {
            if (dto == null)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(
                    "Request object cannot be null",
                    Models.Errors.ServiceErrorType.Validation
                    );
            }
            if (dto.CustomerId <= 0)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(
                    "CustomerId must be greater than 0.",
                    Models.Errors.ServiceErrorType.Validation
                    );
            }
            if (dto.BillToAddressId <= 0)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(
                    "BillToAddressId must be greater than 0.",
                    Models.Errors.ServiceErrorType.Validation
                    );
            }
            if (dto.ShipMethodId <= 0)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(
                    "ShipMethodId must be greater than 0.",
                    Models.Errors.ServiceErrorType.Validation
                    );
            }
            if (dto.TotalAmount <= 0)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(
                    "TotalAmount must be greater than 0.",
                    Models.Errors.ServiceErrorType.Validation
                    );
            }
            // business rule: check if customer exists. If not, return error
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == dto.CustomerId);
            if (!customerExists)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(
                    $"Customer with id: {dto.CustomerId} does not exist.",
                    Models.Errors.ServiceErrorType.NotFound);
            }
            // check for same or duplicate orders (same customer, same order date, same total amount)
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
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(
                    "A similar order already exists. Please check your order details.",
                    Models.Errors.ServiceErrorType.Conflict);
            }
            // All validations passed. Now create order
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

            var response = new DTOs.Orders.CreateOrderResponseDto
            {
                OrderId = order.SalesOrderId,
            };

            return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Created(response);
        }

        Task<PagedResponse<PT.OrderSearchResultDto>> IOrderService.SearchOrdersBadN1QueryAsync(DTOs.PerformanceTraining.OrderSearchRequestDto requestDto)
        {
            throw new NotImplementedException();
        }

        Task<PagedResponse<PT.OrderSearchResultDto>> IOrderService.SearchOrdersGoodQueryAsync(DTOs.PerformanceTraining.OrderSearchRequestDto requestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult<DTOs.Orders.CreateOrderResponseDto>> CreateOrderV2Async(CreateOrderRequestDto dto)
        {
            var serviceErrors = new List<ServiceError>();
            // start with validation
            if (dto.UserId <= 0)
            {
                serviceErrors.Add(new ServiceError
                {
                    Field = "UserId",
                    Message = "UserId must be greater than 0.",
                    Type = ServiceErrorType.Validation
                });
            }
            if (dto.TotalAmount <= 0)
            {
                serviceErrors.Add(new ServiceError
                {
                    Field = "TotalAmount",
                    Message = "TotalAmount must be greater than 0.",
                    Type = ServiceErrorType.Validation
                });
            }
            var allowedStatusValues = new[] { 1, 2, 3 };
            if (!allowedStatusValues.Contains(dto.Status))
            {
                serviceErrors.Add(new ServiceError
                {
                    Field = "Status",
                    Message = $"Status must be one of the following values: {string.Join(", ", allowedStatusValues)}.",
                    Type = ServiceErrorType.Validation
                });
            }
            if (serviceErrors.Any())
            {
                _logger.LogWarning(
                "Create order validation failed. UserId: {UserId}, Status: {Status}, TotalAmount: {TotalAmount}, Errors: {Errors}",
                dto.UserId,
                dto.Status,
                dto.TotalAmount,
                serviceErrors.Select(e => e.Message));
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(serviceErrors);
            }
            // Add conflict validation. So the validation order is:
            //1. Basic validation
            //2. If validation errors → return 400
            //3. Business conflict rule
            //4. If conflict → return 409
            //5. Create order
            if (dto.TotalAmount > 10000)
            {
                _logger.LogWarning(
                "Create order blocked by business rule. UserId: {UserId}, Status: {Status}, TotalAmount: {TotalAmount}",
                dto.UserId,
                dto.Status,
                dto.TotalAmount);
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Conflict(
                    "Orders over 10000 require manager approval.",
                    "TotalAmount");
            }

            // Validations passed. Now create Order
            var order = new Order
            {
                UserId = dto.UserId,
                CreatedAtUtc = DateTime.UtcNow,
                Status = (OrderStatus)dto.Status,
                TotalAmount = dto.TotalAmount
            };
            // Save Order to DB
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            // Add logging
            _logger.LogInformation(
            "Order created successfully. OrderId: {OrderId}, UserId: {UserId}, Status: {Status}, TotalAmount: {TotalAmount}",
            order.Id,
            order.UserId,
            order.Status,
            order.TotalAmount);

            var response = new DTOs.Orders.CreateOrderResponseDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                CreatedAtUtc = order.CreatedAtUtc,
                Status = (int)order.Status,
                TotalAmount = order.TotalAmount
            };
            return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Created(response);
        }

        public async Task<ServiceResult<DTOs.Orders.CreateOrderResponseDto>> UpdateOrderAsync(int id, DTOs.Orders.UpdateOrderRequestDto dto)
        {
            var errors = new List<string>();
            if (id <= 0)
            {
                errors.Add("Id must be greater than 0.");
            }
            if (dto == null)
            {
                errors.Add("Order data is required");
            }
            if (dto != null)
            {
                //if (string.IsNullOrWhiteSpace(dto.ProductName))
                //{
                //    errors.Add("ProductName is required.");
                //}
                if (dto.TotalAmount <= 0)
                {
                    errors.Add("TotalAmount must be greater than 0.");
                }
            }
            if (errors.Any())
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(errors);
            }
            // update order
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.NotFound($"Order with id {id} does not exist.");
            }
            // update editable fields
            order.TotalAmount = dto.TotalAmount;
            await _context.SaveChangesAsync();
            return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Ok(new DTOs.Orders.CreateOrderResponseDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                CreatedAtUtc = order.CreatedAtUtc,
                Status = (int)order.Status,
                TotalAmount = order.TotalAmount
            });
        }

        public async Task<ServiceResult<DTOs.Orders.CreateOrderResponseDto>> DeleteOrderAsync(int id)
        {
            var serviceErrors = new List<ServiceError>();
            if (id <= 0)
            {
                serviceErrors.Add(new ServiceError
                {
                    Field = "Id",
                    Message = "Id must be greater than 0.",
                    Type = ServiceErrorType.Validation
                });
            }
            if (serviceErrors.Any())
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(serviceErrors);
            }
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.NotFound($"Order with id {id} does not exist.");
            }
            // is already deleted
            if (order.IsDeleted)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.NotFound($"Order with id {id} is already deleted.");
            }
            // soft delete
            order.IsDeleted = true;
            order.DeletedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.NoContent("Order deleted successfully.");
        }

        public async Task<ServiceResult<DTOs.Orders.CreateOrderResponseDto>> PatchOrderAsync(int id, DTOs.Orders.PatchOrderRequestV2Dto dto)
        {
            var serviceErrors = new List<ServiceError>();
            if (id <= 0)
            {
                serviceErrors.Add(new ServiceError
                {
                    Field = "Id",
                    Message = "Id must be greater than 0.",
                    Type = ServiceErrorType.Validation
                });
            }
            if (dto == null)
            {
                serviceErrors.Add(new ServiceError
                {
                    Field = "Request body",
                    Message = "Request body cannot be null.",
                    Type = ServiceErrorType.Validation
                });
            }
            if (dto != null)
            {
                if (!dto.Status.HasValue && !dto.TotalAmount.HasValue)
                {
                    serviceErrors.Add(new ServiceError
                    {
                        Field = "Status, TotalAmount",
                        Message = "At least one field (Status or TotalAmount) must be provided for update.",
                        Type = ServiceErrorType.Validation
                    });
                }

                var allowedStatusValues = new[] { 1, 2, 3 };
                if (dto.Status.HasValue && !allowedStatusValues.Contains(dto.Status.Value))
                {
                    serviceErrors.Add(new ServiceError
                    {
                        Field = "Status",
                        Message = $"Status must be one of the following values: {string.Join(", ", allowedStatusValues)}.",
                        Type = ServiceErrorType.Validation
                    });
                }
                if (dto.TotalAmount.HasValue && dto.TotalAmount.Value <= 0)
                {
                    serviceErrors.Add(new ServiceError
                    {
                        Field = "TotalAmount",
                        Message = "TotalAmount must be greater than 0.",
                        Type = ServiceErrorType.Validation
                    });
                }
            }
            if (serviceErrors.Any())
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Fail(serviceErrors);
            }
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.NotFound($"Order with id {id} does not exist.");
            }
            if(order.IsDeleted)
            {
                return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.NotFound($"Order with id {id} is deleted.");
            }
            if (dto.Status.HasValue)
            {
                order.Status = (OrderStatus)dto.Status.Value;
            }

            if (dto.TotalAmount.HasValue)
            {
                order.TotalAmount = dto.TotalAmount.Value;
            }
            await _context.SaveChangesAsync();
            var response = new DTOs.Orders.CreateOrderResponseDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                CreatedAtUtc = order.CreatedAtUtc,
                Status = (int)order.Status,
                TotalAmount = order.TotalAmount
            };
            return ServiceResult<DTOs.Orders.CreateOrderResponseDto>.Ok(response);
        }        
    }
}
