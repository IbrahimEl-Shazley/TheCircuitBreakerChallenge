using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker.Core.CustomExceptions
{
    public class CircuitOpenException : Exception
    {
        public CircuitOpenException() : base("Circuit is open.") { }
    }
}
