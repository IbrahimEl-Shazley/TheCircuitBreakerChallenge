﻿using CircuitBreaker.Core.CustomExceptions;
using CircuitBreaker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker.Services.Implementations
{
    public class AuthService : IAuthService
    {
        public bool ForgetPassword()
        {
            //Implement Foget Password Logic
            return true;
        }

        public bool ExecuteOperation()
        {
            var circuitBreaker = new CircuitStatesService();

            bool data = false;
            circuitBreaker.Execute(() =>
            {
                data = ForgetPassword();
            });
            return data;
        }

    }
}
