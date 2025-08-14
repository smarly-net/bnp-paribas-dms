using DMS.Web.BackgroundServices;
using DMS.Web.Controllers.Base;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Web.Controllers
{
    [Route("api/[controller]")]
    public class PingController : AuthorizeControllerBase
    {
        private readonly ILogger<PingController> _logger;

        public PingController(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<PingController>();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok("pong");
        }
    }
}
