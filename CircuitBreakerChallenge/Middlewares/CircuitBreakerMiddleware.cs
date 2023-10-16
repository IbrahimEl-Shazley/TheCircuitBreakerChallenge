using CircuitBreaker.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static CircuitBreaker.Core.Enums.CircuitBreakerEnums;

namespace CircuitBreakerChallenge.Middlewares
{

    public class CircuitBreakerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICircuitStatesService _circuitStateService;
        private readonly ConcurrentQueue<HttpContext> _requestQueue;

        public CircuitBreakerMiddleware(RequestDelegate next, ICircuitStatesService circuitStateService)
        {
            _next = next;
            _circuitStateService = circuitStateService;
            _requestQueue = new ConcurrentQueue<HttpContext>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _circuitStateService.ResetAfterTime(TimeSpan.FromMinutes(1));
            if (_circuitStateService.CurrentState == CircuitBreakerState.Open)
            {
                // Circuit breaker is open, enqueue the request
                _requestQueue.Enqueue(context);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("the circuit is open please try again later");

                return;
            }

            try
            {
                // Process the request
                await _next(context);

                // Check if the circuit breaker is in the half-open state
                if (_circuitStateService.CurrentState == CircuitBreakerState.HalfOpen)
                {
                    // Process any pending requests in the queue
                    await ProcessPendingRequests();
                }
            }
            catch (Exception)
            {
                // An exception occurred, mark the circuit breaker as tripped
                _circuitStateService.Fail();
                throw;
            }
        }

        private async Task ProcessPendingRequests()
        {
            while (_requestQueue.TryDequeue(out var queuedContext))
            {
                // Process the queued request
                await _next(queuedContext);
            }
        }
    }



    public static class CircuitBreakerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCircuitBreakerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CircuitBreakerMiddleware>();
        }
    }
}

