using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker.Core.Enums
{
    public class CircuitBreakerEnums
    {
        public enum CircuitBreakerState
        {
            Closed,
            Open,
            HalfOpen
        }

        public enum CircuitBreakerTrigger
        {
            Failure,
            Success,
            Reset
        }
    }
}
