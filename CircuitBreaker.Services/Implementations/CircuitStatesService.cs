using CircuitBreaker.Core.CustomExceptions;
using CircuitBreaker.Services.Interfaces;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CircuitBreaker.Core.Enums.CircuitBreakerEnums;

namespace CircuitBreaker.Services.Implementations
{
    public class CircuitStatesService : ICircuitStatesService
    {
        private readonly StateMachine<CircuitBreakerState, CircuitBreakerTrigger> _stateMachine;
        private static DateTime? openTime; // Track when the circuit was opened
        private static int consecutiveFailures = 0; // Track the number of consecutive failures
        private static int failureThreshold = 3; // Set the threshold for consecutive failures
        private static dynamic initalState = CircuitBreakerState.Closed;

        public CircuitStatesService()
        {
            _stateMachine = new StateMachine<CircuitBreakerState, CircuitBreakerTrigger>(initalState);

            _stateMachine.Configure(CircuitBreakerState.Closed)
                .OnEntry(() => EnteredClosed())
                .Permit(CircuitBreakerTrigger.Failure, CircuitBreakerState.Open)
                .Permit(CircuitBreakerTrigger.Reset, CircuitBreakerState.HalfOpen);

            _stateMachine.Configure(CircuitBreakerState.Open)
                .OnEntry(() => OpenedEntered())
                .Permit(CircuitBreakerTrigger.Reset, CircuitBreakerState.HalfOpen);

            _stateMachine.Configure(CircuitBreakerState.HalfOpen)
                .OnEntry(() => HalfOpenedEntered())
                .Permit(CircuitBreakerTrigger.Success, CircuitBreakerState.Closed)
                .Permit(CircuitBreakerTrigger.Failure, CircuitBreakerState.Open)
                .Permit(CircuitBreakerTrigger.Reset, CircuitBreakerState.Closed);


        }
        public CircuitBreakerState CurrentState => initalState;


        public void Execute(Action action)
        {
            ResetAfterTime(TimeSpan.FromMinutes(1));
            if (CurrentState == CircuitBreakerState.Open)
            {
                // Circuit is open, don't execute the action
                throw new CircuitOpenException();
            }

            try
            {
                //await Task.Run(() => { action(); }) ;
                action();
                if (CurrentState == CircuitBreakerState.HalfOpen)
                {
                    _stateMachine.Fire(CircuitBreakerTrigger.Success);
                    initalState = CircuitBreakerState.Closed;
                }
            }
            catch (Exception ex)
            {
                if (CurrentState == CircuitBreakerState.Open == false && consecutiveFailures < failureThreshold)
                {
                    consecutiveFailures++;
                }
                //if the count of max attempt if reached, then open the circuits 
                if (consecutiveFailures == failureThreshold)
                {
                    if (CurrentState == CircuitBreakerState.Open == false)
                    {
                        _stateMachine.Fire(CircuitBreakerTrigger.Failure);
                    }
                }
                throw new Exception(ex.Message);
            }
        }



        public void ResetAfterTime(TimeSpan delay)
        {
            if (CurrentState == CircuitBreakerState.Open && openTime.HasValue && DateTime.Now - openTime.Value >= delay)
            {
                _stateMachine.Fire(CircuitBreakerTrigger.Reset);
            }
        }
        public void Fail()
        {
            consecutiveFailures++;
        }

        #region Private Methods


        private void Reset()
        {
            if (CurrentState == CircuitBreakerState.Open)
            {
                _stateMachine.Fire(CircuitBreakerTrigger.Reset);
                initalState = CircuitBreakerState.HalfOpen;
            }
        }

        private void OpenedEntered()
        {
            openTime = DateTime.Now;
            initalState = CircuitBreakerState.Open;
        }

        private void EnteredClosed()
        {
            initalState = CircuitBreakerState.Closed;
            consecutiveFailures = 0; // Reset consecutive failures when entering Closed state
        }

        private void HalfOpenedEntered()
        {
            initalState = CircuitBreakerState.HalfOpen;
        }

        #endregion

    }
}
