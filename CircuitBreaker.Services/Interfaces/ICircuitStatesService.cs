using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CircuitBreaker.Core.Enums.CircuitBreakerEnums;

namespace CircuitBreaker.Services.Interfaces
{
    public interface ICircuitStatesService
    {
        public CircuitBreakerState CurrentState { get; }

        public void ResetAfterTime(TimeSpan delay);
        public void Fail();
        //public void Reset();
        public void Execute(Action action);
    }
}
