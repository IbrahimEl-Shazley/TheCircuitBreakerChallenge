using CircuitBreaker.Core.CustomExceptions;
using CircuitBreaker.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CircuitBreakerChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IOrderService _Order;
        public OrderController(IOrderService Order)
        {
            _Order = Order;
        }

        [HttpGet(Name = "GetData")]
        public async Task<IActionResult> GetData(bool success)
        {
            try
            {
                // Perform your protected operation here
                // For example, make an HTTP request to an external service
                var result = _Order.ExecuteOperation(success);

                return Ok(result);
            }
            catch (CircuitOpenException)
            {
                // Handle the circuit breaker being open
                return StatusCode(503, "Service is not available. The Circuit Is Open Please try after some time");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, ex.Message);
            }
        }
    }
}
