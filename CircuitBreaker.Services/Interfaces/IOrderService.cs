using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker.Services.Interfaces
{
    public interface IOrderService
    {
        public string PerformOperation(bool success);
        public string ExecuteOperation(bool success);
    }
}
