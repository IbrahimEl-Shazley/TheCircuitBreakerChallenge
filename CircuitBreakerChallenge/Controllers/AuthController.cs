using CircuitBreaker.Core.Filters;
using CircuitBreaker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace CircuitBreakerChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }


        [HttpPost]
        [Throttle(Name = "ForgetPassword", Seconds = 60)]
        public IActionResult ForgetPassword()
        {
            return Ok(_auth.ExecuteOperation());
        }
    }
}
