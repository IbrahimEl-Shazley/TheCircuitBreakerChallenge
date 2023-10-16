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

            if (_circuitBreaker.CurrentState == CircuitBreakerState.Open)
                throw new CircuitOpenException();


            // Perform the operation that needs circuit breaker protection
            //that's Dummy API data, assume it's the payement gateway integration service.
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                //RequestUri = new Uri("https://dummy.restapiexample.com/api/v1/employee/1")
                RequestUri = new Uri("https://reqres.in/api/users/2")

            };
            HttpClient httpClient = new HttpClient();
            var response = httpClient.Send(request);
            response.EnsureSuccessStatusCode();

            var responseBody = response.Content.ReadAsStringAsync().Result;
            return responseBody;

        }

        public string ExecuteOperation()
        {
            var circuitBreaker = new CircuitStatesService();


            string data = "";
            circuitBreaker.Execute(() =>
            {
                data = PerformOperation();
            });
            return data;

        }

    }
}