using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.ExceptionHandling;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.PerformanceTraining.Orders;
using Microsoft.ApplicationInsights;

namespace PersonalWebsite.Api.Controllers.PerformanceTraining
{
    [ApiController]
    [Route("/api/performance-training/v4/orders")]
    public class Ordersv4Controller : ApiControllerBase
    {
        private readonly IOrderv4Service _service;
        private readonly ILogger<Ordersv4Controller> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly IConfiguration _configuration;
        public Ordersv4Controller(
    IOrderv4Service service,
    ILogger<Ordersv4Controller> logger,
    TelemetryClient telemetryClient,
    IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _telemetryClient = telemetryClient;
            _configuration = configuration;
        }
        /// <summary>
        /// Cancels an order if the order is eligible to be cancelled.
        /// </summary>
        /// <param name="orderId">The order id to cancel.</param>
        /// <returns>The cancellation result.</returns>
        [HttpPost("{orderId}/cancel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CancelOrder(int orderId)
        {            
            var result = await _service.CancelOrderAsync(orderId);
            return result.ToActionResult();
        }

        [HttpGet("global-exception")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status500InternalServerError)]
        public IActionResult TestException()
        {
            throw new NotImplementedException();
        }

        [HttpGet("app-insight-200")]
        [Produces("application/json")]
        public IActionResult Endpoint_200()
        {
            return Ok("AppInsight test 200 Ok");
        }

        [HttpGet("app-insight-400")]
        public IActionResult Endpoint_400()
        {
            return BadRequest("AppInsight test 400 Bad Request");
        }

        [HttpGet("app-insight-404")]
        public IActionResult Endpoint_404()
        {
            return NotFound("AppInsight test 404 Not Found");
        }

        [HttpGet("app-insight-409")]
        public IActionResult Endpoint_409()
        {
            return Conflict("AppInsight test 409 Conflict");
        }

        [HttpGet("app-insight-service-result")]
        [Produces("application/json")]
        public IActionResult Endpoint_ServiceResult()
        {
            _logger.LogInformation("Endpoint_ServiceResult was hit");
            _logger.LogWarning("TRACE TEST - Endpoint_ServiceResult was hit");
            var result = new ServiceResult<string>
            {
                IsSuccess = false,
                Code = "ORDER_NOT_CREATED",
                ErrorType = "Validation",
                Message = "Order was not created because customer id is missing.",
                Data = null
            };

            _logger.LogWarning(
                "ServiceResult failed. Code={Code}, ErrorType={ErrorType}, Message={Message}",
                result.Code,
                result.ErrorType,
                result.Message);

            _telemetryClient.TrackTrace("DIRECT AI TRACE TEST - ServiceResult failed");
            _telemetryClient.Flush();
            return Ok(result);
        }

        [HttpGet("app-insight-config-test")]
        public IActionResult AppInsightConfigTest()
        {
            var connectionString = _configuration["ApplicationInsights:ConnectionString"];

            return Ok(new
            {
                HasConnectionString = !string.IsNullOrWhiteSpace(connectionString),
                StartsWithInstrumentationKey = connectionString?.StartsWith("InstrumentationKey="),
                ContainsIngestionEndpoint = connectionString?.Contains("IngestionEndpoint="),
                Length = connectionString?.Length
            });
        }
    }
}
