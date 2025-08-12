using DMS.Web.Controllers.Base;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Web.Controllers
{
    [Route("api/[controller]")]
    public class PingController : AuthorizeControllerBase
    {
        private readonly ILogger<PingController> _logger;

        public PingController(ILogger<PingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            _logger.LogInformation($"Method {nameof(PingController.Get)}");
            return Ok("pong");
        }
    }
}
