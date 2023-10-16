using CircuitBreaker.Core.CustomExceptions;
using CircuitBreaker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CircuitBreaker.Core.Enums.CircuitBreakerEnums;

namespace CircuitBreaker.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ICircuitStatesService _circuitBreaker;

        public OrderService(ICircuitStatesService circuitBreaker)
        {
            _circuitBreaker = circuitBreaker;

        }

        public string PerformOperation()
        {
            try
            {
                if (_circuitBreaker.CurrentState != CircuitBreakerState.Open)
                {
                    // Perform the operation that needs circuit breaker protection
                    //that's Dummy API data, assume it's the payement gateway intefration service.
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri("https://dummy.restapiexample.com/api/v1/employee/1")
                    };
                    HttpClient httpClient = new HttpClient();
                    var response = httpClient.Send(request);
                    response.EnsureSuccessStatusCode();

                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    return responseBody;
                }


                else
                {
                    // Circuit breaker is open or half-open, handle accordingly

                  //  return "Service is not available. Please try after some time";
                    throw new CircuitOpenException();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string ExecuteOperation()
        {
            var circuitBreaker = new CircuitStatesService();

            try
            {
                string data = "";
                circuitBreaker.Execute(() =>
                {
                    data = PerformOperation();
                });
                return data;
            }
            catch (CircuitOpenException ex)
            {
                // Handle the circuit being open
                //return ex.Message;
                throw new CircuitOpenException();

            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return ex.Message;
            }
        }

    }
}